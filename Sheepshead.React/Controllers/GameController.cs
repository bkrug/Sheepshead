using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.React.Controllers
{
    public class GameController : Controller
    {
        [HttpGet]
        public IActionResult GameSummary(string gameId)
        {
            var gameCoins = GetGame(gameId).GameCoins();
            return Json(gameCoins.Select(p => new {
                name = p.Name,
                score = p.Coins
            }));
        }
        
        [HttpGet]
        public IActionResult HandSummary(string gameId)
        {
            IGame game = GetGame(gameId);
            var mustRedeal = game.Decks.LastOrDefault(d => d.IsComplete)?.MustRedeal;
            var hand = game.Decks.LastOrDefault(d => d.Hand != null)?.Hand;
            var scores = hand?.Scores();
            return Json(new
            {
                points = scores?.Points?.ToDictionary(k => k.Key.Name, k => k.Value),
                coins = scores?.Coins?.ToDictionary(k => k.Key.Name, k => k.Value),
                mustRedeal = mustRedeal
            });
        }

        [HttpGet]
        public IActionResult GetCards(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var player = game.Players.OfType<IHumanPlayer>().Single(p => p.Id == Guid.Parse(playerId));
            return Json(player.Cards.Select(c => new
            {
                filename = CardUtil.GetPictureFilename(c),
                cardAbbr = CardUtil.ToAbbr(c)
            }));
        }

        [HttpGet]
        public IActionResult GetTurnType(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            return Json(new
            {
                turnType = game.TurnType.ToString(),
                playerCount = game.PlayerCount,
                trickCount = game.TrickCount
            });
        }

        [HttpGet]
        public IActionResult GetPickState(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var playState = game.PickState(Guid.Parse(playerId));
            if (game.TurnType == TurnType.Pick && !playState.HumanTurn)
            {
                game.PlayNonHumanPickTurns();
                playState = game.PickState(Guid.Parse(playerId));
            }
            return Json(playState);
        }

        [HttpGet]
        public IActionResult GetBuryState(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var playState = game.BuryState(Guid.Parse(playerId));
            return Json(playState);
        }

        [HttpGet]
        public IActionResult GetPlayState(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var playState = game.PlayState(Guid.Parse(playerId));
            if (game.TurnType == TurnType.PlayTrick && !playState.HumanTurn)
            {
                game.PlayNonHumansInTrick();
                playState = game.PlayState(Guid.Parse(playerId));
            }
            return Json(playState);
        }

        [HttpGet]
        public IActionResult GetTrickResults(string gameId)
        {
            var game = GetGame(gameId);
            return Json(game.GetTrickWinners());
        }

        [HttpPost]
        public IActionResult StartDeck(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var mustRedeal = game.Decks.LastOrDefault()?.MustRedeal ?? false;
            if (game.TurnState.TurnType == TurnType.BeginDeck || mustRedeal)
                new Deck(game);
            var playState = game.PlayState(Guid.Parse(playerId));
            return Json(playState);
        }

        [HttpPost]
        public IActionResult RecordPickChoice(string gameId, string playerId, bool willPick)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var hand = game.ContinueFromHumanPickTurn(human, willPick);
            if (willPick)
                return Json(game.PlayState(Guid.Parse(playerId)).Blinds);
            return Json(new List<int>());
        }

        [HttpPost]
        public IActionResult RecordBury(string gameId, string playerId, string[] cards, bool goItAlone, string partnerCard)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var buriedCards = cards.Select(c => Enum.Parse<SheepCard>(c)).ToList();
            var partnerCardVal = !string.IsNullOrEmpty(partnerCard) ? (SheepCard?)Enum.Parse<SheepCard>(partnerCard): null;
            game.BuryCards(human, buriedCards, goItAlone, partnerCardVal);
            return Json(new { buryRecorded = true });
        }

        [HttpPost]
        public IActionResult RecordTrickChoice(string gameId, string playerId, string card)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var playedCard = Enum.Parse<SheepCard>(card);
            game.RecordTurn(human, playedCard);
            game.PlayNonHumansInTrick();
            return Json(new { playRecorded = true });
        }

        private static IGame GetGame(string gameId)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            return repository.GetById(Guid.Parse(gameId));
        }

        private static void GetGameAndHuman(string gameId, string playerId, out IGame game, out IHumanPlayer human)
        {
            var playerGuid = Guid.Parse(playerId);
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            game = repository.GetById(Guid.Parse(gameId));
            human = game.Players.OfType<IHumanPlayer>().Single(h => h.Id == playerGuid);
        }
    }
}
