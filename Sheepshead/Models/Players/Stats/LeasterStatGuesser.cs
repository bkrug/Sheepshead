using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Sheepshead.Models.Players.Stats
{
    public interface ILeasterStatGuesser : IGuesser<LeasterStatUniqueKey, LeasterStat>
    {
    }

    /// <summary>
    /// Class is incomplete. Not sure if I want to use this, or not.
    /// </summary>
    public class LeasterStatGuesser : Guesser<LeasterStatUniqueKey, LeasterStat>, ILeasterStatGuesser
    {
        protected override void SetRangeValues()
        {
            MaxRanges = LeasterStatConsts.MaxRanges;
            ReverseValue = new List<string>() { "AvgRankInHand", "SuitsInHand" };
        }

        protected override LeasterStatUniqueKey CreateKeyInstance()
        {
            return new LeasterStatUniqueKey();
        }

        protected override LeasterStat CreateStatInstance()
        {
            return new LeasterStat();
        }

        protected override LeasterStat CreateResult(LeasterStatUniqueKey key)
        {
            var points = CreateImaginaryScore(key);
            var handsPicked = 5;
            return new LeasterStat()
            {
                HandsWon = (int)Math.Round(points * handsPicked),
                HandsTried = handsPicked
            };
        }

        private double CreateImaginaryScore(LeasterStatUniqueKey key)
        {
            var normalized = NormalizeKeyValuesAsDictionary(ref key);
            double weightOpponentPercent = 3;
            double weightAvgVisible = 2;
            double weightUnknownStronger = 3;
            double weightHeldStronger = 3;
            var normalScore =
                    (normalized["OpponentPercentDone"] * weightOpponentPercent
                        + normalized["AvgVisibleCardPoints"] * weightAvgVisible
                        + normalized["UnknownStrongerCards"] * weightUnknownStronger
                        + normalized["HeldStrongerCards"] * weightHeldStronger)
                    / (weightOpponentPercent + weightAvgVisible + weightUnknownStronger + weightHeldStronger);
            return normalScore;
        }

        public override LeasterStat GetGuess(LeasterStatUniqueKey key)
        {
            if (!key.WonOneTrick && !key.LostOneTrick)
            {
                //first turn
                //Try to win with few points, or loose with many.
            }
            if (key.WonOneTrick && key.LostOneTrick)
            { 
                //try to loose
            }
            if (!key.WonOneTrick && key.LostOneTrick)
            {
                //try to win with few points
            }
            if (key.WonOneTrick && !key.LostOneTrick)
            {
                //If this is the second turn, try to loose
                //If this is the third or later turn, try to win.
            }
            return null;
        }
    }
}
