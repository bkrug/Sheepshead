using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public class StatResultPredictor
    {
        IMoveStatRepository _repository;

        public StatResultPredictor(IMoveStatRepository repository)
        {
            _repository = repository;
        }

        public MoveStat GetWeightedStat(MoveStatUniqueKey key)
        {
            var mainStat = _repository.GetRecordedResults(key);
            if (mainStat.HandsTried >= 1000)
                return mainStat;
            var requiredStats = 1000;
            var usedKeys = new List<MoveStatUniqueKey>();
            double offset = 0;
            var stat = new MoveStat();
            while (stat.HandsTried < requiredStats && offset <= 0.509)
            {
                var offsetKey = new MoveStatUniqueKey();
                offsetKey.CardWillOverpower = key.CardWillOverpower;
                var propertyNames = new List<string>();
                var ranges = new List<RangeDetail>();
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
                var startingKey = new MoveStatUniqueKey()
                {
                    CardWillOverpower = key.CardWillOverpower
                };
                AddKeys(startingKey, usedKeys, propertyNames, ranges, ref stat);
                offset = Math.Round(offset + 0.05, 5);
            }
            var overpowers = usedKeys.Select(k => k.CardWillOverpower).Distinct().ToList();
            var percent = usedKeys.Select(k => k.OpponentPercentDone).OrderBy(k => k).Distinct().ToList();
            var points = usedKeys.Select(k => k.CardPoints).OrderBy(k => k).Distinct().ToList();
            var unknowns = usedKeys.Select(k => k.UnknownStrongerCards).OrderBy(k => k).Distinct().ToList();
            var inhands = usedKeys.Select(k => k.HeldStrongerCards).OrderBy(k => k).Distinct().ToList();
            return stat;
        }

        private void AddKeys(MoveStatUniqueKey oldKey, List<MoveStatUniqueKey> usedKeys, List<string> propertyNames, List<RangeDetail> ranges, ref MoveStat stat)
        {
            var propertyName = propertyNames.First();
            var range = ranges.First();
            foreach (var v in GetRange(propertyName, range))
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

        private IEnumerable<int> GetRange(string propertyName, RangeDetail currentRange)
        {
            var range = MaxRanges[propertyName];
            var minVal = Math.Max(currentRange.Min, range.Min);
            var maxVal = Math.Min(currentRange.Max, range.Max);
            if (range.ValidValues != null && range.ValidValues.Any())
                return range.ValidValues.Where(v => v >= minVal && v <= maxVal);
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