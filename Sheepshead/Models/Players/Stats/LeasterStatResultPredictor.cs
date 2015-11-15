using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface ILeasterResultPredictor
    {
        LeasterStat GetWeightedStat(LeasterStatUniqueKey key);
    }

    public class LeasterStatResultPredictor : ResultPredictor<LeasterStatUniqueKey, LeasterStat>, ILeasterResultPredictor
    {
        ILeasterStatRepository _repository;

        public LeasterStatResultPredictor(ILeasterStatRepository repository) : base(repository)
        {
            _repository = repository;
            MaxRanges = LeasterStatConsts.MaxRanges;
        }

        protected override LeasterStat CreateStat()
        {
            return new LeasterStat();
        }

        protected override LeasterStatUniqueKey CreateKey(LeasterStatUniqueKey originalKey, List<int> keyValues)
        {
            return new LeasterStatUniqueKey()
            {
                WonOneTrick = originalKey.WonOneTrick,
                LostOneTrick = originalKey.LostOneTrick,
                MostPowerfulInTrick = originalKey.MostPowerfulInTrick,
                CardMatchesSuit = originalKey.CardMatchesSuit,
                OpponentPercentDone = keyValues[0],
                AvgVisibleCardPoints = keyValues[1],
                UnknownStrongerCards = keyValues[2],
                HeldStrongerCards = keyValues[3]
            };
        }

        private const int MINIMUM_TRIES = 1000;

        protected override bool ReachedMinimumTries(LeasterStat generatedStat)
        {
            return generatedStat.HandsTried >= MINIMUM_TRIES;
        }
    }
}