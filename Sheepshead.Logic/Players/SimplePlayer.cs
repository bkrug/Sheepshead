using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;

namespace Sheepshead.Logic.Players
{
    public class SimplePlayer : ComputerPlayer
    {
        public SimplePlayer(Participant participant) : base(participant) { }

        public override SheepCard GetMove(ITrick trick)
        {
            return this.Cards.First(c => trick.IsLegalAddition(c, this));
        }

        public override bool WillPick(IHand hand)
        {
            return QueueRankInHand(hand) == hand.PlayerCount;
        }

        protected override List<SheepCard> DropCardsForPickInternal(IHand hand)
        {
            return Cards.OrderByDescending(c => CardUtil.GetRank(c)).Take(2).ToList();
        }

        public override SheepCard? ChooseCalledAce(IHand hand)
        {
            var legalCards = LegalCalledAceSuits(hand);
            var acceptableSuits = legalCards.LegalSuits.Select(g => g.Key);
            if (!acceptableSuits.Any())
                return null;
            return GetCardOfSuit(legalCards.CardType, acceptableSuits.First());
        }
    }
}