using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sheepshead.Model;
using Sheepshead.Model.Models;

namespace Sheepshead.React.Controllers
{
    public class SetupController : Controller
    {
        private IConfiguration _config;

        public SetupController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, string partnerCard, string leastersGame)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var partnerMethod = partnerCard?.Equals("Jack of Hearts", StringComparison.OrdinalIgnoreCase) == true
                ? PartnerMethod.JackOfDiamonds 
                : PartnerMethod.CalledAce;
            var leastersOn = leastersGame.Equals("On", StringComparison.OrdinalIgnoreCase);
            var game = repository.Create(humanCount, simpleCount, intermediateCount, advancedCount, partnerMethod, leastersOn);
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
                new Hand(game);
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

        [HttpGet]
        public IActionResult GetVersion()
        {
            var version = Assembly.GetAssembly(typeof(GameRepository)).GetName().Version.ToString();
            var inBeta = _config.GetValue<bool>("InBeta") ? " Beta" : string.Empty;
            return Json(version + inBeta);
        }
    }
}