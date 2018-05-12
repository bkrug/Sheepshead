using System;
using System.Collections.Generic;
using System.Linq;

using System.Linq.Expressions;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;


namespace Sheepshead.Models
{
    public class GameDictionary
    {
        private static GameDictionary _instance = new GameDictionary();
        private Dictionary<long, IGame> _dictionary = new Dictionary<long, IGame>();

        private GameDictionary() { }

        public static GameDictionary Instance {
            get { return _instance; }
        }

        public Dictionary<long, IGame> Dictionary { get { return _dictionary; } }
    }

    public class GameRepository : BaseRepository<IGame>, IGameRepository
    {
        public GameRepository(Dictionary<long, IGame> gameList) : base(gameList)
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
            var newGame = new Game(0, playerList);
            Save(newGame);
            newGame.RearrangePlayers();
            return newGame;
        }

        public IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd)
        {
            var game = new Game(0, players, rnd, new HandFactory(), null);
            game.Name = name;
            return game;
        }

        public IGame GetGame(Func<IGame, bool> lambda)
        {
            return Items.Select(l => l.Value).ToList().FirstOrDefault(lambda);
        }

        //public IEnumerable<IGame> GetOpenGames() {
        //    return Items.Where(l => l.Value.HumanPlayerCount < l.Value.MaxHumanPlayers).Select(l => l.Value);
        //}
    }

    public interface IGameRepository : IBaseRepository<IGame>
    {
        IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd);
        IGame GetGame(Func<IGame, bool> lambda);
        //IEnumerable<IGame> GetOpenGames();
    }
}