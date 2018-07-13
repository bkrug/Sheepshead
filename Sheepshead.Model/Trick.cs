using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;


namespace Sheepshead.Models
{
    public class Trick : ITrick
    {
        private Dictionary<IPlayer, SheepCard> _cards = new Dictionary<IPlayer, SheepCard>();
        private IHand _hand;

        public IHand Hand { get { return _hand; } }
        public IGame Game { get { return _hand.Deck.Game; } }
        public IPlayer StartingPlayer { get; private set; }
        public Dictionary<IPlayer, SheepCard> CardsPlayed { get { return new Dictionary<IPlayer, SheepCard>(_cards); } }
        public event EventHandler<EventArgs> OnTrickEnd;
        public event EventHandler<MoveEventArgs> OnMove;

        public List<KeyValuePair<IPlayer, SheepCard>> OrderedMoves 
        { 
            get 
            {
                var indexOfStartingPlayer = Players.IndexOf(StartingPlayer);
                var playerList = Players.Skip(indexOfStartingPlayer).Union(Players.Take(indexOfStartingPlayer)).ToList();
                var orderedMoves = new List<KeyValuePair<IPlayer, SheepCard>>();
                foreach (var player in playerList)
                    if (_cards.ContainsKey(player))
                        orderedMoves.Add(new KeyValuePair<IPlayer, SheepCard>( player, _cards[player] ));
                return orderedMoves;
            } 
        }

        public Trick(IHand hand) : this (hand, new StartingPlayerCalculator())
        {
        }

        public Trick(IHand hand, IStartingPlayerCalculator startingPlayerCalculator)
        {
            _hand = hand;
            _hand.AddTrick(this);
            StartingPlayer = startingPlayerCalculator.GetStartingPlayer(hand, this);
        }

        public void Add(IPlayer player, SheepCard card)
        {
            _cards.Add(player, card);
            player.Cards.Remove(card);
            if (_hand.PartnerCard == card)
                _hand.SetPartner(player, this);
            OnMoveHandler(player, card);
            if (IsComplete())
                OnTrickEndHandler();
        }

        public bool IsLegalAddition(SheepCard card, IPlayer player)
        {
            if (!_cards.Any())
            {
                if (Hand.Deck.Game.PartnerMethod == PartnerMethod.JackOfDiamonds)
                    return true;
                //Picker cannot lead with last card of Called Ace's suit
                var suitOfAce = CardUtil.GetSuit(Hand.PartnerCard.Value);
                if (Hand.PartnerCard != null 
                    && player == Hand.Picker
                    && CardUtil.GetSuit(card) == suitOfAce
                    && player.Cards.Union(Hand.Deck.Buried).ToList().Count(c => CardUtil.GetSuit(c) == suitOfAce) == 1)
                        return false;
                return true;
            }
            var firstCard = _cards.First().Value;
            return player.Cards.Contains(card) 
                && (CardUtil.GetSuit(card) == CardUtil.GetSuit(firstCard) || !player.Cards.Any(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(firstCard)));
        }

        public TrickWinner Winner()
        {
            if (!_cards.Any())
                return null;
            var firstSuite = CardUtil.GetSuit(_cards.First().Value);
            var validCards = new List<KeyValuePair<IPlayer, SheepCard>>();
            foreach(var keyValuePair in _cards) {
                var suite = CardUtil.GetSuit(keyValuePair.Value);
                if (suite == firstSuite || suite == Suit.TRUMP)
                    validCards.Add(keyValuePair);
            }
            return new TrickWinner()
            {
                Player = validCards.OrderBy(kvp => CardUtil.GetRank(kvp.Value)).First().Key,
                Points = _cards.Sum(c => CardUtil.GetPoints(c.Value))
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

        public bool IsComplete()
        {
            return CardsPlayed.Count() == Hand.PlayerCount;
        }

        public int PlayerCount
        {
            get { return Hand.Deck.Game.PlayerCount; }
        }

        public List<IPlayer> Players
        {
            get { return Hand.Players; }
        }

        public int QueueRankOfPicker
        {
            get { return Hand.Picker.QueueRankInTrick(this); }
        }

        public int? QueueRankOfPartner
        {
            get { return Hand.Partner == null ? (int?)null : Hand.Partner.QueueRankInTrick(this); } 
        }

        public int IndexInHand
        {
            get { return Hand.Tricks.IndexOf(this); }
        }

        public SheepCard? PartnerCard { get { return Hand.PartnerCard; } }

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
        IHand Hand { get; }
        IGame Game { get; }
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