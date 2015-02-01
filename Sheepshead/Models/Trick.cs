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
        private Dictionary<IPlayer, MoveStatUniqueKey> _learningKeys = new Dictionary<IPlayer,MoveStatUniqueKey>();
        private ILearningHelper _learningHelper;

        public IHand Hand { get { return _hand; } }
        public IGame Game { get { return _hand.Deck.Game; } }
        public IPlayer StartingPlayer { get; private set; }
        public Dictionary<IPlayer, ICard> CardsPlayed { get { return new Dictionary<IPlayer, ICard>(_cards); } }
        public Dictionary<IPlayer, MoveStatUniqueKey> LearningKeys { get { return _learningKeys; } }

        private Trick()
        {
        }

        public Trick(IHand hand, ILearningHelper learningHelper)
        {
            _learningHelper = learningHelper;
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
            _learningKeys.Add(player, _learningHelper.GenerateKey(this, player, card));
            _cards.Add(player, card);
            player.Cards.Remove(card);
            if (_hand.PartnerCard != null && _hand.PartnerCard.StandardSuite == card.StandardSuite && _hand.PartnerCard.CardType == card.CardType)
                _hand.Partner = player;
            if (IsComplete())
                _learningHelper.EndTrick(this);
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
            return CardsPlayed.Count() == Hand.PlayerCount;
        }

        public MoveStatUniqueKey GenerateKey(IPlayer player, ICard legalCard)
        {
            return _learningHelper.GenerateKey(this, player, legalCard);
        }

        public void OnHandEnd()
        {
            _learningHelper.OnHandEnd(this);
        }

        public int PlayerCount
        {
            get { return Hand.Deck.Game.PlayerCount; }
        }

        public List<IPlayer> Players
        {
            get { return Hand.Deck.Game.Players; }
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
    }

    public class TrickWinner {
        public IPlayer Player;
        public int Points;
    }

    public interface ITrick
    {
        Dictionary<IPlayer, MoveStatUniqueKey> LearningKeys { get; }
        IHand Hand { get; }
        IGame Game { get; }
        TrickWinner Winner();
        void Add(IPlayer player, ICard card);
        bool IsLegalAddition(ICard card, IPlayer player);
        IPlayer StartingPlayer { get; }
        Dictionary<IPlayer, ICard> CardsPlayed { get; }
        bool IsComplete();
        MoveStatUniqueKey GenerateKey(IPlayer player, ICard legalCard);
        void OnHandEnd();
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        int QueueRankOfPicker { get; }
        int? QueueRankOfPartner { get; }
        int IndexInHand { get; }
        ICard PartnerCard { get; }
    }
}