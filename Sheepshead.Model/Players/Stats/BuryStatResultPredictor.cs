using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface IBuryResultPredictor
    {
        BuryStat GetWeightedStat(BuryStatUniqueKey key);
    }

    public class BuryStatResultPredictor : ResultPredictor<BuryStatUniqueKey, BuryStat>, IBuryResultPredictor
    {
        IBuryStatRepository _repository;
        IBuryStatGuesser _guesser;

        public BuryStatResultPredictor(IBuryStatRepository repository, IBuryStatGuesser guessRepository) : base(repository)
        {
            _repository = repository;
            _guesser = guessRepository;
            MaxRanges = BuryStatConst.MaxRanges;
        }

        protected override BuryStat CreateStat()
        {
            return new BuryStat();
        }

        protected override BuryStatUniqueKey CreateKey(BuryStatUniqueKey originalKey, List<int> keyValues)
        {
            var list = keyValues.ToList();
            return new BuryStatUniqueKey()
            {
                BuriedPoints = list[0],
                AvgPointsInHand = list[1],
                AvgRankInHand = list[2],
                SuitsInHand = list[3]
            };
        }

        private const int MINIMUM_TRIES = 1000;

        protected override bool ReachedMinimumTries(BuryStat generatedStat)
        {
            return generatedStat.HandsPicked >= MINIMUM_TRIES;
        }

        public override BuryStat GetWeightedStat(BuryStatUniqueKey key)
        {
            var stat = base.GetWeightedStat(key);
            var guessStat = _guesser.GetGuess(key);
            stat.AddOtherStat(guessStat);
            return stat;
        }
    }
}