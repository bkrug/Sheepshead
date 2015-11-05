using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class PickStat : IStat<PickStat>
    {
        public int TotalPickPoints { get; set; }
        public int HandsPicked { get; set; }
        public int TotalPassedPoints { get; set; }
        public int HandsPassed { get; set; }

        [ScriptIgnore]
        public double? AvgPickPoints { get { return HandsPicked == 0 ? null : (double?)TotalPickPoints / HandsPicked; } }
        [ScriptIgnore]
        public double? AvgPassedPoints { get { return HandsPassed == 0 ? null : (double?)TotalPassedPoints / HandsPassed; } }

        public void AddOtherStat(PickStat otherStat)
        {
            TotalPassedPoints += otherStat.TotalPassedPoints;
            HandsPassed += otherStat.HandsPassed;
            TotalPickPoints += otherStat.TotalPickPoints;
            HandsPicked += otherStat.HandsPicked;
        }
    }

    public struct PickStatUniqueKey : IStatUniqueKey
    {
        public int TrumpCount;
        public int AvgTrumpRank;
        public int AvgPointsInHand;
        public int TotalCardsWithPoints;
    }

    public class PickStatConst
    {
        public static Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
            {
                { "TrumpCount", new RangeDetail() { Min = 0, Max = 6  } },
                { "AvgTrumpRank", new RangeDetail() { Min = 0, Max = 14 } },
                { "AvgPointsInHand", new RangeDetail() { Min = 0, Max = 11 } },
                { "TotalCardsWithPoints", new RangeDetail() { Min = 0, Max = 6 } }
            };
    }
}