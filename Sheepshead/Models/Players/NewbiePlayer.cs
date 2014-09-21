using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class NewbiePlayer : ComputerPlayer
    {
        public override ICard GetMove(ITrick trick)
        {
            return this.Cards.First(c => trick.IsLegalAddition(c, this));
        }

        public override bool WillPick(ITrick trick)
        {
            var indexOfMe = trick.Hand.Deck.Game.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Hand.Deck.Game.Players.IndexOf(trick.StartingPlayer);
            return (indexOfStartingPlayer == 0 && trick.Hand.Deck.Game.Players.Last() == this)
                || (indexOfMe + 1 == indexOfStartingPlayer);
        }
    }
}