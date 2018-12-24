using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sheepshead.Model.DAL;
using Sheepshead.Logic.Models;

namespace Sheepshead.React.Controllers
{
    public class SetupController : Controller
    {
        private GameRepository _gameRepository;
        private IConfiguration _config;

        public SetupController(IConfiguration config, SheepsheadContext context)
        {
            _gameRepository = new GameRepository(context);
            _config = config;
        }

        [HttpPost]
        public IActionResult Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, string partnerCard, string leastersGame)
        {
            var partnerMethod = partnerCard?.Equals("Jack of Hearts", StringComparison.OrdinalIgnoreCase) == true
                ? PartnerMethod.JackOfDiamonds 
                : PartnerMethod.CalledAce;
            var leastersOn = leastersGame.Equals("On", StringComparison.OrdinalIgnoreCase);
            var game = _gameRepository.Create(humanCount, simpleCount, intermediateCount, advancedCount, partnerMethod, leastersOn);
            _gameRepository.Save();
            return RedirectToAction("RegisterHuman", "Setup", new { id = game.Id });
        }

        [HttpPost]
        public IActionResult RegisterHuman(string gameId, string playerName)
        {
            var game = _gameRepository.GetGameById(Guid.Parse(gameId));
            var player = game.UnassignedPlayers.FirstOrDefault();
            player?.AssignToClient(playerName);
            if (!game.UnassignedPlayers.Any())
            {
                game.MaybeGiveComputerPlayersNames();
                new Hand(game);
            }
            _gameRepository.UpdateGame(game);
            _gameRepository.Save();
            return Json(new {
                gameId = game.Id,
                playerId = player?.Id,
                full = player == null
            });
        }

        [HttpGet]
        public IActionResult AllPlayersReady(string gameId)
        {
            var game = _gameRepository.GetGameById(Guid.Parse(gameId));
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