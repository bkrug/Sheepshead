using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;
using System.Collections.Generic;
using System.Web.Http;

namespace Sheeshead.WebApi.Controllers
{
    public class GameController : ApiController
    {
        private static IRandomWrapper _rnd = new RandomWrapper();

        public class GameModel
        {
            public string Name { get; set; }
            public int NewbiewCount { get; set; }
            public int BasicCount { get; set; }
            public int LearningCount { get; set; }
        }
        
        public string GetText()
        {
            return "Live long and prosper.";
        }

        [HttpPost]
        public string Create(GameModel model)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var playerList = new List<IPlayer>();
            playerList.Add(new HumanPlayer(new User()));
            for (var i = 0; i < model.NewbiewCount; ++i)
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
            var newGame = repository.CreateGame(model.Name, playerList, _rnd, new LearningHelperFactory());
            repository.Save(newGame);
            newGame.RearrangePlayers();
            return "Success is ours!";
        }
    }
}
