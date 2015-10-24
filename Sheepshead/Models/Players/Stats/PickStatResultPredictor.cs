using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public class PickStatConst
    {
        public static Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
            {
                { "TrumpCount", new RangeDetail() { Min = 0, Max = 6  } },
                { "AvgTrumpRank", new RangeDetail() { Min = 1, Max = 14 } },
                { "PointsInHand", new RangeDetail() { Min = 0, Max = 64 } },
                { "TotalCardsWithPoints", new RangeDetail() { Min = 0, Max = 6 } }
            };
    }

    public interface IPickResultPredictor
    {
        PickStat GetWeightedStat(PickStatUniqueKey key);
    }

    public class PickStatResultPredictor : ResultPredictor<PickStatUniqueKey, PickStat>, IPickResultPredictor
    {
        IPickStatRepository _repository;
        IPickStatGuessRepository _guessRepository;

        public PickStatResultPredictor(IPickStatRepository repository, IPickStatGuessRepository guessRepository) : base(repository)
        {
            _repository = repository;
            _guessRepository = guessRepository;
            MaxRanges = PickStatConst.MaxRanges;
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

        public override PickStat GetWeightedStat(PickStatUniqueKey key)
        {
            var stat = base.GetWeightedStat(key);
            var guessStat = _guessRepository.GetRecordedResults(key);
            stat.AddOtherStat(guessStat);
            return stat;
        }
    }
}