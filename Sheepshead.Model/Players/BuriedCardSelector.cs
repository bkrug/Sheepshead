using Sheepshead.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheepshead.Models.Players
{
    public class BuriedCardSelector
    {
        public Dictionary<Suit, int> CardsPerSuit { get; }
        private List<SheepCard> _cards;
        private List<SheepCard> _acesAndTens;

        public BuriedCardSelector(List<SheepCard> cards)
        {
            CardsPerSuit = cards.GroupBy(c => CardUtil.GetSuit(c)).ToDictionary(g => g.Key, g => g.Count());
            _cards = cards;
            _acesAndTens = cards
                .Where(c => new[] { CardType.ACE, CardType.N10 }.Contains(CardUtil.GetFace(c)))
                .ToList();
        }

        public List<SheepCard> GetTwoFailAceOrTens()
        {
            var AcesAndTensPerSuit = _acesAndTens
                .GroupBy(c => CardUtil.GetSuit(c))
                .Where(g => g.Key != Suit.TRUMP)
                .OrderBy(g => g.Count())
                .ToList();

            if (_acesAndTens.Count < 2)
                return new List<SheepCard>();

            var oneCardSuits = AcesAndTensPerSuit
                .Where(g => g.Count() == 1)
                .Where(g => CardsPerSuit[g.Key] == 1)
                .ToList();
            if (oneCardSuits.Count >= 2)
                return oneCardSuits.SelectMany(g => g).Take(2).ToList();

            var twoCardSuits = AcesAndTensPerSuit
                .Where(g => g.Count() == 2)
                .Where(g => CardsPerSuit[g.Key] == 2)
                .ToList();
            if (twoCardSuits.Any())
                return twoCardSuits.First().Select(g => g).ToList();

            var orderedCards = AcesAndTensPerSuit
                .OrderBy(g => CardsPerSuit[g.Key])
                .SelectMany(g => g)
                .ToList();
            return orderedCards.Take(2).ToList();
        }

        public List<SheepCard> RetireTwoFailSuitsWithOneAceOrTen()
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> RetireOneFailSuitsWithOneAceOrTen()
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> RetireTwoFailSuitsTwoCards()
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> RetireOneFailSuitTwoCards()
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> BuryCardsByEasiestToRetireLowestRank()
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> BuryCardsByLowestRank()
        {
            throw new NotImplementedException();
        }
    }
}
