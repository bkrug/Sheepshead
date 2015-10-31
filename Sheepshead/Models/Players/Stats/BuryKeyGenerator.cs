using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models.Players.Stats
{
    public interface IBuryKeyGenerator
    {
        BuryStatUniqueKey GenerateKey(IDeck deck);
    }

    public class BuryKeyGenerator : IBuryKeyGenerator
    {
        public BuryStatUniqueKey GenerateKey(List<ICard> cardsHeld, List<ICard> buried)
        {
            return new BuryStatUniqueKey()
            {
                BuriedPoints = buried.Sum(c => c.Points),
                AvgRankInHand = (int)Math.Round(cardsHeld.Average(c => c.Rank)),
                PointsInHand = cardsHeld.Sum(c => c.Points),
                SuitsInHand = cardsHeld.GroupBy(c => c.StandardSuite).Count()
            };
        }

        public BuryStatUniqueKey GenerateKey(IDeck deck)
        {
            var cardsHeld = deck.Hand.Picker.Cards.Union(
                deck.Hand.Tricks.Select(t => t.OrderedMoves.Single(kvp => kvp.Key == deck.Hand.Picker).Value))
                .ToList();
            return GenerateKey(cardsHeld, deck.Buried);
        }
    }
}