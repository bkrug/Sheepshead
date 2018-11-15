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
            public int SimpleCount { get; set; }
            public int IntermediateCount { get; set; }
            public int AdvancedCount { get; set; }
        }

        public ActionResult Create()
        {
            return View(new GameModel());
        }

        [HttpPost]
        public ActionResult Create(GameModel model)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var newGame = repository.Create(1, model.SimpleCount, model.IntermediateCount, model.AdvancedCount, PartnerMethod.JackOfDiamonds, true);
            return RedirectToAction("Play", new { id = newGame.Id });
        }

        [HttpGet]
        public ActionResult Play(string id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(id));
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
        public ActionResult Play(string id, int? indexOfCard, bool? willPick, string buriedCardIndicies)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(id));
            switch (game.TurnType)
            {
                case TurnType.BeginDeck:
                    break;
                case TurnType.Pick:
                    Pick(game, willPick.Value);
                    break;
                case TurnType.Bury:
                    Bury(game, buriedCardIndicies, false);
                    break;
                case TurnType.PlayTrick:
                    if (indexOfCard.HasValue)
                        PlayTrick(game, indexOfCard.Value);
                    break;
            }
            return RedirectToAction("Play", new { id = game.Id });
        }

        private void Pick(IGame game, bool willPick)
        {
            var human = game.Players.OfType<IHumanPlayer>().First();
            var hand = game.ContinueFromHumanPickTurn(human, willPick);
        }

        private void Bury(IGame game, string buriedCardsIndicies, bool goItAlone)
        {
            var human = game.Players.OfType<IHumanPlayer>().First();
            var buriedCardsIndex = buriedCardsIndicies.Split(';').Select(c => Int16.Parse(c)).ToArray();
            var buriedCards = buriedCardsIndex.Select(i => human.Cards[i]).ToList();
            game.BuryCards(human, buriedCards, goItAlone, null);
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