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
            return RedirectToAction("Play", new { id = newGame.Id });
        }

        [HttpGet]
        public ActionResult Play(int id)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            var turnState = new TurnState();
            turnState.HumanPlayer = (IHumanPlayer)game.Players.First(p => p is IHumanPlayer);
            if (!game.Decks.Any() || game.LastDeckIsComplete())
            {
                turnState.TurnType = TurnType.BeginDeck;
                turnState.Deck = BeginDeck(game);
            }
            else if (game.Decks.Last().Hand == null || game.Decks.Last().Hand.Picker == null)
            {
                turnState.TurnType = TurnType.Pick;
                turnState.Deck = Pick(game);
            }
            else if (!game.Decks.Last().Buried.Any())
            {
                turnState.TurnType = TurnType.Bury;
                turnState.Deck = Bury(game);
            }
            else
            {
                turnState.TurnType = TurnType.PlayTrick;
                turnState.Deck = PlayTrick(game);
            }
            return View(turnState);
        }

        private IDeck BeginDeck(IGame game)
        {
            return game.LastDeckIsComplete() ? new Deck(game) : game.Decks.Last();
        }

        private IDeck Pick(IGame game)
        {
            var deck = game.Decks.Last();
            var picker = game.PlayNonHumans(deck);
            if (picker != null)
                ProcessPick(deck, (IComputerPlayer)picker);
            return deck;
        }

        public IDeck Bury(IGame game)
        {
            var deck = game.Decks.Last();
            ViewBag.HumanPlayer = game.Players.First(p => p is HumanPlayer);
            return deck;
        }

        private IDeck PlayTrick(IGame game)
        {
            var hand = game.Decks.Last().Hand;
            if (hand.IsComplete())
                throw new ApplicationException("Hand is already complete.");
            ITrick trick = hand.Tricks.LastOrDefault();
            if (trick == null || trick.IsComplete())
                trick = new Trick(hand);
            game.PlayNonHumans(trick);
            return hand.Deck;
        }

        [HttpPost]
        public ActionResult Play(int id, int? indexOfCard, bool? willPick, string buriedCardIndicies)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var game = repository.GetById(id);
            if (!game.Decks.Any() || game.LastDeckIsComplete())
            {
                return RedirectToAction("Play", new { id = game.Id });
            }
            else if (game.Decks.Last().Hand == null || game.Decks.Last().Hand.Picker == null)
            {
                return Pick(game, willPick.Value, buriedCardIndicies);
            }
            else if (!game.Decks.Last().Buried.Any())
            {
                return Bury(game, buriedCardIndicies);
            }
            else
            {
                return PlayTrick(game, indexOfCard.Value);
            }
        }

        private ActionResult Pick(IGame game, bool willPick, string buriedCardIndicies)
        {
            var deck = game.Decks.Last();
            IPlayer human = game.Players.First(p => p is HumanPlayer);
            if (willPick)
            {
                human.Cards.AddRange(deck.Blinds);
                new Hand(deck, human, new List<ICard>());
            }
            else
            {
                deck.PlayerWontPick(human);
                var picker = game.PlayNonHumans(game.Decks.Last());
                if (picker == null)
                    throw new ApplicationException("No one picked");
                ProcessPick(deck, (IComputerPlayer)picker);
            }
            return RedirectToAction("Play", new { id = game.Id });
        }

        private ActionResult Bury(IGame game, string buriedCardsIndicies)
        {
            var deck = game.Decks.Last();
            IPlayer human = game.Players.First(p => p is HumanPlayer);
            var buriedCardsIndex = buriedCardsIndicies.Split(';').Select(c => Int16.Parse(c)).ToArray();
            var buriedCards = buriedCardsIndex.Select(i => human.Cards[i]).ToList();
            buriedCards.ForEach(c => human.Cards.Remove(c));
            buriedCards.ForEach(c => deck.Buried.Add(c));
            return RedirectToAction("Play", new { id = game.Id });
        }

        private ActionResult PlayTrick(IGame game, int indexOfCard)
        {
            var hand = game.Decks.Last().Hand;
            ITrick trick = hand.Tricks.Last();
            var player = game.Players.First(p => p is HumanPlayer);
            var card = player.Cards[indexOfCard];
            trick.Add(player, card);
            game.PlayNonHumans(trick);
            return RedirectToAction("Play", new { id = game.Id });
        }

        private IHand ProcessPick(IDeck deck, IComputerPlayer picker)
        {
            var buriedCards = picker.DropCardsForPick(deck);
            return new Hand(deck, picker, buriedCards);
        }
    }
}
