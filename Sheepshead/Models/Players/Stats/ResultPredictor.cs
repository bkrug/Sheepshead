using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public abstract class ResultPredictor<K, S> where S : IStat<S> where K : IStatUniqueKey
    {
        protected readonly IStatRepository<K, S> Repository;
        protected Dictionary<string, RangeDetail> MaxRanges;

        public ResultPredictor(IStatRepository<K, S> repository)
        {
            Repository = repository;
        }

        protected abstract List<K> CreateKeyList();
        protected abstract S CreateStat();
        protected abstract bool ReachedMinimumTries(S generatedStat);

        public virtual S GetWeightedStat(K key)
        {
            var realStat = Repository.GetRecordedResults(key);
            if (ReachedMinimumTries(realStat))
                return realStat;
            var usedKeys = CreateKeyList();
            var generatedStat = CreateStat();
            double offset = 0;
            while (!ReachedMinimumTries(generatedStat) && offset <= 0.509)
            {
                var propertyNames = new List<string>();
                var ranges = new List<RangeDetail>();
                CreateSearchRange(key, offset, propertyNames, ranges);
                AddKeys(key, usedKeys, propertyNames, ranges, ref generatedStat);
                offset = Math.Round(offset + 0.15, 5);
            }
            return generatedStat;
        }

        protected void CreateSearchRange<T>(T key, double offset, List<string> propertyNames, List<RangeDetail> ranges)
        {
            foreach (var rangeKey in MaxRanges.Keys)
            {
                int centerValue = (int)typeof(T).GetField(rangeKey).GetValue(key);
                var extreme = (MaxRanges[rangeKey].Max - MaxRanges[rangeKey].Min) / 2;
                propertyNames.Add(rangeKey);
                ranges.Add(new RangeDetail()
                {
                    Min = (int)Math.Round(centerValue - offset * extreme),
                    Max = (int)Math.Round(centerValue + offset * extreme)
                });
            }
        }

        protected void AddKeys(K oldKey, List<K> usedKeys, List<string> propertyNames, List<RangeDetail> ranges, ref S stat)
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
                    var recordedStat = Repository.GetRecordedResults(newKey);
                    stat.AddOtherStat(recordedStat);
                    usedKeys.Add(newKey);
                }
            }
        }

        protected IEnumerable<int> GetSearchValues(string propertyName, RangeDetail limitedRange)
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
    }

    public struct RangeDetail
    {
        public int Min;
        public int Max;
        public List<int> ValidValues;
    }
}