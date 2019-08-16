using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic.Models
{
    public partial class Trick : ITrick
    {
        private IHand _mockHand = null;
        [NotMapped]
        public IHand IHand {
            get { return _mockHand ?? Hand; }
            private set {
                _mockHand = null;
                if (value is Hand) Hand = (Hand)value; else _mockHand = value;
            }
        }
        public List<IPlayer> Players => IHand.Players;
        [NotMapped]
        public IPlayer StartingPlayer {
            get { return StartingParticipant.Player; }
            private set { StartingParticipant = value.Participant; }
        }

        public virtual Dictionary<IPlayer, SheepCard> CardsByPlayer {
            get {
                if (TrickPlays == null)
                    throw new NullReferenceException("TrickPlays is null");
                if (TrickPlays.Any(tp => tp.Participant == null))
                    throw new NullReferenceException("Participant is null");
                return TrickPlays.Any() 
                    ? TrickPlays.ToDictionary(tp => tp.Participant.Player, tp => CardUtil.GetCardFromAbbreviation(tp.Card).Value) 
                    : new Dictionary<IPlayer, SheepCard>();
            }
        }
        public int QueueRankOfPicker => IHand.Picker.QueueRankInTrick(this);
        public int? QueueRankOfPartner => IHand.Partner == null ? (int?)null : IHand.Partner.QueueRankInTrick(this);
        public List<IPlayer> PlayersWithoutTurn => PlayerOrderer.PlayersWithoutTurn(Players, StartingPlayer, CardsByPlayer.Keys.ToList());

        public List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves =>
            TrickPlays
                .OrderBy(tp => tp.SortOrder)
                .Select(trickPlay => new KeyValuePair<IPlayer, SheepCard>( 
                    trickPlay.Participant.Player, 
                    CardUtil.GetCardFromAbbreviation(trickPlay.Card).Value 
                 ))
                .ToList();

        public Trick(IHand hand) : this (hand, new StartingPlayerCalculator())
        {
        }

        public Trick(IHand hand, IStartingPlayerCalculator startingPlayerCalculator) : this()
        {
            IHand = hand;
            IHand.AddTrick(this);
            SortOrder = IHand?.ITricks.Count() ?? 0;
            StartingPlayer = startingPlayerCalculator.GetStartingPlayer(hand, this);
        }

        public void Add(IPlayer player, SheepCard card)
        {
            TrickPlays.Add(new TrickPlay()
            {
                Trick = this,
                Participant = player.Participant,
                Card = CardUtil.GetAbbreviation(card),
                SortOrder = TrickPlays.Count() + 1
            });
            player.RemoveCard(card);
            if (IHand.PartnerCardEnum == card)
                IHand.SetPartner(player, this);
        }

        public bool IsLegalAddition(SheepCard card, IPlayer player)
        {
            //In the last trick, anything is legal.
            if (player.Cards.Count() == 1)
                return true;

            var moves = OrderedMoves;
            //There are some rules for the lead card in a trick.
            if (!moves.Any())
                return IHand.IGame.PartnerMethodEnum == PartnerMethod.JackOfDiamonds
                    || IHand.PartnerCardEnum == null
                    || IsLegalStartingCardInCalledAceGame(card, player);

            var firstCard = moves.First().Value;
            //Only applys to called ace games
            if (IHand.IGame.PartnerMethodEnum == PartnerMethod.CalledAce && IHand.PartnerCardEnum.HasValue)
                return player.Cards.Contains(IHand.PartnerCardEnum.Value)
                        ? card == IHand.PartnerCardEnum.Value && CardFollowsSuit(card, player, firstCard)
                        : CardFollowsSuit(card, player, firstCard);

            //Other cards must follow suit.
            return player.Cards.Contains(card) && CardFollowsSuit(card, player, firstCard);
        }

        private static bool CardFollowsSuit(SheepCard card, IPlayer player, SheepCard firstCard)
        {
            return (CardUtil.GetSuit(card) == CardUtil.GetSuit(firstCard) || !player.Cards.Any(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(firstCard)));
        }

        private bool IsLegalStartingCardInCalledAceGame(SheepCard card, IPlayer player)
        {
            var suitOfPartnerCard = CardUtil.GetSuit(IHand.PartnerCardEnum.Value);
            if (player == IHand.Picker)
                return true;
            //Partner cannot lead with card in partner suit unless partner card has already been played or card is the partner card.
            return !IHand.PartnerCardEnum.HasValue
                || !player.Cards.Contains(IHand.PartnerCardEnum.Value)
                || card == IHand.PartnerCardEnum
                || CardUtil.GetSuit(card) != CardUtil.GetSuit(IHand.PartnerCardEnum.Value);
        }

        public TrickWinner Winner()
        {
            var moves = OrderedMoves;
            if (!moves.Any())
                return null;
            var firstSuite = CardUtil.GetSuit(moves.First().Value);
            var validCards = new List<KeyValuePair<IPlayer, SheepCard>>();
            foreach(var keyValuePair in moves) {
                var suite = CardUtil.GetSuit(keyValuePair.Value);
                if (suite == firstSuite || suite == Suit.TRUMP)
                    validCards.Add(keyValuePair);
            }
            return new TrickWinner()
            {
                Player = validCards.OrderBy(kvp => CardUtil.GetRank(kvp.Value)).First().Key,
                Points = moves.Sum(c => CardUtil.GetPoints(c.Value))
            };
        }

        public virtual bool IsComplete()
        {
            return CardsByPlayer.Count() == IHand.PlayerCount;
        }
    }

    public class TrickWinner {
        public IPlayer Player;
        public int Points;
    }

    public interface ITrick
    {
        IHand IHand { get; }
        List<IPlayer> Players { get; }
        IPlayer StartingPlayer { get; }

        Dictionary<IPlayer, SheepCard> CardsByPlayer { get; }
        List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves { get; }

        int QueueRankOfPicker { get; }
        int? QueueRankOfPartner { get; }
        List<IPlayer> PlayersWithoutTurn { get; }

        void Add(IPlayer player, SheepCard card);
        bool IsLegalAddition(SheepCard card, IPlayer player);
        bool IsComplete();
        TrickWinner Winner();
    }
}