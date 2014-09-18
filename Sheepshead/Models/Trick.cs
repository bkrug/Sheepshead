using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Trick : ITrick
    {
        private Dictionary<IPlayer, ICard> _cards = new Dictionary<IPlayer, ICard>();
        private IHand _hand;

        public Trick(IHand hand)
        {
            _hand = hand;
            _hand.AddTrick(this);
        }

        public void Add(IPlayer player, ICard card)
        {
            _cards.Add(player, card);
            player.Cards.Remove(card);
            if (_hand.PartnerCard.StandardSuite == card.StandardSuite && _hand.PartnerCard.CardType == card.CardType)
                _hand.Partner = player;
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
    }

    public class TrickWinner {
        public IPlayer Player;
        public int Points;
    }

    public interface ITrick
    {
        TrickWinner Winner();
        void Add(IPlayer player, ICard card);
        bool IsLegalAddition(ICard card, IPlayer player);
    }
}