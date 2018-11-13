using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public class BasicPlayer : ComputerPlayer
    {
        public override SheepCard GetMove(ITrick trick)
        {
            SheepCard moveCard;
            if (!trick.Hand.Leasters)
            {
                moveCard = TryToWinTrick(trick);
            }
            else
            {
                var previousWinners = trick.Hand.Tricks.Where(t => t != trick).Select(t => t.Winner());
                var lowestTrick = previousWinners.Any() ? previousWinners.Min(w => w.Points) : -1;
                if (previousWinners.Any(t => t.Player == this) || trick.CardsPlayed.Sum(c => CardUtil.GetPoints(c.Value)) > lowestTrick)
                    moveCard = TryToLooseTrick(trick);
                else
                    moveCard = TryToWinTrick(trick);
            }
            return moveCard;
        }

        private SheepCard TryToWinTrick(ITrick trick)
        {
            if (trick.StartingPlayer == this)
                return GetLeadCard(trick, this.Cards);
            var legalCards = Cards.Where(c => trick.IsLegalAddition(c, this));
            if (QueueRankInTrick(trick) < trick.PlayerCount)
                return GetMiddleCard(trick, legalCards);
            return GetFinishingCard(trick, legalCards);
        }

        private SheepCard TryToLooseTrick(ITrick trick)
        {
            var legalCards = Cards.Where(c => trick.IsLegalAddition(c, this));
            return legalCards.OrderByDescending(l => CardUtil.GetRank(l)).First();
        }

        protected SheepCard GetLeadCard(ITrick trick, IEnumerable<SheepCard> legalCards)
        {
            IEnumerable<SheepCard> cardsOfPreferedSuite = new List<SheepCard>();
            if (trick.Hand.Picker == this || IamPartner(trick))
                cardsOfPreferedSuite = legalCards.Where(c => CardUtil.GetSuit(c) == Suit.TRUMP).ToList();
            else
            {
                if (trick.Game.PartnerMethod == PartnerMethod.CalledAce && trick.PartnerCard.HasValue)
                    cardsOfPreferedSuite = legalCards.Where(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(trick.PartnerCard.Value));
                if (!cardsOfPreferedSuite.Any())
                    cardsOfPreferedSuite = legalCards.Where(c => CardUtil.GetSuit(c) != Suit.TRUMP).ToList();
            }
            return legalCards.OrderBy(c => cardsOfPreferedSuite.Contains(c) ? 1 : 2)
                             .OrderByDescending(c => CardUtil.GetRank(c))
                             .ThenByDescending(c => CardUtil.GetPoints(c))
                             .First();
        }

        private SheepCard GetMiddleCard(ITrick trick, IEnumerable<SheepCard> legalCards)
        {
            return legalCards.OrderByDescending(c => CardUtil.GetRank(c))
                             .ThenByDescending(c => CardUtil.GetPoints(c))
                             .First();
        }

        private SheepCard GetFinishingCard(ITrick trick, IEnumerable<SheepCard> legalCards)
        {
            var highestPlayedCard = trick.CardsPlayed.OrderByDescending(d => CardUtil.GetRank(d.Value)).First().Value;
            var winningCards = legalCards.Where(c => CardUtil.GetRank(c) > CardUtil.GetRank(highestPlayedCard)).ToList();
            return legalCards.OrderBy(c => winningCards.Contains(c) ? 1 : 2).ThenByDescending(c => CardUtil.GetRank(c)).First();
        }

        public override bool WillPick(IDeck deck)
        {
            var playerQueueRankInTrick = QueueRankInDeck(deck);
            var middleQueueRankInTrick = (deck.PlayerCount + 1) / 2;
            var trumpCount = this.Cards.Count(c => CardUtil.GetSuit(c) == Suit.TRUMP);
            var willPick = playerQueueRankInTrick > middleQueueRankInTrick && trumpCount >= 2
                || playerQueueRankInTrick == middleQueueRankInTrick && trumpCount >= 3
                || trumpCount >= 4;
            return willPick;
        }

        protected override List<SheepCard> DropCardsForPickInternal(IDeck deck)
        {
            //get a list of cards for which there are no other cards in their suite.  Exclude Trump cards.
            var soloCardsOfSuite = Cards
                .GroupBy(g => CardUtil.GetSuit(g))
                .Where(g => g.Count() == 1 && CardUtil.GetSuit(g.First()) != Suit.TRUMP)
                .Select(g => g.First()).ToList();
            return Cards.OrderBy(c => soloCardsOfSuite.Contains(c) ? 1 : 2).ThenByDescending(c => CardUtil.GetRank(c)).Take(2).ToList();
        }

        public override SheepCard? ChooseCalledAce(IDeck deck)
        {
            var acceptableSuits = LegalCalledAceSuits(deck);
            if (!acceptableSuits.LegalSuits.Any())
                return null;
            var selectedSuit = acceptableSuits.LegalSuits
                .OrderBy(g => g.Count())
                .First()
                .Key;
            return GetCardOfSuit(acceptableSuits.CardType, selectedSuit);
        }
    }
}