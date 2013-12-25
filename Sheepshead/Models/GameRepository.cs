using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace Sheepshead.Models
{
    public class GameDictionary
    {
        private static GameDictionary _instance = new GameDictionary();
        private Dictionary<long, Game> _dictionary = new Dictionary<long, Game>();

        private GameDictionary() { }

        public static GameDictionary Instance {
            get { return _instance; }
        }

        public Dictionary<long, Game> Dictionary { get { return _dictionary; } }
    }

    public class GameRepository : BaseRepository<IGame>, IGameRepository
    {
        public GameRepository(Dictionary<long, IGame> gameList) : base(gameList)
        {
        }

        public IGame CreateGame(string name)
        {
            var game = new Game(0);
            return game;
        }

        public IGame GetGame(Func<IGame, bool> lambda)
        {
            return Items.Select(l => l.Value).ToList().FirstOrDefault(lambda);
        }
    }

    public interface IGameRepository : IBaseRepository<IGame>
    {
        IGame CreateGame(string name);
        IGame GetGame(Func<IGame, bool> lambda);
    }
}