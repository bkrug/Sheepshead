using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface IStatResultPredictor
    {
        MoveStat GetWeightedStat(MoveStatUniqueKey key);
    }

    public class StatResultPredictor : IStatResultPredictor
    {
        IMoveStatRepository _repository;

        public StatResultPredictor(IMoveStatRepository repository)
        {
            _repository = repository;
        }

        public MoveStat GetWeightedStat(MoveStatUniqueKey key)
        {
            var realStat = _repository.GetRecordedResults(key);
            var minimumnTries = 1000;
            if (realStat.HandsTried >= minimumnTries)
                return realStat;
            var usedKeys = new List<MoveStatUniqueKey>();
            double offset = 0;
            var generatedStat = new MoveStat();
            while (generatedStat.HandsTried < minimumnTries && offset <= 0.509)
            {
                var propertyNames = new List<string>();
                var ranges = new List<RangeDetail>();
                CreateSearchRange(key, offset, propertyNames, ranges);
                AddKeys(key, usedKeys, propertyNames, ranges, ref generatedStat);
                offset = Math.Round(offset + 0.05, 5);
            }
            return generatedStat;
        }

        private void CreateSearchRange(MoveStatUniqueKey key, double offset, List<string> propertyNames, List<RangeDetail> ranges)
        {
            foreach (var rangeKey in MaxRanges.Keys)
            {
                int centerValue = (int)typeof(MoveStatUniqueKey).GetField(rangeKey).GetValue(key);
                var extreme = (MaxRanges[rangeKey].Max - MaxRanges[rangeKey].Min) / 2;
                propertyNames.Add(rangeKey);
                ranges.Add(new RangeDetail()
                {
                    Min = (int)Math.Round(centerValue - offset * extreme),
                    Max = (int)Math.Round(centerValue + offset * extreme)
                });
            }
        }

        private void AddKeys(MoveStatUniqueKey oldKey, List<MoveStatUniqueKey> usedKeys, List<string> propertyNames, List<RangeDetail> ranges, ref MoveStat stat)
        {
            var propertyName = propertyNames.First();
            var range = ranges.First();
            foreach (var v in GetSearchValues(propertyName, range))
            {
                var newKey = oldKey;
                newKey.GetType().GetField(propertyName).SetValueDirect(__makeref(newKey), v);
                if (ranges.Count() > 1)
                    AddKeys(newKey, usedKeys, propertyNames.Skip(1).ToList(), ranges.Skip(1).ToList(), ref stat);
                else if (!usedKeys.Contains(newKey))
                {
                    var recordedStat = _repository.GetRecordedResults(newKey);
                    stat.AddOtherStat(recordedStat);
                    usedKeys.Add(newKey);
                }
            }
        }

        private IEnumerable<int> GetSearchValues(string propertyName, RangeDetail limitedRange)
        {
            var maxRange = MaxRanges[propertyName];
            var minVal = Math.Max(limitedRange.Min, maxRange.Min);
            var maxVal = Math.Min(limitedRange.Max, maxRange.Max);
            if (maxRange.ValidValues != null && maxRange.ValidValues.Any())
                return maxRange.ValidValues.Where(v => v >= minVal && v <= maxVal);
            else
            {
                var list = new List<int>();
                for (var v = minVal; v <= maxVal; ++v)
                    list.Add(v);
                return list;
            }
        }

        private struct RangeDetail
        {
            public int Min;
            public int Max;
            public List<int> ValidValues;
        }

        private Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
        {
            { "OpponentPercentDone", new RangeDetail() { Min = 0, Max = 100, ValidValues = new List<int>() { 0, 25, 33, 50, 67, 75, 100 } } },
            { "CardPoints", new RangeDetail() { Min = -11, Max = 11, ValidValues = new List<int>() { -11, -10, -4, -3, -2, 0, 2, 3, 4, 10, 11 } } },
            { "UnknownStrongerCards", new RangeDetail() { Min = 0, Max = 19 } },
            { "HeldStrongerCards", new RangeDetail() { Min = 0, Max = 5 } }
        };
    }
}