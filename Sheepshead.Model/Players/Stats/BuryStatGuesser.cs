using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface IBuryStatGuesser : IGuesser<BuryStatUniqueKey, BuryStat>
    {
    }

    public class BuryStatGuesser : Guesser<BuryStatUniqueKey, BuryStat>, IBuryStatGuesser
    {
        protected override void SetRangeValues()
        {
            MaxRanges = BuryStatConst.MaxRanges;
            ReverseValue = new List<string>() { "AvgRankInHand", "SuitsInHand" };
        }

        protected override BuryStatUniqueKey CreateKeyInstance()
        {
            return new BuryStatUniqueKey();
        }

        protected override BuryStat CreateStatInstance()
        {
            return new BuryStat();
        }

        protected override BuryStat CreateResult(BuryStatUniqueKey key)
        {
            var points = CreateImaginaryScore(key);
            var handsPicked = 5;
            return new BuryStat()
            {
                TotalPoints = (int)Math.Round(points * handsPicked),
                HandsPicked = handsPicked
            };
        }

        private double CreateImaginaryScore(BuryStatUniqueKey key)
        {
            var normalized = NormalizeKeyValuesAsDictionary(ref key);
            double weightBuriedPoints = 3;
            double weightPointsInHand = 2;
            double weightAvgRankInHand = 3;
            double weightSuitsInHand = 3;
            var normalScore = 
                    (normalized["BuriedPoints"] * weightBuriedPoints
                        + normalized["AvgPointsInHand"] * weightPointsInHand
                        + normalized["AvgRankInHand"] * weightAvgRankInHand
                        + normalized["SuitsInHand"] * weightSuitsInHand)
                    / (weightBuriedPoints + weightPointsInHand + weightAvgRankInHand + weightSuitsInHand);
            //The normal score is between 0 and 1, but we want a score between -3 and 3
            return normalScore * 6 - 3;
        }
    }
}
