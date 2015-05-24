using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players
{
    public class BasicPlayer : ComputerPlayer
    {
        public event EventHandler<OnMoveEventArgs> OnMove;
        public class OnMoveEventArgs : EventArgs
        {
            public ITrick Trick;
            public ICard Card;
        }

        public override ICard GetMove(ITrick trick)
        {
            if (!trick.Hand.Leasters)
            {
                return TryToWinTrick(trick);
            }
            else
            {
                var previousWinners = trick.Hand.Tricks.Where(t => t != trick).Select(t => t.Winner());
                var lowestTrick = previousWinners.Any() ? previousWinners.Min(w => w.Points) : -1;
                if (previousWinners.Any(t => t.Player == this) || trick.CardsPlayed.Sum(c => c.Value.Points) > lowestTrick)
                    return TryToLooseTrick(trick);
                else
                    return TryToWinTrick(trick);
            }
        }

        private ICard TryToWinTrick(ITrick trick)
        {
            if (trick.StartingPlayer == this)
                return GetLeadCard(trick, this.Cards);
            var legalCards = Cards.Where(c => trick.IsLegalAddition(c, this));
            if (QueueRankInTrick(trick) < trick.PlayerCount)
                return GetMiddleCard(trick, legalCards);
            return GetFinishingCard(trick, legalCards);
        }

        private ICard TryToLooseTrick(ITrick trick)
        {
            var legalCards = Cards.Where(c => trick.IsLegalAddition(c, this));
            return legalCards.OrderByDescending(l => l.Rank).First();
        }

        private ICard GetLeadCard(ITrick trick, IEnumerable<ICard> legalCards)
        {
            IEnumerable<ICard> cardsOfPreferedSuite;
            if (trick.Hand.Picker == this || this.Cards.Any(c => c.StandardSuite == trick.Hand.PartnerCard.StandardSuite && c.CardType == trick.Hand.PartnerCard.CardType))
                cardsOfPreferedSuite = legalCards.Where(c => CardRepository.GetSuite(c) == Suite.TRUMP).ToList();
            else
                cardsOfPreferedSuite = legalCards.Where(c => CardRepository.GetSuite(c) != Suite.TRUMP).ToList();
            return legalCards.OrderBy(c => cardsOfPreferedSuite.Contains(c) ? 1 : 2)
                             .OrderByDescending(c => c.Rank)
                             .ThenByDescending(c => c.Points)
                             .First();
        }

        private ICard GetMiddleCard(ITrick trick, IEnumerable<ICard> legalCards)
        {
            return legalCards.OrderByDescending(c => c.Rank)
                             .ThenByDescending(c => c.Points)
                             .First();
        }

        private ICard GetFinishingCard(ITrick trick, IEnumerable<ICard> legalCards)
        {
            var highestPlayedCard = trick.CardsPlayed.OrderByDescending(d => d.Value.Rank).First().Value;
            var winningCards = legalCards.Where(c => c.Rank > highestPlayedCard.Rank).ToList();
            return legalCards.OrderBy(c => winningCards.Contains(c) ? 1 : 2).ThenByDescending(c => c.Rank).First();
        }

        public override bool WillPick(IDeck deck)
        {
            var middleQueueRankInTrick = (deck.Game.PlayerCount + 1) / 2;
            var trumpCount = this.Cards.Count(c => CardRepository.GetSuite(c) == Suite.TRUMP);
            return QueueRankInDeck(deck) > middleQueueRankInTrick && trumpCount >= 2
                || QueueRankInDeck(deck) == middleQueueRankInTrick && trumpCount >= 3
                || trumpCount >= 4;
        }

        protected override List<ICard> DropCardsForPickInternal(IDeck deck)
        {
            //get a list of cards for which there are no other cards in their suite.  Exclude Trump cards.
            var soloCardsOfSuite = Cards
                .GroupBy(g => CardRepository.GetSuite(g))
                .Where(g => g.Count() == 1 && CardRepository.GetSuite(g.First()) != Suite.TRUMP)
                .Select(g => g.First()).ToList();
            return Cards.OrderBy(c => soloCardsOfSuite.Contains(c) ? 1 : 2).ThenByDescending(c => c.Rank).Take(2).ToList();
        }

        protected virtual void OnMoveHandler(ITrick trick, ICard card)
        {
            var e = new OnMoveEventArgs()
            {
                Trick = trick,
                Card = card
            };
            if (OnMove != null)
                OnMove(this, e);
        }
    }
}