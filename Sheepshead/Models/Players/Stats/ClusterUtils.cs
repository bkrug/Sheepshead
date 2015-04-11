using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class ClusterUtils
    {
        public static double Distance(MoveStatUniqueKey tuple, MoveStatCentroid centroid)
        {
            double sumSquareDiffs = 0.0;
            sumSquareDiffs += Math.Pow((tuple.HigherRankingCardsPlayedPreviousTricks - centroid.HigherRankingCardsPlayedPreviousTricks), 2);
            sumSquareDiffs += Math.Pow((tuple.HigherRankingCardsPlayedThisTrick - centroid.HigherRankingCardsPlayedThisTrick), 2);
            sumSquareDiffs += Math.Pow((tuple.MoveWithinTrick - centroid.MoveWithinTrick), 2);
            if (tuple.Partner.HasValue)
                sumSquareDiffs += Math.Pow((tuple.Partner.Value - centroid.Partner.Value), 2);
            sumSquareDiffs += Math.Pow(((tuple.PartnerCard ? 1 : 0) - centroid.PartnerCard), 2);
            sumSquareDiffs += Math.Pow((tuple.Picker - centroid.Picker), 2);
            sumSquareDiffs += Math.Pow((tuple.PointsAlreadyInTrick - centroid.PointsAlreadyInTrick), 2);
            sumSquareDiffs += Math.Pow((tuple.PointsInThisCard - centroid.PointsInThisCard), 2);
            sumSquareDiffs += Math.Pow((tuple.RankOfThisCard - centroid.RankOfThisCard), 2);
            sumSquareDiffs += Math.Pow((tuple.TotalPointsInPreviousTricks - centroid.TotalPointsInPreviousTricks), 2);
            sumSquareDiffs += Math.Pow((tuple.Trick - centroid.Trick), 2);
            return Math.Sqrt(sumSquareDiffs);
        }
    }
}