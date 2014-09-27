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
            return View();
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
            var newGame = repository.CreateGame(model.Name, playerList);
            Session["gameId"] = newGame.Id;
            newGame.RearrangePlayers();
            var deck = new Deck(newGame);
            return RedirectToAction("Pick");
        }

        public class GameModel
        {
            public string Name { get; set; }
            public int NewbiewCount { get; set; }
            public int BasicCount { get; set; }
        }
    }
}
