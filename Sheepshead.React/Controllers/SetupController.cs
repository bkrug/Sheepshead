using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var game = repository.Create(humanCount, newbieCount, basicCount);
            return RedirectToAction("RegisterHuman", new { id = game.Id });
        }

        [HttpPost]
        public IActionResult RegisterHuman(string gameId, string playerName)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(Guid.Parse(gameId));
            var player = game.UnassignedPlayers.FirstOrDefault();
            if (player == null)
                return RedirectToAction("GameFull", new { gameId = game.Id });
            
            player.AssignToClient();
            return RedirectToAction("Play", "Game", new { gameId = game.Id, playerId = player.Id });
        }
    }
}