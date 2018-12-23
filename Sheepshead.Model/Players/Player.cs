using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Model.Models;

namespace Sheepshead.Model.Players
{
    public abstract class Player : IPlayer
    {
        public Participant Participant { get; private set; }
        public virtual string Name {
            get { return Participant.Name; }
            set { Participant.Name = value; }
        }
        public IReadOnlyList<SheepCard> Cards => CardUtil.StringToCardList(Participant.Cards);

        public Player(Participant participant)
        {
            Participant = participant;
        }

        public void AddCard(SheepCard card)
        {
            Participant.Cards = CardUtil.CardListToString(Cards.Union(new List<SheepCard>() { card }).ToList());
        }

        public void AddCardRange(List<SheepCard> cards)
        {
            Participant.Cards = CardUtil.CardListToString(Cards.Union(cards).ToList());
        }

        public void RemoveCard(SheepCard card)
        {
            Participant.Cards = CardUtil.CardListToString(Cards.Except(new List<SheepCard>() { card }).ToList());
        }

        public void RemoveAllCards()
        {
            Participant.Cards = string.Empty;
        }

        public int QueueRankInTrick(ITrick trick)
        {
            if (trick.StartingPlayer == null) throw new NullReferenceException();
            var indexOfMe = trick.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Players.IndexOf(trick.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += trick.PlayerCount;
            return rank + 1;
        }

        public int QueueRankInHand(IHand hand)
        {
            if (hand.StartingPlayer == null) throw new NullReferenceException();
            var indexOfMe = hand.Players.IndexOf(this);
            var indexOfStartingPlayer = hand.Players.IndexOf(hand.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += hand.PlayerCount;
            return rank + 1;
        }

        public List<SheepCard> LegalCalledAces(IHand hand)
        {
            var suits = LegalCalledAceSuits(hand);
            return suits.LegalSuits.Select(g => GetCardOfSuit(suits.CardType, g.Key)).ToList();
        }

        protected LegalCalledAces LegalCalledAceSuits(IHand hand)
        {
            var allPickersCards = Cards
                .Union(hand.Blinds)
                .Union(hand.Buried)
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
            return trick.IHand.Partner == this || trick.IHand.PartnerCard.HasValue && Cards.Contains(trick.IHand.PartnerCard.Value);
        }
    }

    public class LegalCalledAces
    {
        public IEnumerable<IGrouping<Suit, SheepCard>> LegalSuits { get; set; }
        public CardType CardType { get; set; }
    }

    public interface IPlayer
    {
        Participant Participant { get; }
        string Name { get; set; }
        IReadOnlyList<SheepCard> Cards { get; }
        int QueueRankInTrick(ITrick trick);
        int QueueRankInHand(IHand hand);
        List<SheepCard> LegalCalledAces(IHand hand);
        void AddCard(SheepCard card);
        void AddCardRange(List<SheepCard> cards);
        void RemoveCard(SheepCard card);
        void RemoveAllCards();
    }
}