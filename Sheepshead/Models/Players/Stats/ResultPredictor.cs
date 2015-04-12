using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{

    public class ResultPredictor
    {
        IMoveStatRepository _repository;

        public ResultPredictor(IMoveStatRepository repository)
        {
            _repository = repository;
        }

        public MoveStat GetWeightedStat(MoveStatUniqueKey key)
        {
            var mainStat = _repository.GetRecordedResults(key);
            if (mainStat.HandsTried >= 10000)
                return mainStat;
            var similarKeys = GetListOfSimilarKeys(key);
            var similarStat = GetAverageStat(similarKeys);
            var pow = Math.Log(mainStat.HandsTried) / Math.Log(10);
            var weightOfSimilar = 1 - pow / 4;
            var denominator = 1 + weightOfSimilar;
            var percentMainHandsWon = mainStat.HandPortionWon;
            var percentMainTricksWon = mainStat.TrickPortionWon;
            var percentSimHandsWon = similarStat.HandPortionWon;
            var percentSimTricksWon = similarStat.TrickPortionWon;
            var percentWeightedTricks = (percentMainTricksWon + percentSimTricksWon * weightOfSimilar) / denominator;
            var percentWeightedHands = (percentMainHandsWon + percentSimHandsWon * weightOfSimilar) / denominator;
            return new MoveStat()
            {
                TricksWon = percentWeightedTricks == null ? 0 : (int)(percentWeightedTricks * mainStat.TricksTried),
                TricksTried = mainStat.TricksTried,
                HandsWon = percentWeightedHands == null ? 0 : (int)(percentWeightedHands * mainStat.HandsTried),
                HandsTried = mainStat.HandsTried
            };
        }

        private List<MoveStatUniqueKey> GetListOfSimilarKeys(MoveStatUniqueKey key)
        {
            var properties = typeof(MoveStatUniqueKey).GetFields();
            var newKeys = new List<MoveStatUniqueKey>();
            var intTypes = new[] { typeof(int), typeof(Int16), typeof(Int32), typeof(Int64) };
            var nullableIntTypes = new[] { typeof(int?), typeof(Int16?), typeof(Int32?), typeof(Int64?) };
            foreach (var property in properties)
            {
                if (intTypes.Contains(property.FieldType))
                {
                    var propValue = (int)property.GetValue(key);
                    AddRangeOfValues(key, property, propValue, newKeys);
                }
                else if (nullableIntTypes.Contains(property.FieldType))
                {
                    var propValue = (int?)property.GetValue(key);
                    if (propValue != null)
                    {
                        object newKey = key;
                        property.SetValue(newKey, null);
                        if (newKey.Equals(key))
                            throw new ApplicationException();
                        newKeys.Add((MoveStatUniqueKey)newKey);
                    }
                    AddRangeOfValues(key, property, propValue, newKeys);
                }
                else if (property.FieldType == typeof(bool))
                {
                    object newKey1 = key;
                    property.SetValue(newKey1, !(bool)property.GetValue(key));
                    if (newKey1.Equals(key))
                        throw new ApplicationException();
                    newKeys.Add((MoveStatUniqueKey)newKey1);
                }
            }
            return newKeys;
        }

        private void AddRangeOfValues(MoveStatUniqueKey key, FieldInfo property, int? propValue, List<MoveStatUniqueKey> newKeys)
        {
            var range = Changes[property.Name];
            int min;
            int max;
            if (propValue == null || !range.MaxOffset.HasValue)
            {
                min = range.Min;
                max = range.Max;
            }
            else
            {
                min = (int)Math.Max((decimal)range.Min, (decimal)(propValue - range.MaxOffset.Value));
                max = (int)Math.Min((decimal)range.Max, (decimal)(propValue + range.MaxOffset.Value));
            }
            for (var i = min; i <= max; ++i)
            {
                if (i == (int?)property.GetValue(key))
                    continue;
                object newKey = key;
                property.SetValue(newKey, i);
                if (newKey.Equals(key))
                    throw new ApplicationException();
                newKeys.Add((MoveStatUniqueKey)newKey);
            }
        }
        
        private struct RangeDetail
        {
            public int Min;
            public int Max;
            public int? MaxOffset;
        }

        private Dictionary<string, RangeDetail> Changes = new Dictionary<string, RangeDetail>()
        {
            { "Picker", new RangeDetail() { Min = 1, Max = 5 } },
            { "Partner", new RangeDetail() { Min = 1, Max = 5 } },
            { "Trick", new RangeDetail() { Min = 1, Max = 6 } },
            { "MoveWithinTrick", new RangeDetail() { Min = 1, Max = 5 } },
            { "PointsAlreadyInTrick", new RangeDetail() { Min = 0, Max = 120, MaxOffset = 20 } },
            { "TotalPointsInPreviousTricks", new RangeDetail() { Min = 0, Max = 120, MaxOffset = 5 } },
            { "PointsInThisCard", new RangeDetail() { Min = 0, Max = 11, MaxOffset = 3 } },
            { "RankOfThisCard", new RangeDetail() { Min = 0, Max = 32, MaxOffset = 5 } },
            { "HigherRankingCardsPlayedPreviousTricks", new RangeDetail() { Min = 0, Max = 32, MaxOffset = 4 } },
            { "HigherRankingCardsPlayedThisTrick", new RangeDetail() { Min = 0, Max = 32, MaxOffset = 1 } }
        };

        private MoveStat GetAverageStat(IEnumerable<MoveStatUniqueKey> keys)
        {
            var tricksWon = new List<double>();
            var handsWon = new List<double>();
            foreach (var key in keys)
            {
                var stat = _repository.GetRecordedResults(key);
                if (stat != null && stat.TricksTried > 0)
                    tricksWon.Add(stat.TrickPortionWon.Value);
                if (stat != null && stat.HandsTried > 0)
                    handsWon.Add(stat.HandPortionWon.Value);
            }
            return new MoveStat()
            {
                TricksWon = tricksWon.Any() ? (int)(tricksWon.Average() * 1000) : 0,
                TricksTried = tricksWon.Any() ? 1000 : 0,
                HandsWon = handsWon.Any() ? (int)(handsWon.Average() * 1000) : 0,
                HandsTried = handsWon.Any() ? 1000 : 0
            };
        }

        public double GetWeightedScore(MoveStatUniqueKey key)
        {
            throw new NotImplementedException();
        }
    }
}