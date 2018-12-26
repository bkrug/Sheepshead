using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sheepshead.Model;
using Sheepshead.Model.Players;

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
        public virtual Dictionary<IPlayer, SheepCard> CardsPlayed {
            get {
                if (TrickPlay == null)
                    throw new NullReferenceException("TrickPlays is null");
                if (TrickPlay.Any(tp => tp.Participant == null))
                    throw new NullReferenceException("Participant is null");
                return TrickPlay.Any() 
                    ? TrickPlay.ToDictionary(tp => tp.Participant.Player, tp => CardUtil.GetCardFromAbbreviation(tp.Card).Value) 
                    : new Dictionary<IPlayer, SheepCard>();
            }
        }
        public event EventHandler<EventArgs> OnTrickEnd;
        public event EventHandler<MoveEventArgs> OnMove;

        public List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves 
        { 
            get 
            {
                var indexOfStartingPlayer = Players.IndexOf(StartingPlayer);
                var playerList = Players.Skip(indexOfStartingPlayer).Union(Players.Take(indexOfStartingPlayer)).ToList();
                var orderedMoves = new List<KeyValuePair<IPlayer, SheepCard>>();
                var cards = CardsPlayed;
                foreach (var player in playerList)
                    if (cards.ContainsKey(player))
                        orderedMoves.Add(new KeyValuePair<IPlayer, SheepCard>( player, cards[player] ));
                return orderedMoves;
            } 
        }

        public Trick(IHand hand) : this (hand, new StartingPlayerCalculator())
        {
        }

        public Trick(IHand hand, IStartingPlayerCalculator startingPlayerCalculator)
        {
            IHand = hand;
            IHand.AddTrick(this);
            StartingPlayer = startingPlayerCalculator.GetStartingPlayer(hand, this);
            if (TrickPlay == null)
                TrickPlay = new List<TrickPlay>();
        }

        public void Add(IPlayer player, SheepCard card)
        {
            TrickPlay.Add(new TrickPlay()
            {
                Trick = this,
                Participant = player.Participant,
                Card = CardUtil.GetAbbreviation(card),
                SortOrder = TrickPlay.Count() + 1
            });
            player.RemoveCard(card);
            if (IHand.PartnerCardEnum == card)
                IHand.SetPartner(player, this);
            OnMoveHandler(player, card);
            if (IsComplete())
                OnTrickEndHandler();
        }

        public bool IsLegalAddition(SheepCard card, IPlayer player)
        {
            //In the last trick, anything is legal.
            if (player.Cards.Count() == 1)
                return true;

            var cards = CardsPlayed;
            //There are some rules for the lead card in a trick.
            if (!cards.Any())
                return IHand.IGame.PartnerMethodEnum == PartnerMethod.JackOfDiamonds 
                    || IHand.PartnerCardEnum == null
                    || IsLegalStartingCardInCalledAceGame(card, player);

            //Other cards must follow suit.
            var firstCard = cards.First().Value;
            return player.Cards.Contains(card) 
                && (CardUtil.GetSuit(card) == CardUtil.GetSuit(firstCard) || !player.Cards.Any(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(firstCard)));
        }

        private bool IsLegalStartingCardInCalledAceGame(SheepCard card, IPlayer player)
        {
            var suitOfPartnerCard = CardUtil.GetSuit(IHand.PartnerCardEnum.Value);
            //Once suit of partner card is lead, picker and partner may lead with that suit.
            if (IHand.ITricks != null
                && IHand.ITricks.Any(t => t != this && t.CardsPlayed.Any() && CardUtil.GetSuit(t.CardsPlayed.First().Value) == suitOfPartnerCard))
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
            var cards = CardsPlayed;
            if (!cards.Any())
                return null;
            var firstSuite = CardUtil.GetSuit(cards.First().Value);
            var validCards = new List<KeyValuePair<IPlayer, SheepCard>>();
            foreach(var keyValuePair in cards) {
                var suite = CardUtil.GetSuit(keyValuePair.Value);
                if (suite == firstSuite || suite == Suit.TRUMP)
                    validCards.Add(keyValuePair);
            }
            return new TrickWinner()
            {
                Player = validCards.OrderBy(kvp => CardUtil.GetRank(kvp.Value)).First().Key,
                Points = cards.Sum(c => CardUtil.GetPoints(c.Value))
            };
        }

        public class MoveEventArgs : EventArgs
        {
            public IPlayer Player;
            public SheepCard Card;
        }

        protected virtual void OnMoveHandler(IPlayer player, SheepCard card)
        {
            var e = new MoveEventArgs()
            {
                Player = player,
                Card = card
            };
            OnMove?.Invoke(this, e);
        }

        protected virtual void OnTrickEndHandler()
        {
            var e = new EventArgs();
            OnTrickEnd?.Invoke(this, e);
        }

        public virtual bool IsComplete()
        {
            return CardsPlayed.Count() == IHand.PlayerCount;
        }

        public int PlayerCount
        {
            get { return IHand.IGame.PlayerCount; }
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

        public int IndexInHand
        {
            get { return IHand.ITricks.IndexOf(this); }
        }

        public SheepCard? PartnerCard { get { return IHand.PartnerCardEnum; } }

        public List<IPlayer> PlayersInTurnOrder => PickPlayerOrderer.PlayersInTurnOrder(Players, StartingPlayer);
        public List<IPlayer> PlayersWithoutTurn => PickPlayerOrderer.PlayersWithoutTurn(PlayersInTurnOrder, CardsPlayed.Keys.ToList());

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
        TrickWinner Winner();
        void Add(IPlayer player, SheepCard card);
        bool IsLegalAddition(SheepCard card, IPlayer player);
        IPlayer StartingPlayer { get; }
        Dictionary<IPlayer, SheepCard> CardsPlayed { get; }
        bool IsComplete();
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        int QueueRankOfPicker { get; }
        int? QueueRankOfPartner { get; }
        int IndexInHand { get; }
        SheepCard? PartnerCard { get; }
        List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves { get; }
        event EventHandler<EventArgs> OnTrickEnd;
        List<IPlayer> PlayersWithoutTurn { get; }
        IPlayerOrderer PickPlayerOrderer { get; }
    }
}