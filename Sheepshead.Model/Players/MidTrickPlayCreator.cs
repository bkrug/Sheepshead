using System.Linq;

namespace Sheepshead.Models.Players
{
    public interface IMidTrickPlayCreator
    {
        SheepCard GiveAwayLeastPower(IPlayer thisPlayer, ITrick trick);
        SheepCard GiveAwayPoints(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayWeakestWin(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayStrongestWin(IPlayer thisPlayer, ITrick trick);
    }

    public class MidTrickPlayCreator : IMidTrickPlayCreator
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
    }
}