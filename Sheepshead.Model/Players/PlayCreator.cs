using System.Linq;

namespace Sheepshead.Models.Players
{
    public interface IPlayCreator
    {
        SheepCard GiveAwayLeastPower(IPlayer thisPlayer, ITrick trick);
        SheepCard GiveAwayPoints(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayWeakestWin(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayStrongestWin(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayStrongestLoosingCard(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayToWin(IPlayer thisPlayer, ITrick trick);
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

        public SheepCard PlayToWin(IPlayer thisPlayer, ITrick trick)
        {
            if (trick.CardsPlayed.Count == trick.Hand.Deck.Game.PlayerCount - 1)
                return PlayWeakestWin(thisPlayer, trick);
            else
                return PlayStrongestWin(thisPlayer, trick);
        }
    }
}