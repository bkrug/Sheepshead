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
            Session["gameId"] = newGame.Id;
            newGame.RearrangePlayers();
            var deck = new Deck(newGame);
            return RedirectToAction("Pick", new { id = newGame.Id });
        }

        public ActionResult Pick(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = new Deck(game);
            var picker = game.PlayNonHumans(deck);
            if (picker == null)
                return View(deck);
            else
            {
                ProcessPick(deck, picker as ComputerPlayer);
                return RedirectToAction("ReportPick", new { id = game.Id });
            }
        }

        [HttpPost]
        public ActionResult Pick(int id, bool willPick, int[] droppedCardsIndex)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var deck = game.Decks.Last();
            IPlayer picker;
            if (willPick)
            {
                picker = game.Players.First(p => p is HumanPlayer);
                var droppedCards = droppedCardsIndex.Select(i => picker.Cards[i]).ToList();
                ProcessPick(deck, picker, droppedCards);
            }
            else
            {
                picker = game.PlayNonHumans(game.Decks.Last());
                if (picker == null)
                    throw new ApplicationException("No one picked");
                ProcessPick(deck, picker as ComputerPlayer);
            }
            return RedirectToAction("ReportPick", new { id = game.Id } );
        }

        private IHand ProcessPick(IDeck deck, ComputerPlayer picker)
        {
            var droppedCards = picker.DropCardsForPick(deck.Hand, picker);
            return ProcessPick(deck, picker, droppedCards);
        }

        private IHand ProcessPick(IDeck deck, IPlayer picker, List<ICard> droppedCards)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            return new Hand(deck, picker, droppedCards);
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
            ITrick trick = hand.Tricks.Last();
            if (trick.IsComplete())
                trick = new Trick(hand);
            game.PlayNonHumans(trick);
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
