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
        public IActionResult Summary(string gameId)
        {
            IGame game = GetGame(gameId);
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
        public IActionResult GetPlayState(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var playState = game.PlayState(Guid.Parse(playerId));
            if (!playState.HumanTurn)
            {
                switch (game.TurnType) {
                    case TurnType.Pick:
                        game.PlayNonHumanPickTurns();
                        playState = game.PlayState(Guid.Parse(playerId));
                        break;
                    case TurnType.PlayTrick:
                        game.PlayNonHumansInTrick();
                        playState = game.PlayState(Guid.Parse(playerId));
                        break;
                    default:
                        break;
                }
            }
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
        public IActionResult RecordBury(string gameId, string playerId, string[] cardFilenames)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var buriedCards = cardFilenames.Select(c => CardUtil.GetCardFromFilename(c)).ToList();
            game.BuryCards(human, buriedCards);
            return Json(new { buryRecorded = true });
        }

        [HttpPost]
        public IActionResult RecordTrickChoice(string gameId, string playerId, string cardFilename)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var playedCard = CardUtil.GetCardFromFilename(cardFilename);
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
