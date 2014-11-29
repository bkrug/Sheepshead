using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class MoveStat
    {
        public int TricksWon {get; set; }
        public int TricksTried {get; set; }
        public int GamesWon {get; set; }
        public int GamesTried { get; set; }

        public double? TrickPercentWon { get { return TricksTried == 0 ? null : (double?)TricksWon / TricksTried; } }
        public double? GamePercentWon { get { return GamesTried == 0 ? null : (double?)GamesWon / GamesTried; } }
    }

    public struct MoveStatUniqueKey {
        public int Picker {get;set;}
        public int? Partner {get;set;}
        public int Trick {get;set;}
        public int MoveWithinTrick {get; set;}
        public int TrumpPlayedPreviousTricks {get; set;}
        public int TrumpPlayedIncludingCurrentTrick {get;set;}
        public int PointsAlreadyInTrick {get;set;}
        public int TotalPointsInPreviousTricks {get;set;}

        public int CardRankPlayed { get; set; }
        public int CardPowerPlayed { get; set; }
        public bool PartnerCard { get; set; }
        public bool CardWasHighestRankWhenPlayed {get;set;}
        public int HigherRankingCardsPlayedPreviousTricks {get; set;}
        public int HigherRankingCardsPlayedIncludingThisTrick {get; set;}
    }
}