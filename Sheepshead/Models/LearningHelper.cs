using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using System.IO;

namespace Sheepshead.Models
{
    public interface IKeyGenerator
    {
        MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard);
        MoveStatUniqueKey GenerateKey(int indexOfTrick, int indexOfPlayer, int queueRankOfPicker, int pointsInPreviousTricks,
            ITrick trick, ICard playedCard, List<ITrick> previousTricks, List<ICard> previousCards,
            ref bool beforePartnerCardPlayed, ref int? queueRankOfPartner, ref int pointsInTrick);
    }

    public class KeyGenerator : IKeyGenerator
    {
        public MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard)
        {
            var indexOfTrick = trick.Hand.Tricks.IndexOf(trick);
            List<IPlayer> playerList = trick.Hand.Players;
            var previousTricks = trick.Hand.Tricks.Take(indexOfTrick).ToList();
            var beforePartnerCardPlayed = BeforePartnerCardPlayed(trick, indexOfTrick);
            return new MoveStatUniqueKey()
            {
                Picker = trick.QueueRankOfPicker,
                Partner = beforePartnerCardPlayed ? null : trick.QueueRankOfPartner,
                Trick = trick.IndexInHand,
                MoveWithinTrick = player.QueueRankInTrick(trick),
                PointsAlreadyInTrick = trick.CardsPlayed.Sum(c => c.Value.Points),
                TotalPointsInPreviousTricks = previousTricks.Sum(t => t.CardsPlayed.Sum(c => c.Value.Points)),
                PointsInThisCard = legalCard.Points,
                RankOfThisCard = legalCard.Rank,
                PartnerCard = trick.Hand.PartnerCard == legalCard,
                HigherRankingCardsPlayedPreviousTricks =
                    previousTricks
                    .SelectMany(t => t.CardsPlayed.Select(kvp => kvp.Value))
                    .Count(c => c.Rank < legalCard.Rank),
                HigherRankingCardsPlayedThisTrick =
                    trick.CardsPlayed
                    .Select(kvp => kvp.Value)
                    .Count(c => c.Rank < legalCard.Rank)
            };
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

        public MoveStatUniqueKey GenerateKey(int indexOfTrick, int indexOfPlayer, int queueRankOfPicker, int pointsInPreviousTricks, 
            ITrick trick, ICard playedCard, List<ITrick> previousTricks, List<ICard> previousCards, 
            ref bool beforePartnerCardPlayed, ref int? queueRankOfPartner, ref int pointsInTrick)
        {
            if (trick.Hand.PartnerCard.Id == playedCard.Id) beforePartnerCardPlayed = false;
            queueRankOfPartner = beforePartnerCardPlayed ? null : queueRankOfPartner ?? trick.QueueRankOfPartner;
            var key = new MoveStatUniqueKey()
            {
                Picker = queueRankOfPicker,
                Partner = queueRankOfPartner,
                Trick = indexOfTrick,
                MoveWithinTrick = indexOfPlayer,
                PointsAlreadyInTrick = pointsInTrick,
                TotalPointsInPreviousTricks = pointsInPreviousTricks,
                PointsInThisCard = playedCard.Points,
                RankOfThisCard = playedCard.Rank,
                PartnerCard = trick.Hand.PartnerCard.Id == playedCard.Id,
                HigherRankingCardsPlayedPreviousTricks =
                    previousTricks.Sum(t => t.CardsPlayed.Count(c => c.Value.Rank < playedCard.Rank)),
                HigherRankingCardsPlayedThisTrick =
                    previousCards.Count(c => c.Rank < playedCard.Rank)
            };
            pointsInTrick += playedCard.Points;
            return key;
        }
    }

    public class LearningHelper
    {
        private string _saveLocation;

        private LearningHelper()
        {
        }

        public LearningHelper(IHand hand, string saveLocation)
        {
            _saveLocation = saveLocation;
            hand.OnHandEnd += WriteHandSummary;
        }

        private static object lockObject = new object();

        private void WriteHandSummary(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            lock (lockObject)
            {
                using (var sw = File.AppendText(_saveLocation))
                {
                    sw.WriteLine(hand.Summary());
                    sw.Flush();
                }
            }
        }
    }
}