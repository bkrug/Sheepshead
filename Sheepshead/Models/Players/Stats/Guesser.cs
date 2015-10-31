using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public interface IGuesser<K, V>
    {
        V MakeGuess(K key);
    }

    public abstract class Guesser<K, V> : IGuesser<K, V>
    {
        protected Dictionary<K, V> _dict = new Dictionary<K, V>();
        protected Dictionary<string, RangeDetail> MaxRanges;
        protected List<string> ReverseValue = new List<string>();

        public Guesser()
        {
            SetRangeValues();
            PopulateGuesses(0, new List<int>());
        }

        protected abstract void SetRangeValues();
        protected abstract K CreateKeyInstance();
        protected abstract V CreateStatInstance();

        public virtual V MakeGuess(K key)
        {
            if (_dict.ContainsKey(key))
                return _dict[key];
            throw new KeyNotFoundException("This guess has not been made.");
        }

        protected void PopulateGuesses(int rangeIndex, List<int> rangeValues)
        {
            var rangeDetail = MaxRanges.ElementAt(rangeIndex).Value;
            for (var newValue = rangeDetail.Min; newValue <= rangeDetail.Max; ++newValue)
            {
                var rangeValues2 = new List<int>(rangeValues);
                rangeValues2.Add(newValue);
                if (rangeIndex < MaxRanges.Count() - 1)
                {
                    PopulateGuesses(rangeIndex + 1, rangeValues2);
                }
                else
                {
                    var key = CreateKey(rangeValues2);
                    _dict[key] = CreateResult(key);
                }
            }
        }

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
        protected List<double> NormalizeKeyValues(ref K key)
        {
            var normalizedValues = new List<double>();
            var type = key.GetType();
            foreach (var prop in type.GetFields())
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

        //The resulting list will have values between 0 and 1 to compare how close the value is to the max and min value of the range.
        protected Dictionary<string, double> NormalizeKeyValuesAsDictionary(ref K key)
        {
            var normalizedValues = new Dictionary<string, double>();
            var type = key.GetType();
            foreach (var prop in type.GetFields())
            {
                var value = (int)prop.GetValue(key);
                var range = MaxRanges[prop.Name];
                var normalized = ((double)value - range.Min) / (range.Max - range.Min);
                if (ReverseValue.Contains(prop.Name))
                    normalized = 1 - normalized;
                normalizedValues.Add(prop.Name, normalized);
            }
            return normalizedValues;
        }
    }
}