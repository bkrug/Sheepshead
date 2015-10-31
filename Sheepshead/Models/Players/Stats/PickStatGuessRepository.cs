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
    public interface IGuesser<K, V>
    {
        V GetRecordedResults(K key);
    }

    public abstract class Guesser<K, V> : IGuesser<K, V>
    {
        protected Dictionary<K, V> _dict = new Dictionary<K, V>();
        protected Dictionary<string, RangeDetail> MaxRanges;
        protected List<string> ReverseValue;

        protected void PopulateGuesses(int rangeIndex, List<int> rangeValues)
        {
            var rangeDetail = MaxRanges.ElementAt(rangeIndex).Value;
            for (var newValue = rangeDetail.Min; newValue <= rangeDetail.Max; ++newValue)
            {
                if (rangeIndex < MaxRanges.Count() - 1)
                {
                    var rangeValues2 = new List<int>(rangeValues);
                    rangeValues2.Add(newValue);
                    PopulateGuesses(rangeIndex + 1, rangeValues2);
                }
                else
                {
                    var rangeValues2 = new List<int>(rangeValues);
                    rangeValues2.Add(newValue);
                    var key = CreateKey(rangeValues2);
                    _dict[key] = CreateResult(key);
                }
            }
        }

        protected abstract K CreateKeyInstance();
        protected abstract V CreateStatInstance();

        protected K CreateKey(List<int> rangeValues)
        {
            var newKey = CreateKeyInstance();
            var type = newKey.GetType();
            for (var i = 0; i < MaxRanges.Count; ++i)
            {
                var fieldInfo = type.GetField(MaxRanges.ElementAt(i).Key);
                fieldInfo.SetValueDirect(__makeref(newKey), rangeValues[i]);
            }
            return newKey;
        }

        protected abstract V CreateResult(K key);

        //The resulting list will have values between 0 and 1 to compare how close the value is to the max and min value of the range.
        protected List<double> NormalizeKeyValues(ref PickStatUniqueKey key)
        {
            var normalizedValues = new List<double>();
            foreach (var prop in typeof(PickStatUniqueKey).GetFields())
            {
                var value = (int)prop.GetValue(key);
                var range = MaxRanges[prop.Name];
                var normalized = ((double)value - range.Min) / (range.Max - range.Min);
                if (ReverseValue.Contains(prop.Name))
                    normalized = 1 - normalized;
                normalizedValues.Add(normalized);
            }
            return normalizedValues;
        }

        public virtual V GetRecordedResults(K key)
        {
            if (_dict.ContainsKey(key))
                return _dict[key];
            return CreateStatInstance();
        }
    }

    public interface IPickStatGuesser : IGuesser<PickStatUniqueKey, PickStat>
    {
    }

    public class PickStatGuesser : Guesser<PickStatUniqueKey, PickStat>, IPickStatGuesser
    {
        public PickStatGuesser() 
        {
            MaxRanges = PickStatConst.MaxRanges;
            ReverseValue = new List<string>() {"AvgTrumpRank"};
            PopulateGuesses(0, new List<int>());
        }

        protected override PickStatUniqueKey CreateKeyInstance()
        {
            return new PickStatUniqueKey();
        }

        protected override PickStat CreateStatInstance()
        {
            return new PickStat();
        }

        protected override PickStat CreateResult(PickStatUniqueKey key)
        {
            var normalizedValues = NormalizeKeyValues(ref key);
            double passScore;
            double pickScore;
            CreateImaginaryScores(normalizedValues, out passScore, out pickScore);
            var handsTried = 5;
            return new PickStat()
            {
                TotalPassedPoints = (int)Math.Round(passScore * handsTried),
                HandsPassed = handsTried,
                TotalPickPoints = (int)Math.Round(pickScore * handsTried),
                HandsPicked = handsTried
            };
        }

        private static void CreateImaginaryScores(List<double> normalizedValues, out double passScore, out double pickScore)
        {
            var avg = normalizedValues.Average();
            var passPercent = avg;
            var pickPercent = Math.Sin(avg * Math.PI - Math.PI / 2) / 2 + 0.5;
            const double maxPossiblePassPoints = 2;
            const double maxPossiblePickPoints = 3;
            passScore = 2 * maxPossiblePassPoints * passPercent - maxPossiblePassPoints;
            pickScore = 2 * maxPossiblePickPoints * pickPercent - maxPossiblePickPoints;
        }
    }
}
