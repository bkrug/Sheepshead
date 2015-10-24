using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Sheepshead.Models.Wrappers;
using System.Web.Script.Serialization;
using System.Timers;

namespace Sheepshead.Models.Players.Stats
{
    public interface IPickStatGuessRepository : IStatRepository<PickStatUniqueKey, PickStat>
    {
        List<PickStatUniqueKey> Keys { get; }
        PickStat GetRecordedResults(PickStatUniqueKey key);
    }

    public class PickStatGuessRepository : StatRepository<PickStatUniqueKey, PickStat>, IPickStatGuessRepository
    {
        Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
            {
                { "TrumpCount", new RangeDetail() { Min = 0, Max = 6  } },
                { "AvgTrumpRank", new RangeDetail() { Min = 1, Max = 14 } },
                { "PointsInHand", new RangeDetail() { Min = 0, Max = 64 } },
                { "TotalCardsWithPoints", new RangeDetail() { Min = 0, Max = 6 } }
            };

        protected override PickStat CreateDefaultStat()
        {
            throw new NotImplementedException("CreateDefaultStat() should never be called from this particular repository.");
        }

        public override PickStat GetRecordedResults(PickStatUniqueKey key)
        {
            var normalizedValues = new List<double>();
            foreach (var prop in typeof(PickStatUniqueKey).GetFields())
            {
                var value = (int)prop.GetValue(key);
                var range = MaxRanges[prop.Name];
                var normalized = ((double)value - range.Min) / (range.Max - range.Min);
                if (prop.Name == "AvgTrumpRank")
                    normalized = 1 - normalized;
                normalizedValues.Add(normalized);
            }
            var avg = normalizedValues.Average();
            var passPercent = avg;
            var pickPercent = Math.Sin(avg * Math.PI - Math.PI / 2) / 2 + 0.5;
            const double maxPossiblePassPoints = 2;
            const double maxPossiblePickPoints = 3;
            var passScore = 2 * maxPossiblePassPoints * passPercent - maxPossiblePassPoints;
            var pickScore = 2 * maxPossiblePickPoints * pickPercent - maxPossiblePickPoints;
            var handsTried = 5;
            return new PickStat()
            {
                TotalPassedPoints = (int)Math.Round(passScore * handsTried),
                HandsPassed = handsTried,
                TotalPickPoints = (int)Math.Round(pickScore * handsTried),
                HandsPicked = handsTried
            };
        }
    }
}
