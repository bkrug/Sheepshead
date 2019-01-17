using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Logic
{
    public static class GameState
    {
        public static PlayState PlayState(IGame game, Guid requestingPlayerId)
        {
            var turnType = game.TurnType;
            var currentDeck = new GameStateDescriber(game).CurrentHand;
            var currentTrick = currentDeck?.PickPhaseComplete == true ? new GameStateDescriber(game).CurrentTrick : null;
            var currentPlayer =
                turnType == TurnType.PlayTrick
                ? currentTrick?.PlayersWithoutTurn?.FirstOrDefault()
                : null;
            var tricks = game.IHands.LastOrDefault(d => d.PickPhaseComplete)?.ITricks ?? new List<ITrick>();
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = game.Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new PlayState
            {
                TurnType = turnType.ToString(),
                HumanTurn = humanPlayer != null,
                CurrentTurn = currentPlayer?.Name,
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                CardsPlayed = tricks.Select(t => t.OrderedMoves
                                                  .Select(om => new Tuple<string, CardSummary>(om.Key.Name, CardUtil.GetCardSummary(om.Value)))
                                                  .ToList()
                                           )?.ToList(),
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c, currentTrick?.IsLegalAddition(c, requestingPlayer)))?.ToList()
            };
        }

        public static PickState PickState(IGame game, Guid requestingPlayerId)
        {
            var turnType = game.TurnType;
            var currentHand = game.IHands.LastOrDefault();
            var currentPlayer =
                turnType == TurnType.Pick
                ? currentHand?.PlayersWithoutPickTurn?.FirstOrDefault()
                : null;
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = game.Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            var pickChoices =
                currentHand?.PlayersRefusingPick.Select(p => new Tuple<string, bool>(p.Name, false))
                    .Union(new List<Tuple<string, bool>> { new Tuple<string, bool>(currentHand?.Picker?.Name, true) })
                    .Where(p => p.Item1 != null)
                    .ToList()
                    ?? new List<Tuple<string, bool>>();
            if (currentHand == null)
                return new PickState
                {
                    TurnType = turnType.ToString(),
                    RequestingPlayerTurn = false,
                    PickChoices = new List<Tuple<string, bool>>(),
                    PlayerCards = new List<CardSummary>(),
                    HumanTurn = false,
                    CurrentTurn = string.Empty,
                    MustRedeal = false
                };
            else
                return new PickState
                {
                    TurnType = turnType.ToString(),
                    RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                    PickChoices = pickChoices,
                    PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c))?.ToList(),
                    HumanTurn = humanPlayer != null,
                    CurrentTurn = currentPlayer?.Name,
                    MustRedeal = currentHand.MustRedeal
                };
        }

        public static BuryState BuryState(IGame game, Guid requestingPlayerId)
        {
            var turnType = game.TurnType;
            var currentDeck = new GameStateDescriber(game).CurrentHand;
            var currentPlayer =
                turnType == TurnType.Bury
                ? currentDeck?.Picker
                : null;
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = game.Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new BuryState
            {
                TurnType = turnType.ToString(),
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                Blinds = turnType == TurnType.Bury ? currentDeck?.Blinds?.Select(b => CardUtil.GetCardSummary(b))?.ToList() : null,
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c))?.ToList(),
                PartnerMethod = game.PartnerMethodEnum.ToString(),
                LegalCalledAces = humanPlayer?.LegalCalledAces(currentDeck).Select(c => CardUtil.GetCardSummary(c)).ToList()
            };
        }

        public static TrickResults GetTrickWinners(IHand hand)
        {
            var tricks = hand.ITricks ?? new List<ITrick>();
            var winners = tricks.Where(t => t.PlayersWithoutTurn.Count == 0).Select(t => t?.Winner()?.Player?.Name).ToList();
            return new TrickResults()
            {
                Picker = hand.Picker?.Name,
                Partner = hand.Partner?.Name,
                PartnerCard = hand.PartnerCardEnum == null ? null : CardUtil.GetAbbreviation(hand.PartnerCardEnum.Value),
                TrickWinners = winners,
                LeastersHand = hand?.Leasters ?? false,
                Tricks = hand
                             ?.ITricks
                             ?.Where(t => t.IsComplete())
                             ?.Select(trick =>
                               trick.OrderedMoves
                                   .Select(move => new KeyValuePair<string, CardSummary>(move.Key.Name, CardUtil.GetCardSummary(move.Value)))
                                   .ToList()
                               )
                             ?.ToList()
            };
        }
    }
}
