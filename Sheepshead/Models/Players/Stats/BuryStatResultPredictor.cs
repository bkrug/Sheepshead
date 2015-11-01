using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        protected override Dictionary<BuryStatUniqueKey, bool> CreateKeyList()
        {
            return new Dictionary<BuryStatUniqueKey, bool>();
        }

        protected override BuryStat CreateStat()
        {
            return new BuryStat();
        }

        protected override BuryStatUniqueKey CreateKey(BuryStatUniqueKey originalKey, Stack<int> keyValues)
        {
            var list = keyValues.ToList();
            return new BuryStatUniqueKey()
            {
                BuriedPoints = list[3],
                AvgPointsInHand = list[2],
                AvgRankInHand = list[1],
                SuitsInHand = list[0]
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