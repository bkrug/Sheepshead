using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public interface IKey2Generator
    {
        MoveStatUniqueKey2 GenerateKey(ITrick trick, IPlayer player, ICard legalCard);
        MoveStatUniqueKey2 GenerateKey(int indexOfTrick, int indexOfPlayer, int queueRankOfPicker, int pointsInPreviousTricks,
            ITrick trick, ICard playedCard, List<ITrick> previousTricks, List<ICard> previousCards, List<ICard> heldCards,
            ref bool beforePartnerCardPlayed, ref int? queueRankOfPartner, ref int pointsInTrick, ref int highestRankInTrick,
            ref int indexOfWinningPlayer);
    }

    public class Key2Generator : IKey2Generator
    {
        public MoveStatUniqueKey2 GenerateKey(ITrick trick, IPlayer player, ICard legalCard)
        {
            var indexOfTrick = trick.Hand.Tricks.IndexOf(trick);
            List<IPlayer> playerList = trick.Hand.Players;
            var previousTricks = trick.Hand.Tricks.Take(indexOfTrick).ToList();
            var beforePartnerCardPlayed = BeforePartnerCardPlayed(trick, indexOfTrick);
            var queueRankOfPlayer = player.QueueRankInTrick(trick);
            var precedingMoves = trick.OrderedMoves.Take(queueRankOfPlayer).ToList();
            var highestRankInTrick = precedingMoves.Any() ? precedingMoves.Min(c => c.Value.Rank) : 33;
            var winningPlayer = trick.OrderedMoves.Where(m => m.Value.Rank == highestRankInTrick).Select(m => m.Key).FirstOrDefault();
            var playedCards = previousTricks.SelectMany(t => t.OrderedMoves.Select(m => m.Value)).ToList();
            playedCards.AddRange(trick.OrderedMoves.Take(queueRankOfPlayer).Select(m => m.Value));
            var knownCards = GetCardsKnownToPlayer(trick, player, playedCards);
            var morePowerfulCards = legalCard.Rank - 1;
            var heldCards = GetUnplayedHeldCards(player, legalCard, previousTricks);
            var offenseSide = trick.Hand.Picker == player || trick.Hand.Partner == player || player.Cards.Contains(trick.PartnerCard);
            var winningPlayerIsPartner = winningPlayer != null && winningPlayer.Cards.Contains(trick.PartnerCard) || trick.Hand.Partner == winningPlayer;
            return new MoveStatUniqueKey2()
            {
                OffenseSide = offenseSide,
                PickerDone = queueRankOfPlayer > trick.QueueRankOfPicker,
                PartnerDone = beforePartnerCardPlayed ? (bool?)null : queueRankOfPlayer > trick.QueueRankOfPartner,
                PointsInTrick = trick.OrderedMoves.Take(queueRankOfPlayer).Sum(c => c.Value.Points),
                HighestRankInTrick = highestRankInTrick,
                WinningSide = (trick.Hand.Picker == winningPlayer || trick.Hand.Partner == winningPlayer) == offenseSide,
                ThisCardMorePowerful = legalCard.Rank < highestRankInTrick,
                MorePowerfulUnknownCards = morePowerfulCards - knownCards.Count(c => c.Rank < legalCard.Rank),
                RemainingUnknownPoints = 120 - knownCards.Sum(c => c.Points),
                MorePowerfulHeld = heldCards.Count(c => c.Rank < legalCard.Rank),
                PointsHeld = heldCards.Sum(c => c.Points),
                CardsHeldWithPoints = heldCards.Count(c => c.Points > 0),
                MoveIndex = queueRankOfPlayer,
                TrickIndex = indexOfTrick
            };
        }

        private static List<ICard> GetUnplayedHeldCards(IPlayer player, ICard legalCard, List<ITrick> previousTricks)
        {
            var heldCards = player.Cards.ToList();
            foreach (var card in previousTricks.Select(t => t.CardsPlayed[player]))
                heldCards.Remove(card);
            heldCards.Remove(legalCard);
            return heldCards;
        }

        private static List<ICard> GetCardsKnownToPlayer(ITrick trick, IPlayer player, List<ICard> playedCards)
        {
            var knownCards = playedCards.ToList();
            if (trick.Hand.Picker == player)
                knownCards.AddRange(trick.Hand.Deck.Buried);
            knownCards = knownCards.Union(player.Cards).ToList();
            return knownCards;
        }

        private static bool BeforePartnerCardPlayed(ITrick trick, int indexOfTrick)
        {
            if (trick.Hand.PartnerCardPlayed == null)
                return true;
            if (indexOfTrick < trick.Hand.PartnerCardPlayed[0])
                return true;
            if (indexOfTrick > trick.Hand.PartnerCardPlayed[0])
                return false;
            var partnerPosition = trick.QueueRankOfPartner;
            return !partnerPosition.HasValue || partnerPosition < trick.Hand.PartnerCardPlayed[1];
        }

        public MoveStatUniqueKey2 GenerateKey(int indexOfTrick, int indexOfPlayer, int queueRankOfPicker, int pointsInPreviousTricks,
            ITrick trick, ICard playedCard, List<ITrick> previousTricks, List<ICard> previousCards, List<ICard> heldCards,
            ref bool beforePartnerCardPlayed, ref int? queueRankOfPartner, ref int pointsInTrick, ref int highestRankInTrick,
            ref int indexOfWinningPlayer)
        {
            if (trick.Hand.PartnerCard.Id == playedCard.Id) beforePartnerCardPlayed = false;
            queueRankOfPartner = queueRankOfPartner ?? trick.QueueRankOfPartner;
            var knownCards = previousCards.Union(heldCards).ToList();
            if (queueRankOfPicker == indexOfPlayer)
                knownCards.AddRange(trick.Hand.Deck.Blinds);
            var morePowerfulCards = playedCard.Rank - 1;
            var thisCardMorePowerful = highestRankInTrick > playedCard.Rank;
            var offenseSide = queueRankOfPicker == indexOfPlayer || queueRankOfPartner == indexOfPlayer;
            var key = new MoveStatUniqueKey2()
            {
                OffenseSide = offenseSide,
                PickerDone = indexOfPlayer > queueRankOfPicker,
                PartnerDone = beforePartnerCardPlayed ? (bool?)null : indexOfPlayer > queueRankOfPartner,
                PointsInTrick = pointsInTrick,
                HighestRankInTrick = highestRankInTrick,
                WinningSide = (queueRankOfPicker == indexOfWinningPlayer || queueRankOfPartner == indexOfWinningPlayer) == offenseSide,
                ThisCardMorePowerful = thisCardMorePowerful,
                MorePowerfulUnknownCards = morePowerfulCards - knownCards.Count(c => c.Rank < playedCard.Rank),
                RemainingUnknownPoints = 120 - knownCards.Sum(c => c.Points),
                MorePowerfulHeld = heldCards.Count(c => c.Rank < playedCard.Rank),
                PointsHeld = heldCards.Sum(c => c.Points),
                CardsHeldWithPoints = heldCards.Count(c => c.Points > 0),
                MoveIndex = indexOfPlayer,
                TrickIndex = indexOfTrick
            };
            pointsInTrick += playedCard.Points;
            if (thisCardMorePowerful)
            {
                indexOfWinningPlayer = indexOfPlayer;
                highestRankInTrick = playedCard.Rank;
            }
            return key;
        }
    }
}