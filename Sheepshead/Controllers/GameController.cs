using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;


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
            playerList.Add(new HumanPlayer());
            for (var i = 0; i < model.NewbiewCount; ++i)
                playerList.Add(new NewbiePlayer());
            for (var i = 0; i < model.BasicCount; ++i)
                playerList.Add(new BasicPlayer());
            var newGame = repository.CreateGame(model.Name, playerList, _rnd);
            repository.Save(newGame);
            newGame.RearrangePlayers();
            return RedirectToAction("Play", new { id = newGame.Id });
        }

        [HttpGet]
        public ActionResult Play(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var turnState = game.TurnState;
            switch (game.TurnType)
            {
                case TurnType.BeginDeck:
                    turnState.Deck = new Deck(game, _rnd);
                    break;
                case TurnType.Pick:
                    game.PlayNonHumanPickTurns(true);
                    break;
                case TurnType.PlayTrick:
                    game.PlayNonHumansInTrick();
                    break;
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
            game.PlayNonHumansInTrick();
        }
    }
}