using Sheepshead.Model.Models;
using System.Linq;

namespace Sheepshead.Model.Players
{
    public interface ILeasterStateAnalyzer
    {
        bool CanIWin(IPlayer thisPlayer, ITrick trick);
        bool CanILoose(IPlayer thisPlayer, ITrick trick);
        bool EarlyInTrick(ITrick trick);
        bool HaveIAlreadyWon(IPlayer thisPlayer, ITrick trick);
        bool HaveAnyPowerCards(IPlayer thisPlayer, ITrick trick);
        bool HaveTwoPowerCards(IPlayer thisPlayer, ITrick trick);
        bool HaveHighPointsBeenPlayed(ITrick trick);
    }

    public class LeasterStateAnalyzer : ILeasterStateAnalyzer
    {
        public bool CanIWin(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer)).ToList();
            return GameStateUtils.GetCardsThatCouldWin(trick, legalCards).Any();
        }

        public bool CanILoose(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer)).ToList();
            var winnableCards = GameStateUtils.GetCardsThatCouldWin(trick, legalCards).ToList();
            var looseableCards = legalCards.Except(winnableCards);
            return looseableCards.Any();
        }

        public bool EarlyInTrick(ITrick trick)
        {
            if (trick.IHand.IGame.PlayerCount == 3)
                return trick.CardsPlayed.Count < 2;
            else
                return trick.CardsPlayed.Count < 3;
        }

        public bool HaveIAlreadyWon(IPlayer thisPlayer, ITrick trick)
        {
            return trick.IHand.Tricks
                .Where(t => t.CardsPlayed.Count == trick.IHand.IGame.PlayerCount)
                .Any(t => t.Winner().Player == thisPlayer);
        }

        public bool HaveAnyPowerCards(IPlayer thisPlayer, ITrick trick)
        {
            return CountQueensJacks(thisPlayer) >= 1;
        }

        public bool HaveTwoPowerCards(IPlayer thisPlayer, ITrick trick)
        {
            return CountQueensJacks(thisPlayer) >= 2;
        }

        private int CountQueensJacks(IPlayer thisPlayer)
        {
            return thisPlayer.Cards.Count(c => new[] { CardType.JACK, CardType.QUEEN }.Contains(CardUtil.GetFace(c)));
        }

        public bool HaveHighPointsBeenPlayed(ITrick trick)
        {
            var totalPoints = trick.CardsPlayed.Sum(cp => CardUtil.GetPoints(cp.Value));
            return trick.IHand.IGame.PlayerCount == 3
                ? totalPoints >= 10
                : totalPoints >= 12;
        }
    }
}
