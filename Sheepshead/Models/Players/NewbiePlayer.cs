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
            return QueueRankInTrick(trick) == trick.Hand.Deck.Game.PlayerCount;
        }

        public override List<ICard> DropCardsForPick(IHand hand, IPlayer player)
        {
            return player.Cards.OrderByDescending(c => c.Rank).Take(2).ToList();
        }
    }
}