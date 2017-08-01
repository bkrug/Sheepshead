using System;
using System.Collections.Generic;
using System.Linq;
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

    public class GameStartModel
    {
        public string GameName { get; set; }
        public int HumanCount { get; set; }
        public int NewbieCount { get; set; }
        public int BasicCount { get; set; }
        public int LearningCount { get; set; }
    }

    public class GameRepository : BaseRepository<IGame>, IGameRepository
    {
        public GameRepository(Dictionary<long, IGame> gameList) : base(gameList)
        {
        }

        public IGame CreateGame(GameStartModel model, IRandomWrapper rnd, ILearningHelperFactory factory)
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < model.HumanCount; ++i)
                playerList.Add(new HumanPlayer(new User()));
            for (var i = 0; i < model.NewbieCount; ++i)
                playerList.Add(new NewbiePlayer());
            for (var i = 0; i < model.BasicCount; ++i)
                playerList.Add(new BasicPlayer());
            //TODO: Guessers can be turned into Singletons
            for (var i = 0; i < model.LearningCount; ++i)
            {
                playerList.Add(new LearningPlayer(
                    new MoveKeyGenerator(),
                    new MoveStatResultPredictor(RepositoryRepository.Instance.MoveStatRepository),
                    new PickKeyGenerator(),
                    new PickStatResultPredictor(RepositoryRepository.Instance.PickStatRepository, new PickStatGuesser()),
                    new BuryKeyGenerator(),
                    new BuryStatResultPredictor(RepositoryRepository.Instance.BuryStatRepository, new BuryStatGuesser()),
                    new LeasterKeyGenerator(),
                    new LeasterStatResultPredictor(RepositoryRepository.Instance.LeasterStatRepository)
                ));
            }
            return CreateGame("", playerList, rnd, new LearningHelperFactory());
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