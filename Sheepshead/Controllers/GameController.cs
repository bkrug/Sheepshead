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
            playerList.Add(new HumanPlayer(new User()));
            for (var i = 0; i < model.NewbiewCount; ++i)
                playerList.Add(new NewbiePlayer());
            for (var i = 0; i < model.BasicCount; ++i)
                playerList.Add(new BasicPlayer());
            var newGame = repository.CreateGame(model.Name, playerList);
            repository.Save(newGame);
            Session["gameId"] = newGame.Id;
            newGame.RearrangePlayers();
            return RedirectToAction("BeginDeck", new { id = newGame.Id });
        }

        public ActionResult BeginDeck(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = game.LastDeckIsComplete() ? new Deck(game) : game.Decks.Last();
            return View(deck);
        }

        public ActionResult Pick(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = game.Decks.Last();
            var picker = game.PlayNonHumans(deck);
            ViewBag.HumanPlayer = game.Players.First(p => p is HumanPlayer);
            if (picker == null)
            {
                return View(deck);
            }
            else
            {
                ProcessPick(deck, picker as ComputerPlayer);
                return RedirectToAction("ReportPick", new { id = game.Id });
            }
        }

        [HttpPost]
        public ActionResult Pick(int id, bool willPick, string droppedCardIndicies)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = game.Decks.Last();
            IPlayer human = game.Players.First(p => p is HumanPlayer);
            if (willPick)
            {
                human.Cards.AddRange(deck.Blinds);
                return RedirectToAction("Bury", new { id = game.Id });
            }
            else
            {
                deck.PlayerWontPick(human);
                var picker = game.PlayNonHumans(game.Decks.Last());
                if (picker == null)
                    throw new ApplicationException("No one picked");
                ProcessPick(deck, picker as ComputerPlayer);
                return RedirectToAction("ReportPick", new { id = game.Id });
            }
        }

        private IHand ProcessPick(IDeck deck, ComputerPlayer picker)
        {
            var droppedCards = picker.DropCardsForPick(deck);
            return new Hand(deck, picker, droppedCards);
        }

        public ActionResult Bury(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = game.Decks.Last();
            ViewBag.HumanPlayer = game.Players.First(p => p is HumanPlayer);
            return View(deck);
        }

        [HttpPost]
        public ActionResult Bury(int id, string droppedCardIndicies)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = game.Decks.Last();
            IPlayer human = game.Players.First(p => p is HumanPlayer);
            var droppedCardsIndex = droppedCardIndicies.Split(';').Select(c => Int16.Parse(c)).ToArray();
            var droppedCards = droppedCardsIndex.Select(i => human.Cards[i]).ToList();
            new Hand(deck, human, droppedCards);
            return RedirectToAction("ReportPick", new { id = game.Id });
        }

        public ActionResult ReportPick(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var hand = game.Decks.Last().Hand;
            return View(hand);
        }

        public ActionResult PlayTrick(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var hand = game.Decks.Last().Hand;
            if (hand.IsComplete())
                throw new ApplicationException("Hand is already complete.");
            ITrick trick = hand.Tricks.LastOrDefault();
            if (trick == null || trick.IsComplete())
                trick = new Trick(hand);
            game.PlayNonHumans(trick);
            ViewBag.HumanPlayer = game.Players.First(p => p is HumanPlayer);
            return View(trick);
        }

        [HttpPost]
        public ActionResult PlayTrick(int id, int indexOfCard)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var hand = game.Decks.Last().Hand;
            ITrick trick = hand.Tricks.Last();
            var player = game.Players.First(p => p is HumanPlayer);
            var card = player.Cards[indexOfCard];
            trick.Add(player, card);
            game.PlayNonHumans(trick);
            return RedirectToAction("ReportTrick", new { id = id });
        }

        public ActionResult ReportTrick(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var hand = game.Decks.Last().Hand;
            ITrick trick = hand.Tricks.Last();
            return View(trick);
        }

        public ActionResult ReportHand(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var hand = game.Decks.Last().Hand;
            return View(hand);
        }
    }
}
