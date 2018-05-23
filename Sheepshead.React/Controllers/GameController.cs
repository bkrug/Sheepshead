using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sheepshead.Models;

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
    }
}
