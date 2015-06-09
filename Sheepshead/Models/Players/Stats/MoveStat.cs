using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class MoveStat : IHandStat
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

    public struct MoveStatUniqueKey
    {
        public bool CardWillOverpower;   //Only true if this card changes the player's win state from loosing to winning
        public int OpponentPercentDone;  //If partner unknown, this won't reach 1.0 unless current player is last.
        public int CardPoints;           //Negative if given to opposing team
        public int UnknownStrongerCards;
        public int HeldStrongerCards;   //That have not been played
    }
}