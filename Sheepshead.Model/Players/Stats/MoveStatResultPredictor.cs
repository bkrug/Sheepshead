using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface IStatResultPredictor
    {
        MoveStat GetWeightedStat(MoveStatUniqueKey key);
    }

    public class MoveStatResultPredictor : ResultPredictor<MoveStatUniqueKey, MoveStat>, IStatResultPredictor
    {
        IMoveStatRepository _repository;

        public MoveStatResultPredictor(IMoveStatRepository repository) : base(repository)
        {
            _repository = repository;
            MaxRanges = MoveStatConsts.MaxRanges;
        }

        protected override MoveStat CreateStat()
        {
            return new MoveStat();
        }

        protected override MoveStatUniqueKey CreateKey(MoveStatUniqueKey originalKey, List<int> keyValues)
        {
            var list = keyValues.ToList();
            return new MoveStatUniqueKey()
            {
                OpponentPercentDone = list[0],
                CardPoints = list[1],
                UnknownStrongerCards = list[2],
                HeldStrongerCards = list[3],
                CardWillOverpower = originalKey.CardWillOverpower
            };
        }

        private const int MINIMUM_TRIES = 1000;

        protected override bool ReachedMinimumTries(MoveStat generatedStat)
        {
            return generatedStat.HandsTried >= MINIMUM_TRIES;
        }
    }
}