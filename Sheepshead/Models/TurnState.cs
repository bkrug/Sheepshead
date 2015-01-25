using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public class TurnState
    {
        public IHumanPlayer HumanPlayer { get; set; }
        public TurnType TurnType { get; set; }
        public IDeck Deck { get; set; }
    }

    public enum TurnType
    {
        BeginDeck, Pick, Bury, PlayTrick
    }
}