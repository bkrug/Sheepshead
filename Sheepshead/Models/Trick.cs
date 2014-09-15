using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Trick
    {
        private List<Card> _cards = new List<Card>();
        public List<Card> Cards { get { return _cards.ToList(); } }

        public void Add(Card card)
        {
            _cards.Add(card);
        }

        public bool IsLegalAddition(Card card, List<Card> hand)
        {
            if (!_cards.Any())
                return true;
            var firstCard = _cards.First();
            return hand.Contains(card) 
                && (CardRepository.GetSuite(card) == CardRepository.GetSuite(firstCard) || !hand.Any(c => CardRepository.GetSuite(c) == CardRepository.GetSuite(firstCard)));
        }
    }
}