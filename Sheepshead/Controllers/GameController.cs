using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Controllers
{
    public class GameController : Controller
    {
        private static IRandomWrapper _rnd = new RandomWrapper();

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
            public int LearningCount { get; set; }
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
            for (var i = 0; i < model.LearningCount; ++i)
                playerList.Add(new LearningPlayer(new Key2Generator(), SummaryLoaderSingleton.Instance.ResultPredictor));
            var newGame = repository.CreateGame(model.Name, playerList, _rnd);
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
            turnState.Deck = game.Decks.LastOrDefault();
            if (!game.Decks.Any() || game.LastDeckIsComplete())
            {
                turnState.TurnType = TurnType.BeginDeck;
                turnState.Deck = new Deck(game, _rnd);
            }
            else if (game.Decks.Last().Hand == null)
            {
                turnState.TurnType = TurnType.Pick;
                Pick(game);
            }
            else if (!turnState.Deck.Buried.Any() && !turnState.Deck.Hand.Leasters)
            {
                turnState.TurnType = TurnType.Bury;
            }
            else
            {
                turnState.TurnType = TurnType.PlayTrick;
                PlayTrick(game);
            }
            return View(turnState);
        }

        private void Pick(IGame game)
        {
            var deck = game.Decks.Last();
            var picker = game.PlayNonHumans(deck);
            if (picker != null)
            {
                var hand = ProcessPick(deck, (IComputerPlayer)picker);
                new LearningHelper(hand, SaveLocations.FIRST_SAVE);
            }
        }

        private void PlayTrick(IGame game)
        {
            var hand = game.Decks.Last().Hand;
            if (hand.IsComplete())
                throw new ApplicationException("Hand is already complete.");
            ITrick trick = hand.Tricks.LastOrDefault();
            if (trick == null || trick.IsComplete())
                trick = new Trick(hand);
            game.PlayNonHumans(trick);
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
            else if (game.Decks.Last().Hand == null)
            {
                Pick(game, willPick.Value, buriedCardIndicies);
            }
            else if (!game.Decks.Last().Buried.Any() && !game.Decks.Last().Hand.Leasters)
            {
                Bury(game, buriedCardIndicies);
            }
            else if (indexOfCard.HasValue)
            {
                PlayTrick(game, indexOfCard.Value);
            }
            return RedirectToAction("Play", new { id = game.Id });
        }

        private void Pick(IGame game, bool willPick, string buriedCardIndicies)
        {
            var deck = game.Decks.Last();
            IHand hand;
            IPlayer human = game.Players.First(p => p is HumanPlayer);
            if (willPick)
            {
                human.Cards.AddRange(deck.Blinds);
                hand = new Hand(deck, human, new List<ICard>());
            }
            else
            {
                deck.PlayerWontPick(human);
                var picker = game.PlayNonHumans(game.Decks.Last());
                hand = ProcessPick(deck, (IComputerPlayer)picker);
            }
            new LearningHelper(hand, SaveLocations.FIRST_SAVE);
        }

        private void Bury(IGame game, string buriedCardsIndicies)
        {
            var deck = game.Decks.Last();
            IPlayer human = game.Players.First(p => p is HumanPlayer);
            var buriedCardsIndex = buriedCardsIndicies.Split(';').Select(c => Int16.Parse(c)).ToArray();
            var buriedCards = buriedCardsIndex.Select(i => human.Cards[i]).ToList();
            buriedCards.ForEach(c => human.Cards.Remove(c));
            buriedCards.ForEach(c => deck.Buried.Add(c));
        }

        private void PlayTrick(IGame game, int indexOfCard)
        {
            var hand = game.Decks.Last().Hand;
            ITrick trick = hand.Tricks.Last();
            var player = game.Players.First(p => p is HumanPlayer);
            var card = player.Cards[indexOfCard];
            trick.Add(player, card);
            game.PlayNonHumans(trick);
        }

        private IHand ProcessPick(IDeck deck, IComputerPlayer picker)
        {
            var buriedCards = picker != null ? picker.DropCardsForPick(deck) : new List<ICard>();
            return new Hand(deck, picker, buriedCards);
        }
    }
}
