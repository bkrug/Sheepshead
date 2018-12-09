using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    public class Hand : IHand
    {
        public IDeck Deck { get; private set; }
        public IPlayer Picker { get; private set; }
        public IPlayer Partner { get; private set; }
        public SheepCard? PartnerCard { get; private set; }
        private List<ITrick> _tricks = new List<ITrick>();
        public List<ITrick> Tricks { get { return _tricks.ToList(); } }
        public IPlayer StartingPlayer { get { return Deck.StartingPlayer; } }
        public event EventHandler<EventArgs> OnHandEnd;
        public int PlayerCount => Deck.PlayerCount;
        public List<IPlayer> Players => Deck.Players;
        public bool Leasters { get { return Picker == null; } }

        public Hand(IDeck deck)
        {
            Deck = deck;
            if (Deck.Hand != null)
                throw new DeckHasHandException("The specified deck is already associated with a hand.");
            Deck.Hand = this;
        }

        public void SetPicker(IPlayer picker, List<SheepCard> burried)
        {
            Picker = picker;
            if (picker != null)
            {
                HandUtils.BuryCards(this, picker, burried);
                PartnerCard = HandUtils.ChoosePartnerCard(this, picker);
            }
        }

        public IPlayer PresumedParnter
        {
            get
            {
                if (!PartnerCard.HasValue)
                    return null;
                if (Partner != null)
                    return Partner;
                var potentialPartnerGroups = Tricks
                    .Where(t =>
                        t.CardsPlayed.First().Key != Picker
                        && CardUtil.GetSuit(t.CardsPlayed.First().Value) == Suit.TRUMP
                    )
                    .Select(t => t.CardsPlayed.First().Key)
                    .GroupBy(p => p)
                    .OrderByDescending(g => g.Count())
                    .ToList();
                if (!potentialPartnerGroups.Any()
                    || potentialPartnerGroups.Count >= 2 && potentialPartnerGroups[0].Count() == potentialPartnerGroups[1].Count())
                    return null;
                else
                    return potentialPartnerGroups.First().Key;  
            }
        }

        private SheepCard? ChoosePartnerCard(IPlayer picker)
        {
            var potentialPartnerCards = new[] {
                SheepCard.JACK_DIAMONDS,
                SheepCard.JACK_HEARTS,
                SheepCard.JACK_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.QUEEN_HEARTS,
                SheepCard.QUEEN_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            SheepCard? partnerCard = potentialPartnerCards.Any(c => !picker.Cards.Contains(c))
                ? potentialPartnerCards.First(c => !picker.Cards.Contains(c))
                : (SheepCard?)null;
            return partnerCard;
        }

        public void AddTrick(ITrick trick)
        {
            _tricks.Add(trick);
            if (_tricks.Count == (int)(Game.CARDS_IN_DECK / Deck.PlayerCount))
                trick.OnTrickEnd += (Object sender, EventArgs e) => { OnHandEndHandler(); };
        }

        private HandScores _scores = null;
        public HandScores Scores()
        {
            if (Tricks.Count == Deck.Game.TrickCount && Tricks.Last().IsComplete())
                return _scores = (_scores ?? new ScoreCalculator(this).InternalScores());
            return null;
        }

        public bool IsComplete()
        {
            const int CARDS_IN_PLAY = 30;
            var trickCount = CARDS_IN_PLAY / Deck.PlayerCount;
            return _tricks.Count() == trickCount && _tricks.Last().IsComplete();
        }

        protected virtual void OnHandEndHandler()
        {
            var e = new EventArgs();
            OnHandEnd?.Invoke(this, e);
        }

        public void SetPartner(IPlayer partner, ITrick trick)
        {
            Partner = partner;
        }

        public void GoItAlone()
        {
            PartnerCard = null;
        }

        public void SetPartnerCard(SheepCard? sheepCard)
        {
            if (Deck.Game.PartnerMethod != PartnerMethod.CalledAce)
                throw new InvalidOperationException("Can only set partner card if partner method is 'called ace'.");
            PartnerCard = sheepCard;
        }
    }

    public interface IHand
    {
        IDeck Deck { get; }
        IPlayer Picker { get; }
        IPlayer Partner { get; }
        SheepCard? PartnerCard { get; }
        IPlayer PresumedParnter { get; }
        void GoItAlone();
        List<ITrick> Tricks { get; }
        void AddTrick(ITrick trick);
        HandScores Scores();
        bool IsComplete();
        bool Leasters { get; }
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        IPlayer StartingPlayer { get; }
        event EventHandler<EventArgs> OnHandEnd;
        void SetPartner(IPlayer partner, ITrick trick);
        void SetPartnerCard(SheepCard? sheepCard);
    }

    public class HandScores
    {
        public Dictionary<IPlayer, int> Coins { get; set; }
        public Dictionary<IPlayer, int> Points { get; set; }
    }

    public class DeckHasHandException : ApplicationException
    {
        public DeckHasHandException(string message)
            : base(message)
        {
        }
    }

    public static class HandUtils
    {
        public static void BuryCards(IHand hand, IPlayer picker, List<SheepCard> burried)
        {
            if (picker != null)
            {
                picker.Cards.AddRange(hand.Deck.Blinds.Where(c => !picker.Cards.Contains(c)));
                picker.Cards.Where(c => burried.Contains(c)).ToList().ForEach(c => picker.Cards.Remove(c));
            }
        }

        public static SheepCard? ChoosePartnerCard(IHand hand, IPlayer picker)
        {
            if (hand.Deck.PlayerCount == 3 || hand.Deck.Game.PartnerMethod == PartnerMethod.CalledAce)
                return null;
            var potentialPartnerCards = new[] {
                SheepCard.JACK_DIAMONDS,
                SheepCard.JACK_HEARTS,
                SheepCard.JACK_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.QUEEN_HEARTS,
                SheepCard.QUEEN_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var pickerDoesNotHave = potentialPartnerCards.Where(c => 
                !picker.Cards.Contains(c) 
                && !hand.Deck.Blinds.Contains(c) 
                && !hand.Deck.Buried.Contains(c));
            return pickerDoesNotHave.Any() ? pickerDoesNotHave.First() : (SheepCard?)null;
        }
    }
}