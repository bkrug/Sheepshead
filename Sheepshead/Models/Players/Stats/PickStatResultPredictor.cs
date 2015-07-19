using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface IPickResultPredictor
    {
        PickStat GetWeightedStat(PickStatUniqueKey key);
    }

    public class PickStatResultPredictor : IPickResultPredictor
    {
        IPickStatRepository _repository;

        public PickStatResultPredictor(IPickStatRepository repository)
        {
            _repository = repository;
        }

        public PickStat GetWeightedStat(PickStatUniqueKey key)
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
            var pickPercent = Math.Sin(avg * Math.PI / 2 - Math.PI / 2) + 1;
            var passScore = 4 * passPercent - 2;
            var pickScore = 6 * pickPercent - 3;
            var handsTried = 5;
            return new PickStat()
            {
                TotalPassedPoints = (int)Math.Round(passScore * handsTried),
                HandsPassed = handsTried,
                TotalPickPoints = (int)Math.Round(pickScore * handsTried),
                HandsPicked = handsTried
            };
        }

        private struct RangeDetail
        {
            public int Min;
            public int Max;
        }

        private Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
        {
            { "TrumpCount", new RangeDetail() { Min = 0, Max = 6  } },
            { "AvgTrumpRank", new RangeDetail() { Min = 1, Max = 14 } },
            { "PointsInHand", new RangeDetail() { Min = 0, Max = 64 } },
            { "TotalCardsWithPoints", new RangeDetail() { Min = 0, Max = 6 } }
        };
    }
}