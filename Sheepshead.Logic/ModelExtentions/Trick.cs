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
        public IGame IGame => IHand.IGame;
        [NotMapped]
        public IPlayer StartingPlayer { get { return StartingParticipant.Player; } private set { StartingParticipant = value.Participant; } }
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

        //TODO: Use the TrickPlay.SortOrder property instead of trying to figure out the order of the players.
        public List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves 
        { 
            get 
            {
                var indexOfStartingPlayer = Players.IndexOf(StartingPlayer);
                var playerList = Players.Skip(indexOfStartingPlayer).Union(Players.Take(indexOfStartingPlayer)).ToList();
                var orderedMoves = new List<KeyValuePair<IPlayer, SheepCard>>();
                var cards = CardsByPlayer;
                foreach (var player in playerList)
                    if (cards.ContainsKey(player))
                        orderedMoves.Add(new KeyValuePair<IPlayer, SheepCard>( player, cards[player] ));
                return orderedMoves;
            } 
        }

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

            //Other cards must follow suit.
            var firstCard = moves.First().Value;
            return player.Cards.Contains(card) 
                && (CardUtil.GetSuit(card) == CardUtil.GetSuit(firstCard) || !player.Cards.Any(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(firstCard)));
        }

        private bool IsLegalStartingCardInCalledAceGame(SheepCard card, IPlayer player)
        {
            var suitOfPartnerCard = CardUtil.GetSuit(IHand.PartnerCardEnum.Value);
            //Once suit of partner card is lead, picker and partner may lead with that suit.
            if (IHand.ITricks != null
                && IHand.ITricks.Any(t => t != this && t.CardsByPlayer.Any() && CardUtil.GetSuit(t.CardsByPlayer.First().Value) == suitOfPartnerCard))
                return true;
            //Picker cannot lead with last card of Called Ace's suit.
            if (player == IHand.Picker
                && CardUtil.GetSuit(card) == suitOfPartnerCard
                && player.Cards.Union(IHand.Buried).ToList().Count(c => CardUtil.GetSuit(c) == suitOfPartnerCard) == 1)
                return false;
            //Partner cannot lead with partner card.
            if (IHand.PartnerCardEnum == card)
                return false;
            return true;
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

        public List<IPlayer> Players
        {
            get { return IHand.Players; }
        }

        public int QueueRankOfPicker
        {
            get { return IHand.Picker.QueueRankInTrick(this); }
        }

        public int? QueueRankOfPartner
        {
            get { return IHand.Partner == null ? (int?)null : IHand.Partner.QueueRankInTrick(this); } 
        }

        public List<IPlayer> PlayersInTurnOrder => PickPlayerOrderer.PlayersInTurnOrder(Players, StartingPlayer);
        public List<IPlayer> PlayersWithoutTurn => PickPlayerOrderer.PlayersWithoutTurn(PlayersInTurnOrder, CardsByPlayer.Keys.ToList());

        private IPlayerOrderer _pickPlayerOrderer;
        public IPlayerOrderer PickPlayerOrderer
        {
            get { return _pickPlayerOrderer ?? (_pickPlayerOrderer = new PlayerOrderer()); }
        }
    }

    public class TrickWinner {
        public IPlayer Player;
        public int Points;
    }

    public interface ITrick
    {
        IHand IHand { get; }
        IGame IGame { get; }
        List<IPlayer> Players { get; }
        IPlayer StartingPlayer { get; }

        Dictionary<IPlayer, SheepCard> CardsByPlayer { get; }
        List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves { get; }

        int QueueRankOfPicker { get; }
        int? QueueRankOfPartner { get; }
        List<IPlayer> PlayersWithoutTurn { get; }
        IPlayerOrderer PickPlayerOrderer { get; }

        void Add(IPlayer player, SheepCard card);
        bool IsLegalAddition(SheepCard card, IPlayer player);
        bool IsComplete();
        TrickWinner Winner();
    }
}