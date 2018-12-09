using System;
using System.Collections.Generic;

namespace Sheepshead.Model
{
    public class TurnState
    {
        public Guid GameId { get; set; }
        public TurnType TurnType { get; set; }
        public IHand Hand { get; set; }
    }

    public enum TurnType
    {
        BeginHand, Pick, Bury, PlayTrick
    }

    /// <summary>
    /// Like TurnState, but serializable.
    /// </summary>
    public class PlayState
    {
        public string TurnType { get; set; }
        public bool HumanTurn { get; set; }
        public string CurrentTurn { get; set; }
        public bool RequestingPlayerTurn { get; set; }
        public List<CardSummary> Blinds { get; set; }
        //Player name and whether or not the picked
        public List<Tuple<string, bool>> PickChoices { get; set; }
        //Player name and card filename
        public List<List<Tuple<string, CardSummary>>> CardsPlayed { get; set; }
        //Filename number for the player's cards.
        public List<CardSummary> PlayerCards { get; set; }
    }

    public class PickState
    {
        public string TurnType { get; set; }
        public bool HumanTurn { get; set; }
        public string CurrentTurn { get; set; }
        public bool RequestingPlayerTurn { get; set; }
        //Player name and whether or not the picked
        public List<Tuple<string, bool>> PickChoices { get; set; }
        //Filename number for the player's cards.
        public List<CardSummary> PlayerCards { get; set; }
        public bool MustRedeal { get; set; }
    }

    public class BuryState
    {
        public string TurnType { get; set; }
        public bool RequestingPlayerTurn { get; set; }
        public List<CardSummary> Blinds { get; set; }
        //Filename number for the player's cards.
        public List<CardSummary> PlayerCards { get; set; }
        public List<CardSummary> LegalCalledAces { get; set; }
        public string PartnerMethod { get; set; }
    }
}