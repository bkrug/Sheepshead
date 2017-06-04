using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class MoveStat : IStat<MoveStat>
    {
        public int TricksWon {get; set; }
        public int TricksTried {get; set; }
        public int HandsWon {get; set; }
        public int HandsTried { get; set; }

        [ScriptIgnore]
        public double? TrickPortionWon { get { return TricksTried == 0 ? null : (double?)TricksWon / TricksTried; } }
        [ScriptIgnore]
        public double? HandPortionWon { get { return HandsTried == 0 ? null : (double?)HandsWon / HandsTried; } }

        public void AddOtherStat(MoveStat otherStat)
        {
            TricksWon += otherStat.TricksWon;
            TricksTried += otherStat.TricksTried;
            HandsWon += otherStat.HandsWon;
            HandsTried += otherStat.HandsTried;
        }
    }

    public struct MoveStatUniqueKey : IStatUniqueKey
    {
        public bool CardWillOverpower;   //Only true if this card changes the player's win state from loosing to winning
        public int OpponentPercentDone;  //If partner unknown, this won't reach 1.0 unless current player is last.
        public int CardPoints;           //Negative if given to opposing team
        public int UnknownStrongerCards;
        public int HeldStrongerCards;   //That have not been played
    }

    public class MoveStatConsts
    {
        public static Dictionary<string, RangeDetail> MaxRanges = new Dictionary<string, RangeDetail>()
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
}