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
    }
}