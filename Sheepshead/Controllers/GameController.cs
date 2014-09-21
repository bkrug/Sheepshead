using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sheepshead.Models;

namespace Sheepshead.Controllers
{
    public class GameController : Controller
    {
        public ActionResult Index()
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            return View(repository.GetOpenGames().ToList());
        }

        public ActionResult Create()
        {
            return View(String.Empty);
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var newGame = repository.CreateGame(name);
            var rnd = new Random();
            var playerQueueRank = rnd.Next(5);
            for (var i = 0; i < 5; ++i)
                if (i == playerQueueRank)
                    newGame.AddPlayer(new HumanPlayer(new User()));
                else
                    newGame.AddPlayer(new NewbiePlayer());
            Session["gameId"] = newGame.Id;
            return RedirectToAction("Deal");
        }
    }
}
