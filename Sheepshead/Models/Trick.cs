using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Trick
    {
        private Dictionary<IPlayer, Card> _cards = new Dictionary<IPlayer, Card>();
        private IHand _hand;

        public Trick(IHand hand)
        {
            _hand = hand;
        }

        public void Add(IPlayer player, Card card)
        {
            _cards.Add(player, card);
            player.Cards.Remove(card);
        }

        public bool IsLegalAddition(Card card, IPlayer player)
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
            var validCards = new List<KeyValuePair<IPlayer, Card>>();
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
}