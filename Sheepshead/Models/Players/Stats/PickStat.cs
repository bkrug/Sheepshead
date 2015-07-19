using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class PickStat
    {
        public int TotalPickPoints { get; set; }
        public int HandsPicked { get; set; }
        public int TotalPassedPoints { get; set; }
        public int HandsPassed { get; set; }

        [ScriptIgnore]
        public double? AvgPickPoints { get { return HandsPicked == 0 ? null : (double?)TotalPickPoints / HandsPicked; } }
        [ScriptIgnore]
        public double? AvgPassedPoints { get { return HandsPassed == 0 ? null : (double?)TotalPassedPoints / HandsPassed; } }
    }

    public struct PickStatUniqueKey
    {
        public int TrumpCount;
        public int AvgTrumpRank;
        public int PointsInHand;
        public int TotalCardsWithPoints;
    }
}