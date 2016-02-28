using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public class Trick : ITrick
    {
        private Dictionary<IPlayer, ICard> _cards = new Dictionary<IPlayer, ICard>();
        private IHand _hand;

        public IHand Hand { get { return _hand; } }
        public IGame Game { get { return _hand.Deck.Game; } }
        public IPlayer StartingPlayer { get; private set; }
        public Dictionary<IPlayer, ICard> CardsPlayed { get { return new Dictionary<IPlayer, ICard>(_cards); } }
        public event EventHandler<EventArgs> OnTrickEnd;
        public event EventHandler<MoveEventArgs> OnMove;

        public List<KeyValuePair<IPlayer, ICard>> OrderedMoves 
        { 
            get 
            {
                var indexOfStartingPlayer = Players.IndexOf(StartingPlayer);
                var playerList = Players.Skip(indexOfStartingPlayer).Union(Players.Take(indexOfStartingPlayer)).ToList();
                var orderedMoves = new List<KeyValuePair<IPlayer, ICard>>();
                foreach (var player in playerList)
                    if (_cards.ContainsKey(player))
                        orderedMoves.Add(new KeyValuePair<IPlayer, ICard>( player, _cards[player] ));
                return orderedMoves;
            } 
        }

        private Trick()
        {
        }

        public Trick(IHand hand)
        {
            _hand = hand;
            _hand.AddTrick(this);
            SetStartingPlayer();
        }

        private void SetStartingPlayer()
        {
            var index = Hand.Tricks.IndexOf(this);
            StartingPlayer = (index == 0)
                ? Hand.StartingPlayer
                : Hand.Tricks[index - 1].Winner().Player;
        }

        public void Add(IPlayer player, ICard card)
        {
            _cards.Add(player, card);
            player.Cards.Remove(card);
            if (_hand.PartnerCard != null && _hand.PartnerCard.StandardSuite == card.StandardSuite && _hand.PartnerCard.CardType == card.CardType)
                _hand.SetPartner(player, this);
            OnMoveHandler(player, card);
            if (IsComplete())
                OnTrickEndHandler();
        }

        public bool IsLegalAddition(ICard card, IPlayer player)
        {
            var hand = player.Cards;
            if (!_cards.Any())
                return true;
            var firstCard = _cards.First().Value;
            return hand.Contains(card) 
                && (CardRepository.GetSuit(card) == CardRepository.GetSuit(firstCard) || !hand.Any(c => CardRepository.GetSuit(c) == CardRepository.GetSuit(firstCard)));
        }

        public TrickWinner Winner()
        {
            if (!_cards.Any())
                return null;
            var firstSuite = CardRepository.GetSuit(_cards.First().Value);
            var validCards = new List<KeyValuePair<IPlayer, ICard>>();
            foreach(var keyValuePair in _cards) {
                var suite = CardRepository.GetSuit(keyValuePair.Value);
                if (suite == firstSuite || suite == Suit.TRUMP)
                    validCards.Add(keyValuePair);
            }
            return new TrickWinner()
            {
                Player = validCards.OrderBy(kvp => kvp.Value.Rank).First().Key,
                Points = _cards.Sum(c => c.Value.Points)
            };
        }

        public class MoveEventArgs : EventArgs
        {
            public IPlayer Player;
            public ICard Card;
        }

        protected virtual void OnMoveHandler(IPlayer player, ICard card)
        {
            var e = new MoveEventArgs()
            {
                Player = player,
                Card = card
            };
            if (OnMove != null)
                OnMove(this, e);
        }

        protected virtual void OnTrickEndHandler()
        {
            var e = new EventArgs();
            if (OnTrickEnd != null)
                OnTrickEnd(this, e);
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

        public ICard PartnerCard { get { return Hand.PartnerCard; } }

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
        void Add(IPlayer player, ICard card);
        bool IsLegalAddition(ICard card, IPlayer player);
        IPlayer StartingPlayer { get; }
        Dictionary<IPlayer, ICard> CardsPlayed { get; }
        bool IsComplete();
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        int QueueRankOfPicker { get; }
        int? QueueRankOfPartner { get; }
        int IndexInHand { get; }
        ICard PartnerCard { get; }
        List<KeyValuePair<IPlayer, ICard>> OrderedMoves { get; }
        event EventHandler<EventArgs> OnTrickEnd;
        List<IPlayer> PlayersWithoutTurn { get; }
        IPlayerOrderer PickPlayerOrderer { get; }
    }
}