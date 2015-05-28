using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public class MoveStat
    {
        public int TricksWon {get; set; }
        public int TricksTried {get; set; }
        public int HandsWon {get; set; }
        public int HandsTried { get; set; }

        [ScriptIgnore]
        public double? TrickPortionWon { get { return TricksTried == 0 ? null : (double?)TricksWon / TricksTried; } }
        [ScriptIgnore]
        public double? HandPortionWon { get { return HandsTried == 0 ? null : (double?)HandsWon / HandsTried; } }
    }

    public struct MoveStatUniqueKey {
        //bool CardWillOverpower;   //Only true if this card changes the player's win state from loosing to winning
        //double OpponentPortionDone;  //If partner unknown, this won't reach 1.0 unless current player is last.
        //bool PartnerKnown;
        //bool CardPoints;          //Negative if given to opposing team
        //int UnknownStrongerCards;
        //int KnownStrongerCards;   //That have not been played

        public bool OffenseSide; //Card is played by Offense
        public bool PickerDone; //Picker already played this trick
        public bool? PartnerDone;
        public int PointsInTrick;
        public int HighestRankInTrick;
        public bool WinningSide; //Card is played by side currently winning trick
        public bool ThisCardMorePowerful; //This card is more powerful than highest card played in trick
        public int MorePowerfulUnknownCards; //Includes cards in other players' hands, or in the blind unless this is the picker.
        public int RemainingUnknownPoints; //Total points, not total cards
        public int MorePowerfulHeld;
        public int PointsHeld; //Total points, not total cards
        public int CardsHeldWithPoints;
        public int MoveIndex;
        public int TrickIndex;

        public int CentroidRoom
        {
            get
            {
                return (OffenseSide ? 1 : 0)
                    + (PickerDone ? 1 : 0) * 2
                    + (WinningSide ? 1 : 0) * 2 * 2
                    + (ThisCardMorePowerful ? 1 : 0) * 2 * 2 * 2
                    + PartnerDoneNo * 2 * 2 * 2 * 2;
            }
        }
        private int PartnerDoneNo
        {
            get
            {
                switch (PartnerDone)
                {
                    case true:
                        return 1;
                    case false:
                        return 0;
                    default:
                        return 2;
                }
            }
        }
    }
}