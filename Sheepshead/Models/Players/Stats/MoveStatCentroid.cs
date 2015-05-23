using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public struct MoveStatCentroid
    {
        public double PointsInTrick;
        public double HighestRankInTrick;
        public double MorePowerfulUnknownCards; //Includes cards in other players' hands, or in the blind unless this is the picker.
        public double RemainingUnknownPoints; //Total points, not total cards
        public double MorePowerfulHeld;
        public double PointsHeld; //Total points, not total cards
        public double CardsHeldWithPoints;
        public double MoveIndex;
        public double TrickIndex;
    }
}