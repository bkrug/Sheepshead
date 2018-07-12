using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Models.Players
{
    public class Player : IPlayer
    {
        public virtual string Name { get; set; }
        public List<SheepCard> Cards { get; } = new List<SheepCard>();

        public int QueueRankInTrick(ITrick trick)
        {
            var indexOfMe = trick.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Players.IndexOf(trick.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += trick.PlayerCount;
            return rank + 1;
        }

        public int QueueRankInDeck(IDeck deck)
        {
            var indexOfMe = deck.Players.IndexOf(this);
            var indexOfStartingPlayer = deck.Players.IndexOf(deck.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += deck.PlayerCount;
            return rank + 1;
        }

        public List<SheepCard> LegalCalledAces(IDeck deck)
        {
            var suits = LegalCalledAceSuits(deck);
            return suits.Select(g => GetAceOfSuit(g.Key)).ToList();
        }

        protected IEnumerable<IGrouping<Suit, SheepCard>> LegalCalledAceSuits(IDeck deck)
        {
            var allCards = Cards
                .Union(deck.Blinds)
                .Union(deck.Buried)
                .ToList();
            var suitsOfAcesInHand =
                new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_HEARTS, SheepCard.ACE_SPADES }
                .Where(sc => allCards.Contains(sc))
                .Select(sc => CardUtil.GetSuit(sc))
                .ToList();
            var acceptableSuits = allCards
                .Where(c => {
                    var suit = CardUtil.GetSuit(c);
                    return suit != Suit.TRUMP && !suitsOfAcesInHand.Contains(suit);
                })
                .GroupBy(c => CardUtil.GetSuit(c));
            return acceptableSuits;
        }

        protected static SheepCard GetAceOfSuit(Suit key)
        {
            switch (key)
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

    public interface IPlayer
    {
        string Name { get; set; }
        List<SheepCard> Cards { get; }
        int QueueRankInTrick(ITrick trick);
        int QueueRankInDeck(IDeck deck);
        List<SheepCard> LegalCalledAces(IDeck deck);
    }
}