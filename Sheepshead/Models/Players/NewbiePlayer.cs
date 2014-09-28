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

        public override bool WillPick(IDeck deck)
        {
            return QueueRankInDeck(deck) == deck.Game.PlayerCount;
        }

        public override List<ICard> DropCardsForPick(IHand hand, IPlayer player)
        {
            return player.Cards.OrderByDescending(c => c.Rank).Take(2).ToList();
        }
    }
}