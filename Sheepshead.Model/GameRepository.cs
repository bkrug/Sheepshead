using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    public class GameDictionary
    {
        private GameDictionary() { }
        public static GameDictionary Instance { get; } = new GameDictionary();
        public IDictionary<Guid, IGame> Dictionary { get; } = new SelfClearingDictionary();
    }

    public class GameRepository : BaseRepository<IGame>, IGameRepository
    {
        public GameRepository(IDictionary<Guid, IGame> gameList) : base(gameList)
        {
        }

        public IGame Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, PartnerMethod partnerMethod, bool leastersGame)
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < humanCount; ++i)
                playerList.Add(new HumanPlayer());
            for (var i = 0; i < simpleCount; ++i)
                playerList.Add(new SimplePlayer());
            for (var i = 0; i < intermediateCount; ++i)
                playerList.Add(new IntermediatePlayer());
            for (var i = 0; i < advancedCount; ++i)
                playerList.Add(new AdvancedPlayer());
            var newGame = new Game(playerList, partnerMethod, leastersGame);
            Save(newGame);
            newGame.RearrangePlayers();
            return newGame;
        }

        public IGame GetGame(Func<IGame, bool> lambda)
        {
            return Items.Select(l => l.Value).ToList().FirstOrDefault(lambda);
        }
    }

    public interface IGameRepository : IBaseRepository<IGame>
    {
        IGame Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, PartnerMethod partnerMethod, bool leastersGame);
        IGame GetGame(Func<IGame, bool> lambda);
    }
}