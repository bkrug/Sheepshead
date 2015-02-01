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
        private IMoveStatRepository _moveStatRepository;
        private Dictionary<IPlayer, MoveStatUniqueKey> _learningKeys = new Dictionary<IPlayer,MoveStatUniqueKey>();

        public IHand Hand { get { return _hand; } }
        public IPlayer StartingPlayer { get; private set; }
        public Dictionary<IPlayer, ICard> CardsPlayed { get { return new Dictionary<IPlayer, ICard>(_cards); } }

        public Trick(IHand hand, IMoveStatRepository moveStatRepository)
        {
            _moveStatRepository = moveStatRepository;
            _hand = hand;
            _hand.AddTrick(this);
            SetStartingPlayer();
        }

        private void SetStartingPlayer()
        {
            var index = Hand.Tricks.IndexOf(this);
            StartingPlayer = (index == 0)
                ? Hand.Deck.StartingPlayer
                : Hand.Tricks[index - 1].Winner().Player;
        }

        public void Add(IPlayer player, ICard card)
        {
            _learningKeys.Add(player, GenerateKey(player, card));
            _cards.Add(player, card);
            player.Cards.Remove(card);
            if (_hand.PartnerCard != null && _hand.PartnerCard.StandardSuite == card.StandardSuite && _hand.PartnerCard.CardType == card.CardType)
                _hand.Partner = player;
            if (IsComplete())
                EndTrick();
        }

        public bool IsLegalAddition(ICard card, IPlayer player)
        {
            var hand = player.Cards;
            if (!_cards.Any())
                return true;
            var firstCard = _cards.First().Value;
            return hand.Contains(card) 
                && (CardRepository.GetSuite(card) == CardRepository.GetSuite(firstCard) || !hand.Any(c => CardRepository.GetSuite(c) == CardRepository.GetSuite(firstCard)));
        }

        public TrickWinner Winner()
        {
            if (!_cards.Any())
                return null;
            var firstSuite = CardRepository.GetSuite(_cards.First().Value);
            var validCards = new List<KeyValuePair<IPlayer, ICard>>();
            foreach(var keyValuePair in _cards) {
                var suite = CardRepository.GetSuite(keyValuePair.Value);
                if (suite == firstSuite || suite == Suite.TRUMP)
                    validCards.Add(keyValuePair);
            }
            return new TrickWinner()
            {
                Player = validCards.OrderBy(kvp => kvp.Value.Rank).First().Key,
                Points = _cards.Sum(c => c.Value.Points)
            };
        }

        public bool IsComplete()
        {
            return CardsPlayed.Count() == Hand.Deck.Game.PlayerCount;
        }

        public MoveStatUniqueKey GenerateKey(IPlayer player,ICard legalCard)
        {
            List<IPlayer> playerList = this.Hand.Deck.Game.Players;
            return new MoveStatUniqueKey()
            {
                Picker = playerList.IndexOf(this.Hand.Picker),
                Partner = this.Hand.Partner != null ? (int?)playerList.IndexOf(this.Hand.Partner) : null,
                Trick = this.Hand.Tricks.Count(),
                MoveWithinTrick = QueueRankInTrick(player),
                PointsAlreadyInTrick = this.CardsPlayed.Sum(c => c.Value.Points),
                TotalPointsInPreviousTricks = this.Hand.Tricks.Where(t => t != this).Sum(t => t.CardsPlayed.Sum(c => c.Value.Points)),
                PointsInThisCard = legalCard.Points,
                RankOfThisCard = legalCard.Rank,
                PartnerCard = this.Hand.PartnerCard == legalCard,
                HigherRankingCardsPlayedPreviousTricks = this.Hand.Tricks.Where(t => t != this).SelectMany(t => t.CardsPlayed.Select(kvp => kvp.Value)).Count(c => c.Rank > legalCard.Rank),
                HigherRankingCardsPlayedThisTrick = this.CardsPlayed.Select(kvp => kvp.Value).Count(c => c.Rank > legalCard.Rank)
            };
        }

        //TODO: Duplicate logic
        protected int QueueRankInTrick(IPlayer player)
        {
            var indexOfMe = this.Hand.Deck.Game.Players.IndexOf(player);
            var indexOfStartingPlayer = this.Hand.Deck.Game.Players.IndexOf(this.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += this.Hand.Deck.Game.PlayerCount;
            return rank + 1;
        }

        //TODO: Duplicate logic
        protected int QueueRankInDeck(IPlayer player)
        {
            var indexOfMe = this.Hand.Deck.Game.Players.IndexOf(player);
            var indexOfStartingPlayer = this.Hand.Deck.Game.Players.IndexOf(player);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += this.Hand.Deck.Game.PlayerCount;
            return rank + 1;
        }

        private void EndTrick()
        {
            foreach (var player in this.Hand.Deck.Game.Players)
            {
                var statKey = _learningKeys[player];
                var repository = MoveStatRepository.Instance;
                repository.IncrementTrickResult(statKey, Winner().Player == this);
            }
            if (Hand.IsComplete())
                Hand.EndHand();
        }

        public void OnHandEnd()
        {
            foreach (var player in this.Hand.Deck.Game.Players)
            {
                var statKey = _learningKeys[player];
                var repository = MoveStatRepository.Instance;
                repository.IncrementHandResult(statKey, Hand.Scores()[player] > 0);
            }
        }
    }

    public class TrickWinner {
        public IPlayer Player;
        public int Points;
    }

    public interface ITrick
    {
        IHand Hand { get; }
        TrickWinner Winner();
        void Add(IPlayer player, ICard card);
        bool IsLegalAddition(ICard card, IPlayer player);
        IPlayer StartingPlayer { get; }
        Dictionary<IPlayer, ICard> CardsPlayed { get; }
        bool IsComplete();
        MoveStatUniqueKey GenerateKey(IPlayer player, ICard legalCard);
        void OnHandEnd();
    }
}