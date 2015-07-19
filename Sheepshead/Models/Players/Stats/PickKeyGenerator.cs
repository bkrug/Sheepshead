using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models.Players.Stats
{
    public interface IPickKeyGenerator
    {
        PickStatUniqueKey GenerateKey(IHand hand, IPlayer player);
    }

    public class PickKeyGenerator : IPickKeyGenerator
    {
        public PickStatUniqueKey GenerateKey(IHand hand, IPlayer player)
        {
            var cardsPlayed = hand.Tricks == null
                ? new List<ICard>()
                : hand.Tricks.Select(t => t.CardsPlayed[player]).Where(c => c != null);
            var cardsBuried = hand.Deck.Buried == null ? new List<ICard>() : hand.Deck.Buried;
            var blinds = hand.Deck.Blinds == null ? new List<ICard>() : hand.Deck.Blinds;
            var cards = player.Cards
                .Union(cardsPlayed)
                .Union(cardsBuried)
                .Where(c => !blinds.Contains(c))
                .ToList();
            var trumpCards = cards.Where(c => CardRepository.GetSuit(c) == Suit.TRUMP).ToList();
            var trumpCount = trumpCards.Count();
            var trumpRankAvg = trumpCards.Any() ? trumpCards.Average(c => c.Rank) : 0;
            return new PickStatUniqueKey()
            {
                TrumpCount = trumpCount,
                AvgTrumpRank = (int)Math.Round(trumpRankAvg),
                //TrumpStdDeviation = trumpCards.Any() ? (int)Math.Round(Math.Sqrt(trumpCards.Sum(c => Math.Pow(c.Rank - trumpRankAvg, 2)) / trumpCount)) : 0,
                PointsInHand = cards.Sum(c => c.Points),
                TotalCardsWithPoints = cards.Count(c => c.Points > 0)
            };
        }
    }
}