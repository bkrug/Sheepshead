using Sheepshead.Models;
using Sheepshead.Models.Players;

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
        bool HaveHighPointsBeenPlayed(IPlayer thisPlayer, ITrick trick);
    }

    public class LeasterStateAnalyzer : ILeasterStateAnalyzer
    {
        public bool CanILoose(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool CanIWin(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool EarlyInTrick(ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool HaveAnyPowerCards(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool HaveHighPointsBeenPlayed(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool HaveIAlreadyWon(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool HaveTwoPowerCards(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }
    }
}
