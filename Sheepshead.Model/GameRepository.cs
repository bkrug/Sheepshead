using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public class GameDictionary
    {
        private GameDictionary() { }
        public static GameDictionary Instance { get; } = new GameDictionary();
        public Dictionary<Guid, IGame> Dictionary { get; } = new Dictionary<Guid, IGame>();
    }

    public class GameRepository : BaseRepository<IGame>, IGameRepository
    {
        public GameRepository(Dictionary<Guid, IGame> gameList) : base(gameList)
        {
        }

        public IGame Create(int humanCount, int newbieCount, int basicCount)
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < humanCount; ++i)
                playerList.Add(new HumanPlayer());
            for (var i = 0; i < newbieCount; ++i)
                playerList.Add(new NewbiePlayer());
            for (var i = 0; i < basicCount; ++i)
                playerList.Add(new BasicPlayer());
            var newGame = new Game(playerList);
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
        IGame Create(int humanCount, int newbieCount, int basicCount);
        IGame GetGame(Func<IGame, bool> lambda);
    }
}