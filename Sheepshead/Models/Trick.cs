using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Trick
    {
        private Dictionary<IPlayer, Card> _cards = new Dictionary<IPlayer, Card>();

        public void Add(IPlayer player, Card card)
        {
            _cards.Add(player, card);
        }

        public bool IsLegalAddition(Card card, IPlayer player)
        {
            var hand = player.Hand;
            if (!_cards.Any())
                return true;
            var firstCard = _cards.First().Value;
            return hand.Contains(card) 
                && (CardRepository.GetSuite(card) == CardRepository.GetSuite(firstCard) || !hand.Any(c => CardRepository.GetSuite(c) == CardRepository.GetSuite(firstCard)));
        }

        public IPlayer Winner()
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
            return validCards.OrderBy(kvp => kvp.Value.Rank).First().Key;
        }
    }
}