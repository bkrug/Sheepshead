using System;
using System.Linq;
using Sheepshead.Models;
using Microsoft.AspNetCore.Mvc;

namespace Sheepshead.React.Controllers
{
    public class SetupController : Controller
    {
        [HttpPost]
        public IActionResult Create(int humanCount, int newbieCount, int basicCount)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.Create(humanCount, newbieCount, basicCount, PartnerMethod.JackOfDiamonds);
            return RedirectToAction("RegisterHuman", "Setup", new { id = game.Id });
        }

        [HttpPost]
        public IActionResult RegisterHuman(string gameId, string playerName)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var player = game.UnassignedPlayers.FirstOrDefault();
            player?.AssignToClient(playerName);
            if (!game.UnassignedPlayers.Any())
            {
                game.MaybeGiveComputerPlayersNames();
                new Deck(game);
            }
            return Json(new {
                gameId = game.Id,
                playerId = player?.Id,
                full = player == null
            });
        }

        [HttpGet]
        public IActionResult AllPlayersReady(string gameId)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            return Json(new
            {
                allPlayersReady = !game.UnassignedPlayers.Any()
            });
        }
    }
}