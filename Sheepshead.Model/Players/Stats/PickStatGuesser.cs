using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using Sheepshead.Models.Wrappers;
using System.Web.Script.Serialization;
using System.Timers;

namespace Sheepshead.Models.Players.Stats
{
    public interface IPickStatGuesser : IGuesser<PickStatUniqueKey, PickStat>
    {
    }

    public class PickStatGuesser : Guesser<PickStatUniqueKey, PickStat>, IPickStatGuesser
    {
        protected override void SetRangeValues()
        {
            MaxRanges = PickStatConst.MaxRanges;
            ReverseValue = new List<string>() { "AvgTrumpRank" };
        }

        protected override PickStatUniqueKey CreateKeyInstance()
        {
            return new PickStatUniqueKey();
        }

        protected override PickStat CreateStatInstance()
        {
            return new PickStat();
        }

        protected override PickStat CreateResult(PickStatUniqueKey key)
        {
            var normalizedValues = NormalizeKeyValues(ref key);
            double passScore;
            double pickScore;
            CreateImaginaryScores(normalizedValues, out passScore, out pickScore);
            var handsTried = 5;
            return new PickStat()
            {
                TotalPassedPoints = (int)Math.Round(passScore * handsTried),
                HandsPassed = handsTried,
                TotalPickPoints = (int)Math.Round(pickScore * handsTried),
                HandsPicked = handsTried
            };
        }

        private static void CreateImaginaryScores(List<double> normalizedValues, out double passScore, out double pickScore)
        {
            var avg = normalizedValues.Average();
            var passPercent = avg;
            var pickPercent = Math.Sin(avg * Math.PI - Math.PI / 2) / 2 + 0.5;
            const double maxPossiblePassPoints = 2;
            const double maxPossiblePickPoints = 3;
            passScore = 2 * maxPossiblePassPoints * passPercent - maxPossiblePassPoints;
            pickScore = 2 * maxPossiblePickPoints * pickPercent - maxPossiblePickPoints;
        }
    }
}
