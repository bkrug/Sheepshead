using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public class BuryStatConst
    {
        public static Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
            {
                { "BuriedPoints", new RangeDetail() { Min = 0, Max = 11 } },
                { "AvgBuriedRank", new RangeDetail() { Min = 0, Max = 20 } },
                { "PointsInHand", new RangeDetail() { Min = 0, Max = 11 } },
                { "AvgRankInHand", new RangeDetail() { Min = 0, Max = 20 } }
            };
    }
    
    public interface IBuryStatGuessRepository : IStatRepository<BuryStatUniqueKey, BuryStat>
    {
    }

    public class BuryStatGuessRepository : StatRepository<BuryStatUniqueKey, BuryStat>, IBuryStatGuessRepository
    {
        Dictionary<string, RangeDetail> MaxRanges = BuryStatConst.MaxRanges;

        public BuryStatGuessRepository() 
        {
            PopulateGuesses(0, new List<int>());
        }

        public void PopulateGuesses(int rangeIndex, List<int> rangeValues)
        {
        }

        protected override BuryStat CreateDefaultStat()
        {
            throw new NotImplementedException("CreateDefaultStat() should never be called from this particular repository.");
        }
    }
}
