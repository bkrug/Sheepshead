using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public struct MoveStatCentroid
    {
        public double Picker;
        public double? Partner;
        public double Trick;
        public double MoveWithinTrick;
        public double PointsAlreadyInTrick;
        public double TotalPointsInPreviousTricks;

        public double PointsInThisCard;
        public double RankOfThisCard;
        public double PartnerCard;
        public double HigherRankingCardsPlayedPreviousTricks;
        public double HigherRankingCardsPlayedThisTrick;
    }
}