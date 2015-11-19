using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models.Players.Stats
{
    public interface ILeasterKeyGenerator
    {
        LeasterStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard);
    }

    public class LeasterKeyGenerator : BaseMoveKeyGenerator, ILeasterKeyGenerator
    {
        //TODO: Write a test for what happens when previousMoves is an empty list and cardMatchesSuit is true.
        public LeasterStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard)
        {
            var cardMatchesSuit = !trick.OrderedMoves.Any() || CardRepository.GetSuit(trick.OrderedMoves.First().Value) == CardRepository.GetSuit(legalCard);
            var queueRankOfPlayer = player.QueueRankInTrick(trick);
            var indexOfTrick = trick.Hand.Tricks.IndexOf(trick);
            var previousTricks = trick.Hand.Tricks.Take(indexOfTrick).ToList();
            var previousMoves = trick.OrderedMoves.Take(queueRankOfPlayer - 1).ToList();
            return new LeasterStatUniqueKey()
            {
                WonOneTrick = trick.Hand.Tricks.Take(indexOfTrick).Any(t => t.Winner().Player == player),
                LostOneTrick = trick.Hand.Tricks.Take(indexOfTrick).Any(t => t.Winner().Player != player),
                CardMatchesSuit = cardMatchesSuit,
                MostPowerfulInTrick = !previousMoves.Any() || cardMatchesSuit && legalCard.Rank <= previousMoves.Min(c => c.Value.Rank),
                OpponentPercentDone = (queueRankOfPlayer - 1) * 100 / (trick.PlayerCount - 1),
                AvgVisibleCardPoints = (int)Math.Round((previousMoves.Sum(m => m.Value.Points) + legalCard.Points) / (double)queueRankOfPlayer),
                UnknownStrongerCards = StrongerUnknownCards(trick, player, legalCard, queueRankOfPlayer, previousTricks),
                HeldStrongerCards = StrongerHeldCards(player, legalCard, previousTricks, trick)
            };
        }
    }
}