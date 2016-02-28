using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Controllers
{
    public class GameController : Controller
    {
        private static IRandomWrapper _rnd = new RandomWrapper();

        public ActionResult Index()
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            return View();
        }

        public class GameModel
        {
            public string Name { get; set; }
            public int NewbiewCount { get; set; }
            public int BasicCount { get; set; }
            public int LearningCount { get; set; }
        }

        public ActionResult Create()
        {
            return View(new GameModel());
        }

        [HttpPost]
        public ActionResult Create(GameModel model)
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
            return RedirectToAction("Play", new { id = newGame.Id });
        }

        [HttpGet]
        public ActionResult Play(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var turnState = new TurnState();
            turnState.HumanPlayer = (IHumanPlayer)game.Players.First(p => p is IHumanPlayer);
            turnState.Deck = game.Decks.LastOrDefault();
            turnState.TurnType = game.TurnType;
            if (turnState.TurnType == TurnType.BeginDeck)
            {
                turnState.Deck = new Deck(game, _rnd);
            }
            else if (turnState.TurnType == TurnType.Pick)
            {
                game.PlayNonHumanPickTurns();
            }
            else if (turnState.TurnType == TurnType.PlayTrick)
            {
                game.PlayNonHumanPickTurns();
            }
            return View(turnState);
        }

        [HttpPost]
        public ActionResult Play(int id, int? indexOfCard, bool? willPick, string buriedCardIndicies)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            switch (game.TurnType)
            {
                case TurnType.BeginDeck:
                    break;
                case TurnType.Pick:
                    Pick(game, willPick.Value, buriedCardIndicies);
                    break;
                case TurnType.Bury:
                    Bury(game, buriedCardIndicies);
                    break;
                case TurnType.PlayTrick:
                    if (indexOfCard.HasValue)
                        PlayTrick(game, indexOfCard.Value);
                    break;
            }
            return RedirectToAction("Play", new { id = game.Id });
        }

        private void Pick(IGame game, bool willPick, string buriedCardIndicies)
        {
            var human = game.Players.OfType<IHumanPlayer>().First();
            var hand = game.ContinueFromHumanPickTurn(human, willPick);
        }

        private void Bury(IGame game, string buriedCardsIndicies)
        {
            var human = game.Players.OfType<IHumanPlayer>().First();
            var buriedCardsIndex = buriedCardsIndicies.Split(';').Select(c => Int16.Parse(c)).ToArray();
            var buriedCards = buriedCardsIndex.Select(i => human.Cards[i]).ToList();
            game.BuryCards(human, buriedCards);
        }

        private void PlayTrick(IGame game, int indexOfCard)
        {
            var player = game.Players.OfType<IHumanPlayer>().First();
            var card = player.Cards[indexOfCard];
            game.RecordTurn(player, card);
            game.PlayNonHumanPickTurns();
        }
    }
}
