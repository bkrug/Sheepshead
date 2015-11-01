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

        protected abstract Dictionary<K, bool> CreateKeyList();
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
                _validSearchValues = new Dictionary<string, IEnumerable<int>>();
                CreateSearchRange(key, offset, propertyNames, ranges);
                AddKeys(key, new Stack<int>(), usedKeys, propertyNames, ranges, 0, ref generatedStat);
                offset = Math.Round(offset + 0.05, 5);
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

        protected void AddKeys(K originalKey, Stack<int> keyValues, Dictionary<K, bool> usedKeys, List<string> propertyNames, List<RangeDetail> ranges, int depth, ref S stat)
        {
            var propertyName = propertyNames[depth];
            var range = ranges[depth];
            var maxDepth = propertyNames.Count() - 1;
            foreach (var v in GetSearchValues(propertyName, range))
            {
                keyValues.Push(v);
                if (depth < maxDepth)
                    AddKeys(originalKey, keyValues, usedKeys, propertyNames, ranges, depth+1, ref stat);
                else
                {
                    var newKey = CreateKey(originalKey, keyValues);
                    if (!usedKeys.ContainsKey(newKey))
                    {
                        var recordedStat = Repository.GetRecordedResults(newKey);
                        stat.AddOtherStat(recordedStat);
                        usedKeys.Add(newKey, false);
                    }
                }
                keyValues.Pop();
            }
        }

        protected abstract K CreateKey(K originalKey, Stack<int> keyValues);
        protected Dictionary<string, IEnumerable<int>> _validSearchValues;
        protected IEnumerable<int> GetSearchValues(string propertyName, RangeDetail limitedRange)
        {
            if (!_validSearchValues.ContainsKey(propertyName))
            {
                List<int> list;
                var maxRange = MaxRanges[propertyName];
                var minVal = Math.Max(limitedRange.Min, maxRange.Min);
                var maxVal = Math.Min(limitedRange.Max, maxRange.Max);
                if (maxRange.ValidValues != null && maxRange.ValidValues.Any())
                    list = maxRange.ValidValues.Where(v => v >= minVal && v <= maxVal).ToList();
                else
                {
                    list = new List<int>();
                    for (var v = minVal; v <= maxVal; ++v)
                        list.Add(v);
                }
                _validSearchValues[propertyName] = list;
            }
            return _validSearchValues[propertyName];
        }
    }

    public struct RangeDetail
    {
        public int Min;
        public int Max;
        public List<int> ValidValues;
    }
}