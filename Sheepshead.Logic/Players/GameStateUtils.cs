using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;

namespace Sheepshead.Logic.Players
{
    public static class GameStateUtils
    {
        public static IEnumerable<SheepCard> GetCardsThatCouldWin(ITrick trick, IEnumerable<SheepCard> comparisonCards)
        {
            if (!trick.CardsPlayed.Any())
                return comparisonCards;
            var startSuit = CardUtil.GetSuit(trick.CardsPlayed.First().Value);
            var winningCard = GameStateUtils.GetWinningPlay(trick).Value;
            var winningCardRank = CardUtil.GetRank(winningCard);
            if (CardUtil.GetSuit(winningCard) == Suit.TRUMP)
                return comparisonCards.Where(c => CardUtil.GetRank(c) < winningCardRank);
            else
                return comparisonCards.Where(c =>
                    CardUtil.GetSuit(c) == Suit.TRUMP
                    || CardUtil.GetSuit(c) == startSuit && CardUtil.GetRank(c) < winningCardRank
                );
        }

        public static KeyValuePair<IPlayer, SheepCard> GetWinningPlay(ITrick trick)
        {
            var startSuit = CardUtil.GetSuit(trick.CardsPlayed.First().Value);
            var winningPlay = trick.CardsPlayed
                .OrderBy(cp => CardUtil.GetSuit(cp.Value) == Suit.TRUMP ? 1 : 2)
                .ThenBy(cp => CardUtil.GetSuit(cp.Value) == startSuit ? 1 : 2)
                .ThenBy(cp => CardUtil.GetRank(cp.Value))
                .First();
            return winningPlay;
        }
    }
}