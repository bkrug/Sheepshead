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
            return Cards.OrderByDescending(c => CardUtil.GetRank(c)).Take(2).ToList();
        }

        public override SheepCard? ChooseCalledAce(IDeck deck)
        {
            var acceptableSuits = LegalCalledAceSuits(deck).Select(g => g.Key);
            if (!acceptableSuits.Any())
                return null;
            var selectedSuit = acceptableSuits.First();
            switch (selectedSuit)
            {
                case Suit.CLUBS:
                    return SheepCard.ACE_CLUBS;
                case Suit.HEARTS:
                    return SheepCard.ACE_HEARTS;
                case Suit.SPADES:
                default:
                    return SheepCard.ACE_SPADES;
            }
        }
    }
}