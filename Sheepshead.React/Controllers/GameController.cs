using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sheepshead.Logic.Models;
using Sheepshead.Logic;
using Sheepshead.Logic.DAL;
using Sheepshead.Logic.Players;

namespace Sheepshead.React.Controllers
{
    public class GameController : Controller
    {
        private readonly GameRepository _gameRepository;
        private readonly ScoreRepository _scoreRepository;

        public GameController(SheepsheadContext context)
        {
            _gameRepository = new GameRepository(context);
            _scoreRepository = new ScoreRepository(context);
        }

        [HttpGet]
        public IActionResult GameSummary(string gameId)
        {
            var game = GetGame(gameId);
            game.Hands.ToList().ForEach(hand => _scoreRepository.RecordScores(hand));
            var gameCoins = _scoreRepository.GameCoins(game.Id);
            return Json(gameCoins.Select(p => new {
                name = p.Name,
                score = p.Coins
            }));
        }
        
        [HttpGet]
        public IActionResult HandSummary(string gameId)
        {
            IGame game = GetGame(gameId);
            var hand = game.IHands.LastOrDefault(d => d.IsComplete());
            var mustRedeal = hand?.MustRedeal;
            var scores = hand?.CalculateScores();
            return Json(new
            {
                points = scores?.Points?.ToDictionary(k => k.Key.Name, k => k.Value),
                coins = scores?.Coins?.ToDictionary(k => k.Key.Name, k => k.Value),
                tricks = hand.ITricks
                             .Select(trick =>
                                new KeyValuePair<string, List<CardSummary>> (
                                    trick.Winner().Player.Name,
                                    trick.OrderedMoves.Select(c => CardUtil.GetCardSummary(c.Value)).ToList()
                                )
                             ),
                mustRedeal,
                buried = hand.Buried.Select(c => CardUtil.GetCardSummary(c)).ToList()
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
                cardAbbr = CardUtil.GetAbbreviation(c)
            }));
        }

        [HttpGet]
        public IActionResult GetTurnType(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            return Json(new
            {
                gameExists = game != null,
                turnType = game?.TurnType.ToString(),
                playerCount = game?.PlayerCount,
                trickCount = game?.TrickCount
            });
        }

        [HttpGet]
        public IActionResult GetPickState(string gameId, string playerId)
        {
            var game = GetGame(gameId);
            var playState = GameState.PickState(game, Guid.Parse(playerId));
            if (game.TurnType == TurnType.Pick && !playState.HumanTurn)
            {
                game.PlayNonHumanPickTurns();
                playState = GameState.PickState(game, Guid.Parse(playerId));
                _gameRepository.UpdateGame(game);
                _gameRepository.Save();
            }
            return Json(playState);
        }

        [HttpGet]
        public IActionResult GetBuryState(string gameId, string playerId)
        {
            IGame game = GetGame(gameId);
            var playState = GameState.BuryState(game, Guid.Parse(playerId));
            return Json(playState);
        }

        [HttpGet]
        public IActionResult GetPlayState(string gameId, string playerId)
        {
            var game = GetGame(gameId);
            var playState = GameState.PlayState(game, Guid.Parse(playerId));
            if (game.TurnType == TurnType.PlayTrick && !playState.HumanTurn)
            {
                game.PlayNonHumansInTrick();
                playState = GameState.PlayState(game, Guid.Parse(playerId));
                _gameRepository.UpdateGame(game);
                _gameRepository.Save();
            }
            return Json(playState);
        }

        [HttpGet]
        public IActionResult GetTrickResults(string gameId)
        {
            var game = GetGame(gameId);
            var hand = game.IHands.LastOrDefault(d => d.PickPhaseComplete);
            var trickWinner = GameState.GetTrickWinners(hand ?? new Hand());
            return Json(trickWinner);
        }

        [HttpPost]
        public IActionResult StartDeck(string gameId, string playerId)
        {
            var game = GetGame(gameId);
            var mustRedeal = game.IHands.LastOrDefault()?.MustRedeal ?? false;
            if (game.TurnState.TurnType == TurnType.BeginHand || mustRedeal)
                new Hand(game);
            var playState = GameState.PlayState(game, Guid.Parse(playerId));
            _gameRepository.UpdateGame(game);
            _gameRepository.Save();
            return Json(playState);
        }

        [HttpPost]
        public IActionResult RecordPickChoice(string gameId, string playerId, bool willPick)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var hand = game.ContinueFromHumanPickTurn(human, willPick);
            _gameRepository.UpdateGame(game);
            _gameRepository.Save();
            if (willPick)
                return Json(GameState.PlayState(game, Guid.Parse(playerId)).Blinds);
            return Json(new List<int>());
        }

        [HttpPost]
        public IActionResult RecordBury(string gameId, string playerId, string[] cards, bool goItAlone, string partnerCard)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var buriedCards = cards.Select(c => Enum.Parse<SheepCard>(c)).ToList();
            var partnerCardVal = !string.IsNullOrEmpty(partnerCard) ? (SheepCard?)Enum.Parse<SheepCard>(partnerCard): null;
            game.BuryCards(human, buriedCards, goItAlone, partnerCardVal);
            _gameRepository.UpdateGame(game);
            _gameRepository.Save();
            return Json(new { buryRecorded = true });
        }

        [HttpPost]
        public IActionResult RecordTrickChoice(string gameId, string playerId, string card)
        {
            GetGameAndHuman(gameId, playerId, out var game, out var human);
            var playedCard = Enum.Parse<SheepCard>(card);
            game.RecordTurn(human, playedCard);
            game.PlayNonHumansInTrick();
            _gameRepository.UpdateGame(game);
            _gameRepository.Save();
            return Json(new { playRecorded = true });
        }

        private Game GetGame(string gameId)
        {
            return _gameRepository.GetGameById(Guid.Parse(gameId));
        }

        private void GetGameAndHuman(string gameId, string playerId, out Game game, out IHumanPlayer human)
        {
            var playerGuid = Guid.Parse(playerId);
            game = _gameRepository.GetGameById(Guid.Parse(gameId));
            human = game.Players.OfType<IHumanPlayer>().Single(h => h.Id == playerGuid);
        }
    }
}
