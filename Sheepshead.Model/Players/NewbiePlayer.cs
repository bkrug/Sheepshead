using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public class NewbiePlayer : ComputerPlayer
    {
        public override SheepCard GetMove(ITrick trick)
        {
            return this.Cards.First(c => trick.IsLegalAddition(c, this));
        }

        public override bool WillPick(IDeck deck)
        {
            return QueueRankInDeck(deck) == deck.PlayerCount;
        }

        protected override List<SheepCard> DropCardsForPickInternal(IDeck deck)
        {
            return Cards.OrderByDescending(c => CardRepository.GetRank(c)).Take(2).ToList();
        }
    }
}