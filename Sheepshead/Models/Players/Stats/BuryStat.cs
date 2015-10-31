using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class BuryStat : IStat<BuryStat>
    {
        public int TotalPoints { get; set; }
        public int HandsPicked { get; set; }

        [ScriptIgnore]
        public double? AvgPickPoints { get { return HandsPicked == 0 ? null : (double?)TotalPoints / HandsPicked; } }

        public void AddOtherStat(BuryStat otherStat)
        {
            TotalPoints += otherStat.TotalPoints;
            HandsPicked += otherStat.HandsPicked;
        }
    }

    public struct BuryStatUniqueKey : IStatUniqueKey
    {
        public int BuriedPoints;
        public int PointsInHand;
        public int AvgRankInHand;
        public int SuitsInHand;
    }
}