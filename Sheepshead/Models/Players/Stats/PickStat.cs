using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class PickStat
    {
        public int PicksWon { get; set; }
        public int HandsPicked { get; set; }
        public int PassedWon { get; set; }
        public int HandsPassed { get; set; }

        [ScriptIgnore]
        public double? PickPortionWon { get { return HandsPicked == 0 ? null : (double?)PicksWon / HandsPicked; } }
        [ScriptIgnore]
        public double? PassedPortionWon { get { return HandsPassed == 0 ? null : (double?)PassedWon / HandsPassed; } }
    }

    public struct PickStatUniqueKey
    {
        public int TrumpCount;
        public int AvgTrumpRank;
        public int TrumpStdDeviation;
        public int PointsInHand;
        public int TotalCardsWithPoints;
    }
}