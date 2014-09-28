using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public abstract class ComputerPlayer : Player
    {
        public abstract ICard GetMove(ITrick trick);

        public abstract bool WillPick(IDeck deck);

        public abstract List<ICard> DropCardsForPick(IHand hand, IPlayer player);

        protected int QueueRankInTrick(ITrick trick)
        {
            var indexOfMe = trick.Hand.Deck.Game.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Hand.Deck.Game.Players.IndexOf(trick.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += trick.Hand.Deck.Game.PlayerCount;
            return rank + 1;
        }

        protected int QueueRankInDeck(IDeck deck)
        {
            var indexOfMe = deck.Game.Players.IndexOf(this);
            var indexOfStartingPlayer = deck.Game.Players.IndexOf(deck.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += deck.Game.PlayerCount;
            return rank + 1;
        }
    }
}