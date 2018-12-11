using System.Linq;
using Sheepshead.Model.Models;

namespace Sheepshead.Model.Players
{
    public interface IPlayCreator
    {
        SheepCard GiveAwayLeastPower(IPlayer thisPlayer, ITrick trick);
        SheepCard GiveAwayPoints(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayWeakestWin(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayStrongestWin(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayStrongestLoosingCard(IPlayer thisPlayer, ITrick trick);
        SheepCard PlaySecondStrongestLoosingCard(IPlayer thisPlayer, ITrick trick);
    }

    public class PlayCreator : IPlayCreator
    {
        public SheepCard GiveAwayLeastPower(IPlayer thisPlayer, ITrick trick)
        {
            return thisPlayer.Cards
                .Where(c => trick.IsLegalAddition(c, thisPlayer))
                .OrderByDescending(c => CardUtil.GetRank(c))
                .First();
        }

        public SheepCard GiveAwayPoints(IPlayer thisPlayer, ITrick trick)
        {
            return thisPlayer.Cards
                .Where(c => trick.IsLegalAddition(c, thisPlayer))
                .OrderByDescending(c => CardUtil.GetPoints(c))
                .First();
        }

        public SheepCard PlayWeakestWin(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer));
            var winnableCards = GameStateUtils.GetCardsThatCouldWin(trick, legalCards);
            return winnableCards.OrderByDescending(c => CardUtil.GetRank(c)).First();
        }

        public SheepCard PlayStrongestWin(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer));
            var winnableCards = GameStateUtils.GetCardsThatCouldWin(trick, legalCards);
            return winnableCards.OrderBy(c => CardUtil.GetRank(c)).First();
        }

        public SheepCard PlayStrongestLoosingCard(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer));
            var winnableCards = GameStateUtils.GetCardsThatCouldWin(trick, legalCards);
            return legalCards.Except(winnableCards).OrderBy(c => CardUtil.GetRank(c)).First();
        }

        public SheepCard PlaySecondStrongestLoosingCard(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer));
            var winnableCards = GameStateUtils.GetCardsThatCouldWin(trick, legalCards);
            var cards = legalCards.Except(winnableCards).OrderBy(c => CardUtil.GetRank(c)).ToList();
            if (cards.Count == 1)
                return cards.Single();
            else
                return cards[1];
        }
    }
}