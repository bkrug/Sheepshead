using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            MaxRanges = new Dictionary<string, RangeDetail>()
            {
                { "OpponentPercentDone", new RangeDetail() { Min = 0, Max = 100, ValidValues = new List<int>() { 0, 25, 33, 50, 67, 75, 100 } } },
                { "CardPoints", new RangeDetail() { Min = -11, Max = 11, ValidValues = new List<int>() { -11, -10, -4, -3, -2, 0, 2, 3, 4, 10, 11 } } },
                { "UnknownStrongerCards", new RangeDetail() { Min = 0, Max = 19 } },
                { "HeldStrongerCards", new RangeDetail() { Min = 0, Max = 5 } }
            };
        }

        protected override List<MoveStatUniqueKey> CreateKeyList()
        {
            return new List<MoveStatUniqueKey>();
        }

        protected override MoveStat CreateStat()
        {
            return new MoveStat();
        }

        private const int MINIMUM_TRIES = 1000;

        protected override bool ReachedMinimumTries(MoveStat generatedStat)
        {
            return generatedStat.HandsTried >= MINIMUM_TRIES;
        }
    }
}