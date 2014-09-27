using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class BasicPlayer : ComputerPlayer
    {
        public override ICard GetMove(ITrick trick)
        {
            if (trick.StartingPlayer == this)
                return GetLeadCard(trick, this.Cards);
            var legalCards = Cards.Where(c => trick.IsLegalAddition(c, this));
            if (QueueRankInTrick(trick) < trick.Hand.Deck.Game.PlayerCount)
                return GetMiddleCard(trick, legalCards);
            return GetFinishingCard(trick, legalCards);
        }

        private ICard GetLeadCard(ITrick trick, IEnumerable<ICard> legalCards)
        {
            IEnumerable<ICard> cardsOfPreferedSuite;
            if (trick.StartingPlayer == this || this.Cards.Any(c => c.StandardSuite == trick.Hand.PartnerCard.StandardSuite || c.CardType == trick.Hand.PartnerCard.CardType))
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

        public override bool WillPick(ITrick trick)
        {
            var middleQueueRankInTrick = (trick.Hand.Deck.Game.PlayerCount + 1) / 2;
            var trumpCount = this.Cards.Count(c => CardRepository.GetSuite(c) == Suite.TRUMP);
            return QueueRankInTrick(trick) == trick.Hand.Deck.Game.PlayerCount
                || QueueRankInTrick(trick) > middleQueueRankInTrick && trumpCount >= 2
                || QueueRankInTrick(trick) == middleQueueRankInTrick && trumpCount >= 3
                || trumpCount >= 4;
        }

        public override List<ICard> DropCardsForPick(IHand hand, IPlayer player)
        {
            //get a list of cards for which there are no other cards in their suite.  Exclude Trump cards.
            var soloCardsOfSuite = player.Cards
                .GroupBy(g => CardRepository.GetSuite(g))
                .Where(g => g.Count() == 1 && CardRepository.GetSuite(g.First()) != Suite.TRUMP)
                .Select(g => g.First()).ToList();
            return player.Cards.OrderBy(c => soloCardsOfSuite.Contains(c) ? 1 : 2).ThenByDescending(c => c.Rank).Take(2).ToList();
        }
    }
}