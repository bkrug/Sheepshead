using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Models.Players
{
    public class AdvancedPlayer : BasicPlayer
    {
        public override bool WillPick(IDeck deck)
        {
            var highPointCards = Cards.Count(c => CardUtil.GetPoints(c) >= 10);
            var avgRank = Cards.Average(c => CardUtil.GetRank(c));
            var playerQueueRankInTrick = QueueRankInDeck(deck);
            var middleQueueRankInTrick = (deck.PlayerCount + 1) / 2;

            if (deck.PlayerCount == 5)
            {
                var willPick = avgRank <= 6
                    || avgRank <= 13 && highPointCards > 2
                    || avgRank <= 13 && playerQueueRankInTrick > middleQueueRankInTrick;
                return willPick;
            }
            else
            {
                var willPick = avgRank <= 8
                    || avgRank <= 16 && highPointCards > 2
                    || avgRank <= 16 && playerQueueRankInTrick > middleQueueRankInTrick;
                return willPick;
            }
        }

        public override SheepCard GetMove(ITrick trick)
        {
            SheepCard moveCard;
            if (!trick.Hand.Leasters)
            {
                if (trick.StartingPlayer == this)
                    moveCard = GetLeadMove(trick);
                else
                    moveCard = (SheepCard)0;
            }
            else
            {
                //TODO: Be more selective about which trick you want to win.
                var previousWinners = trick.Hand.Tricks.Where(t => t != trick).Select(t => t.Winner());
                var lowestTrick = previousWinners.Any() ? previousWinners.Min(w => w.Points) : -1;
                if (previousWinners.Any(t => t.Player == this) || trick.CardsPlayed.Sum(c => CardUtil.GetPoints(c.Value)) > lowestTrick)
                    moveCard = TryToLooseTrick(trick);
                else
                    moveCard = TryToWinTrick(trick);
            }
            return moveCard;
        }

        private SheepCard GetLeadMove(ITrick trick)
        {
            if (trick.Hand.Picker == this || IamPartner(trick))
            {
                if (Cards.Average(c => CardUtil.GetRank(c)) > 10)
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) == Suit.TRUMP ? 1 : 2)
                        .ThenByDescending(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
                else
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) == Suit.TRUMP ? 1 : 2)
                        .ThenBy(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
            }
            else
            {
                if (trick.Hand.Deck.Game.PartnerMethod == PartnerMethod.CalledAce && trick.Hand.PartnerCard.HasValue)
                {
                    var partnerCardSuit = CardUtil.GetSuit(trick.Hand.PartnerCard.Value);
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) == partnerCardSuit ? 1 : 2)
                        .ThenByDescending(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
                }
                else
                {
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) != Suit.TRUMP ? 1 : 2)
                        .ThenByDescending(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
                }
            }
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

        //TODO: What if all of my opponents have already played?
        //That will change my strategy.
        private SheepCard GetMiddleCard(ITrick trick, IEnumerable<SheepCard> legalCards)
        {
            return legalCards.OrderByDescending(c => CardUtil.GetRank(c))
                             .ThenByDescending(c => CardUtil.GetPoints(c))
                             .First();
        }

        //TODO: What if I already know that my side is winning?
        //I can just give away points then instead of trying to win the tick.
        private SheepCard GetFinishingCard(ITrick trick, IEnumerable<SheepCard> legalCards)
        {
            var highestPlayedCard = trick.CardsPlayed.OrderByDescending(d => CardUtil.GetRank(d.Value)).First().Value;
            var winningCards = legalCards.Where(c => CardUtil.GetRank(c) > CardUtil.GetRank(highestPlayedCard)).ToList();
            return legalCards.OrderBy(c => winningCards.Contains(c) ? 1 : 2).ThenByDescending(c => CardUtil.GetRank(c)).First();
        }

        //TODO: This player should, under certain circumstances, bury high-point cards
        protected override List<SheepCard> DropCardsForPickInternal(IDeck deck)
        {
            //get a list of cards for which there are no other cards in their suite.  Exclude Trump cards.
            var soloCardsOfSuite = Cards
                .GroupBy(g => CardUtil.GetSuit(g))
                .Where(g => g.Count() == 1 && CardUtil.GetSuit(g.First()) != Suit.TRUMP)
                .Select(g => g.First()).ToList();
            return Cards.OrderBy(c => soloCardsOfSuite.Contains(c) ? 1 : 2).ThenByDescending(c => CardUtil.GetRank(c)).Take(2).ToList();
        }
    }
}