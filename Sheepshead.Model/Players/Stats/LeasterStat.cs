using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class LeasterStat : IStat<LeasterStat>
    {
        public int HandsWon {get; set; }
        public int HandsTried { get; set; }

        [ScriptIgnore]
        public double? HandPortionWon { get { return HandsTried == 0 ? null : (double?)HandsWon / HandsTried; } }

        public void AddOtherStat(LeasterStat otherStat)
        {
            HandsWon += otherStat.HandsWon;
            HandsTried += otherStat.HandsTried;
        }
    }

    public struct LeasterStatUniqueKey : IStatUniqueKey
    {
        public bool WonOneTrick;
        public bool LostOneTrick;
        public bool MostPowerfulInTrick;
        public bool CardMatchesSuit;  //Always true for first card of the trick.
        public int OpponentPercentDone;
        public int AvgVisibleCardPoints;
        public int UnknownStrongerCards;  //Suit of the card played, not the suit of the card to start the trick.
        public int HeldStrongerCards;   //Suit of the card played, not the suit of the card to start the trick.
    }

    public class LeasterStatConsts
    {
        public static Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
        {
            { "OpponentPercentDone", new RangeDetail() { Min = 0, Max = 100, ValidValues = new List<int>() { 0, 25, 50, 75, 100 }, Precision = 25 } },
            { "AvgVisibleCardPoints", new RangeDetail() { Min = 0, Max = 11 } },
            { "UnknownStrongerCards", new RangeDetail() { Min = 0, Max = 19 } },
            { "HeldStrongerCards", new RangeDetail() { Min = 0, Max = 5 } }
        };
    }
}