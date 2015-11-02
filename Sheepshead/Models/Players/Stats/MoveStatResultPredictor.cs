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
                //If OpponentPercentDone were represented as a fraction it would either have a denominator of 3 or 4.  Therefore the precision should have a denominator of 12.
                { "OpponentPercentDone", new RangeDetail() { Min = 0, Max = 100, ValidValues = new List<int>() { 0, 25, 33, 50, 67, 75, 100 }, Precision = (int)Math.Round(100.0 / 12.0) } },
                //Giving CardPoints a precision of 1 seems wasteful, but to avoid that I would need to establish some sort of key-value system for points.
                //Such a decision would hide the fact that 4 points and 10 points are very different numbers.
                { "CardPoints", new RangeDetail() { Min = -11, Max = 11, ValidValues = new List<int>() { -11, -10, -4, -3, -2, 0, 2, 3, 4, 10, 11 }, Precision = 1 } },
                { "UnknownStrongerCards", new RangeDetail() { Min = 0, Max = 19 } },
                { "HeldStrongerCards", new RangeDetail() { Min = 0, Max = 5 } }
            };
        }

        protected override Dictionary<MoveStatUniqueKey, bool> CreateKeyList()
        {
            return new Dictionary<MoveStatUniqueKey, bool>();
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