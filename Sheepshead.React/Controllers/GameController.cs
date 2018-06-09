using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.React.Controllers
{
    public class GameController : Controller
    {
        [HttpGet]
        public IActionResult Summary(string gameId)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var hand = game.TurnState?.Deck?.Hand;
            if (hand == null)
                return Json(game.Players.Select(p => new
                                {
                                    name = p.Name
                                }));
            else
                return Json(hand.Scores().Select(s => new
                                {
                                    name = s.Key.Name,
                                    score = s.Value
                                }));
        }

        [HttpGet]
        public IActionResult GetCards(string gameId, string playerId)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var player = game.Players.OfType<IHumanPlayer>().Single(p => p.Id == Guid.Parse(playerId));
            return Json(player.Cards.Select(c => new
            {
                filename = CardUtil.GetPictureFilename(c),
                cardAbbr = CardUtil.ToAbbr(c)
            }));
        }

        [HttpGet]
        public IActionResult GetPlayState(string gameId, string playerId)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var playState = game.PlayState(Guid.Parse(playerId));
            if (!playState.HumanTurn && game.TurnType == TurnType.Pick)
            {
                game.PlayNonHumanPickTurns();
                playState = game.PlayState(Guid.Parse(playerId));
            }
            return Json(playState);
        }

        [HttpPost]
        public IActionResult RecordPickChoice(string gameId, string playerId, bool willPick)
        {
            var playerGuid = Guid.Parse(playerId);
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var human = game.Players.OfType<IHumanPlayer>().Single(h => h.Id == playerGuid);
            var hand = game.ContinueFromHumanPickTurn(human, willPick);
            if (willPick)
                return Json(game.PlayState(playerGuid).Blinds);
            return Json(new List<int>());
        }

        [HttpPost]
        public IActionResult RecordBury(string gameId, string playerId, string[] cardFilenames)
        {
            var playerGuid = Guid.Parse(playerId);
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var human = game.Players.OfType<IHumanPlayer>().Single(h => h.Id == playerGuid);
            var buriedCards = cardFilenames.Select(c => CardUtil.GetCardFromFilename(c)).ToList();
            game.BuryCards(human, buriedCards);
            return Json(new { buryRecorded = true });
        }
    }
}
