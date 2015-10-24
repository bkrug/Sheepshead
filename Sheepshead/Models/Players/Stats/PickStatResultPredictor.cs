﻿using System;
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

    public class PickStatResultPredictor : ResultPredictor<PickStatUniqueKey, PickStat>, IPickResultPredictor
    {
        IPickStatRepository _repository;

        //TODO: Add a second repository parameter containing guess data
        public PickStatResultPredictor(IPickStatRepository repository) : base(repository)
        {
            _repository = repository;
            MaxRanges = new Dictionary<string, RangeDetail>()
            {
                { "TrumpCount", new RangeDetail() { Min = 0, Max = 6  } },
                { "AvgTrumpRank", new RangeDetail() { Min = 1, Max = 14 } },
                { "PointsInHand", new RangeDetail() { Min = 0, Max = 64 } },
                { "TotalCardsWithPoints", new RangeDetail() { Min = 0, Max = 6 } }
            };
        }

        protected override List<PickStatUniqueKey> CreateKeyList()
        {
            return new List<PickStatUniqueKey>();
        }

        protected override PickStat CreateStat()
        {
            return new PickStat();
        }

        private const int MINIMUM_TRIES = 1000;

        protected override bool ReachedMinimumTries(PickStat generatedStat)
        {
            return generatedStat.HandsPicked >= MINIMUM_TRIES && generatedStat.HandsPassed >= MINIMUM_TRIES;
        }

        protected override PickStat GetRecordedResults(PickStatUniqueKey key)
        {
            return _repository.GetRecordedResults(key);
        }

        protected override void AddOtherStat(PickStat stat, PickStat recordedStat)
        {
            stat.AddOtherStat(recordedStat);
        }

        //public PickStat GetWeightedStat(PickStatUniqueKey key)
        //{
        //    var normalizedValues = new List<double>();
        //    foreach (var prop in typeof(PickStatUniqueKey).GetFields())
        //    {
        //        var value = (int)prop.GetValue(key);
        //        var range = MaxRanges[prop.Name];
        //        var normalized = ((double)value - range.Min) / (range.Max - range.Min);
        //        if (prop.Name == "AvgTrumpRank")
        //            normalized = 1 - normalized;
        //        normalizedValues.Add(normalized);
        //    }
        //    var avg = normalizedValues.Average();
        //    var passPercent = avg;
        //    var pickPercent = Math.Sin(avg * Math.PI / 2 - Math.PI / 2) + 1;
        //    var passScore = 4 * passPercent - 2;
        //    var pickScore = 6 * pickPercent - 3;
        //    var handsTried = 5;
        //    return new PickStat()
        //    {
        //        TotalPassedPoints = (int)Math.Round(passScore * handsTried),
        //        HandsPassed = handsTried,
        //        TotalPickPoints = (int)Math.Round(pickScore * handsTried),
        //        HandsPicked = handsTried
        //    };
        //}
    }
}