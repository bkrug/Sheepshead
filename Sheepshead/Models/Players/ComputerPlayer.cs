using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public abstract class ComputerPlayer : Player
    {
        public abstract ICard GetMove(ITrick trick);

        public abstract bool WillPick(ITrick trick);

        protected int QueueRankInTrick(ITrick trick)
        {
            var indexOfMe = trick.Hand.Deck.Game.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Hand.Deck.Game.Players.IndexOf(trick.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += 5;
            return rank + 1;
        }
    }
}