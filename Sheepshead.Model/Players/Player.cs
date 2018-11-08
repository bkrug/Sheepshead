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
            if (trick.StartingPlayer == null) throw new NullReferenceException();
            var indexOfMe = trick.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Players.IndexOf(trick.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += trick.PlayerCount;
            return rank + 1;
        }

        public int QueueRankInDeck(IDeck deck)
        {
            if (deck.StartingPlayer == null) throw new NullReferenceException();
            var indexOfMe = deck.Players.IndexOf(this);
            var indexOfStartingPlayer = deck.Players.IndexOf(deck.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += deck.PlayerCount;
            return rank + 1;
        }

        public List<SheepCard> LegalCalledAces(IDeck deck)
        {
            var suits = LegalCalledAceSuits(deck);
            return suits.LegalSuits.Select(g => GetCardOfSuit(suits.CardType, g.Key)).ToList();
        }

        protected LegalCalledAces LegalCalledAceSuits(IDeck deck)
        {
            var allPickersCards = Cards
                .Union(deck.Blinds)
                .Union(deck.Buried)
                .ToList();
            var allAces = new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_HEARTS, SheepCard.ACE_SPADES };
            var allTens = new List<SheepCard>() { SheepCard.N10_CLUBS, SheepCard.N10_HEARTS, SheepCard.N10_SPADES };
            var pickerHasAllAces = allAces.All(c => allPickersCards.Contains(c));

            var potentialPartnerCards = pickerHasAllAces ? allTens : allAces;
            var illegalSuits =
                potentialPartnerCards
                .Where(sc => allPickersCards.Contains(sc))
                .Select(sc => CardUtil.GetSuit(sc))
                .Union(new List<Suit>() { Suit.TRUMP })
                .ToList();

            var acceptableSuits = allPickersCards
                .Where(c =>
                {
                    var suit = CardUtil.GetSuit(c);
                    return !illegalSuits.Contains(suit);
                })
                .GroupBy(c => CardUtil.GetSuit(c));
            return new LegalCalledAces()
            {
                LegalSuits = acceptableSuits,
                CardType = pickerHasAllAces ? CardType.N10 : CardType.ACE
            };
        }

        protected static SheepCard GetCardOfSuit(CardType cardType, Suit key)
        {
            return cardType == CardType.ACE ? GetAceOfSuit(key) : GetTenOfSuit(key);
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

        protected static SheepCard GetTenOfSuit(Suit key)
        {
            switch (key)
            {
                case Suit.CLUBS:
                    return SheepCard.N10_CLUBS;
                case Suit.HEARTS:
                    return SheepCard.N10_HEARTS;
                case Suit.SPADES:
                default:
                    return SheepCard.N10_SPADES;
            }
        }

        protected Boolean IamPartner(ITrick trick)
        {
            return trick.Hand.Partner == this || trick.Hand.PartnerCard.HasValue && Cards.Contains(trick.Hand.PartnerCard.Value);
        }
    }

    public class LegalCalledAces
    {
        public IEnumerable<IGrouping<Suit, SheepCard>> LegalSuits { get; set; }
        public CardType CardType { get; set; }
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