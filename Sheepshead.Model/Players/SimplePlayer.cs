using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Model.Players
{
    public class SimplePlayer : ComputerPlayer
    {
        public override SheepCard GetMove(ITrick trick)
        {
            return this.Cards.First(c => trick.IsLegalAddition(c, this));
        }

        public override bool WillPick(IHand deck)
        {
            return QueueRankInDeck(deck) == deck.PlayerCount;
        }

        protected override List<SheepCard> DropCardsForPickInternal(IHand deck)
        {
            return Cards.OrderByDescending(c => CardUtil.GetRank(c)).Take(2).ToList();
        }

        public override SheepCard? ChooseCalledAce(IHand deck)
        {
            var legalCards = LegalCalledAceSuits(deck);
            var acceptableSuits = legalCards.LegalSuits.Select(g => g.Key);
            if (!acceptableSuits.Any())
                return null;
            return GetCardOfSuit(legalCards.CardType, acceptableSuits.First());
        }
    }
}