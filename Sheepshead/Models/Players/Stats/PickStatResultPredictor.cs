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
        IPickStatGuesser _guessRepository;

        public PickStatResultPredictor(IPickStatRepository repository, IPickStatGuesser guessRepository) : base(repository)
        {
            _repository = repository;
            _guessRepository = guessRepository;
            MaxRanges = PickStatConst.MaxRanges;
        }

        protected override PickStat CreateStat()
        {
            return new PickStat();
        }

        protected override PickStatUniqueKey CreateKey(PickStatUniqueKey originalKey, List<int> keyValues)
        {
            var list = keyValues.ToList();
            return new PickStatUniqueKey()
            {
                TrumpCount = list[0],
                AvgTrumpRank = list[1],
                PointsInHand = list[2],
                TotalCardsWithPoints = list[3]
            };
        }

        private const int MINIMUM_TRIES = 1000;

        protected override bool ReachedMinimumTries(PickStat generatedStat)
        {
            return generatedStat.HandsPicked >= MINIMUM_TRIES && generatedStat.HandsPassed >= MINIMUM_TRIES;
        }

        public override PickStat GetWeightedStat(PickStatUniqueKey key)
        {
            var stat = base.GetWeightedStat(key);
            var guessStat = _guessRepository.GetGuess(key);
            stat.AddOtherStat(guessStat);
            return stat;
        }
    }
}