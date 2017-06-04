using System;
using System.Collections.Generic;
using System.Linq;

using System.Linq.Expressions;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players.Stats;

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

        public IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd, ILearningHelperFactory factory)
        {
            var game = new Game(0, players, rnd, factory, new HandFactory());
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
        IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd, ILearningHelperFactory factory);
        IGame GetGame(Func<IGame, bool> lambda);
        //IEnumerable<IGame> GetOpenGames();
    }
}