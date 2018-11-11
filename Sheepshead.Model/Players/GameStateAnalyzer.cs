using System.Collections.Generic;

namespace Sheepshead.Models.Players
{
    public interface IGameStateAnalyzer
    {
        bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick);
        bool MySideWinning(IPlayer thisPlayer, ITrick trick);
        List<SheepCard> MyCardsThatCanWin(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsCouldWin(List<SheepCard> myStrongCards, ITrick trick);
    }

    public class GameStateAnalyzer : IGameStateAnalyzer
    {
        public bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public List<SheepCard> MyCardsThatCanWin(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool MySideWinning(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool UnplayedCardsCouldWin(List<SheepCard> myStrongCards, ITrick trick)
        {
            throw new System.NotImplementedException();
        }
    }
}