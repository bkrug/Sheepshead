using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public interface IHandStat
    {
        int HandsWon { get; set; }
        int HandsTried { get; set; }

        [ScriptIgnore]
        double? HandPortionWon { get; }
    }

    public class PickStat : IHandStat
    {
        public int HandsWon { get; set; }
        public int HandsTried { get; set; }

        [ScriptIgnore]
        public double? HandPortionWon { get { return HandsTried == 0 ? null : (double?)HandsWon / HandsTried; } }
    }

    public struct PickStatUniqueKey
    {
        public int AvgTrumpRank;
        public int TrumpStdDeviation;
        public int PointsInHand;
        public int TotalCardsWithPoints;
    }
}