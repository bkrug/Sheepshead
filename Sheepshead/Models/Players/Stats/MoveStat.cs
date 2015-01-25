using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class MoveStat
    {
        public int TricksWon {get; set; }
        public int TricksTried {get; set; }
        public int HandsWon {get; set; }
        public int HandsTried { get; set; }

        public double? TrickPortionWon { get { return TricksTried == 0 ? null : (double?)TricksWon / TricksTried; } }
        public double? GamePortionWon { get { return HandsTried == 0 ? null : (double?)HandsWon / HandsTried; } }
    }

    public struct MoveStatUniqueKey {
        public int Picker;
        public int? Partner;
        public int Trick;
        public int MoveWithinTrick;
        public int PointsAlreadyInTrick;
        public int TotalPointsInPreviousTricks;

        public int PointsInThisCard;
        public int RankOfThisCard;
        public bool PartnerCard;
        public int HigherRankingCardsPlayedPreviousTricks;
        public int HigherRankingCardsPlayedThisTrick;
    }
}