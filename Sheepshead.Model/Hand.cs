using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public class Hand : IHand
    {
        public IDeck Deck { get; private set; }
        public IPlayer Picker { get; private set; }
        public IPlayer Partner { get; private set; }
        public ICard PartnerCard { get; private set; }
        public int[] PartnerCardPlayed { get; private set; }
        private List<ITrick> _tricks = new List<ITrick>();
        public List<ITrick> Tricks { get { return _tricks.ToList(); } }
        public IPlayer StartingPlayer { get { return Deck.StartingPlayer; } }
        public event EventHandler<EventArgs> OnAddTrick;
        public event EventHandler<EventArgs> OnHandEnd;

        public bool Leasters { get { return Picker == null; } }

        public Hand(IDeck deck, IPlayer picker, List<ICard> droppedCards)
        {
            Deck = deck;
            if (Deck.Hand != null)
                throw new DeckHasHandException("The specified deck is already associated with a hand.");
            Deck.Hand = this;
            Picker = picker;
            if (picker != null)
            {
                picker.Cards.AddRange(deck.Blinds.Where(c => !picker.Cards.Contains(c)));
                picker.Cards.Where(c => droppedCards.Contains(c)).ToList().ForEach(c => picker.Cards.Remove(c));
                PartnerCard = ChoosePartnerCard(picker);
            }
            PartnerCardPlayed = null;
        }

        private ICard ChoosePartnerCard(IPlayer picker)
        {
            var cri = CardRepository.Instance;
            var potentialPartnerCards = new[] { 
                cri[StandardSuite.DIAMONDS, CardType.JACK],
                cri[StandardSuite.HEARTS, CardType.JACK],
                cri[StandardSuite.SPADES, CardType.JACK],
                cri[StandardSuite.CLUBS, CardType.JACK],
                cri[StandardSuite.DIAMONDS, CardType.QUEEN],
                cri[StandardSuite.HEARTS, CardType.QUEEN],
                cri[StandardSuite.SPADES, CardType.QUEEN]
            };
            if (!picker.Cards.Any())
            {
            }
            if (!potentialPartnerCards.Any(c => !picker.Cards.Contains(c)))
            {
            }
            var partnerCard = potentialPartnerCards.First(c => !picker.Cards.Contains(c));
            return partnerCard;
        }

        public void AddTrick(ITrick trick)
        {
            _tricks.Add(trick);
            OnAddTrickHandler();
            if (_tricks.Count == (int)(Sheepshead.Models.Game.CARDS_IN_DECK / Deck.PlayerCount))
                trick.OnTrickEnd += (Object sender, EventArgs e) => { OnHandEndHandler(); };
        }

        public Dictionary<IPlayer, int> Scores()
        {
            if (!Leasters)
                return NonLeasterPoints();
            else
                return LeasterPoints();
        }

        private Dictionary<IPlayer, int> NonLeasterPoints()
        {
            var defensePoints = 0;
            foreach (var trick in _tricks)
            {
                var winnerData = trick.Winner();
                if (winnerData.Player != Picker && winnerData.Player != Partner)
                    defensePoints += winnerData.Points;
            }
            int defensiveHandPoints;
            if (defensePoints == 0)
                defensiveHandPoints = -3;
            else if (defensePoints <= 29)
                defensiveHandPoints = -2;
            else if (defensePoints <= 59)
                defensiveHandPoints = -1;
            else
                defensiveHandPoints = 2;

            var dict = new Dictionary<IPlayer, int>();
            foreach (var player in Deck.Players)
            {
                if (player == Picker)
                    dict.Add(player, defensiveHandPoints * -2);
                else if (player == Partner)
                    dict.Add(player, defensiveHandPoints * -1);
                else
                    dict.Add(player, defensiveHandPoints);
            }
            return dict;
        }

        private Dictionary<IPlayer, int> LeasterPoints()
        {
            var trickWinners = Tricks.Select(t => t.Winner());
            var leastPoints = trickWinners.GroupBy(t => t.Player).OrderBy(g => g.Sum(t => t.Points)).First();
            var leastPointsPlayer = leastPoints.Select(g => g.Player).First();
            var points = new Dictionary<IPlayer, int>();
            foreach (var player in this.Deck.Players)
                points.Add(player, player == leastPointsPlayer ? Deck.PlayerCount - 1 : -1 );
            return points;
        }

        public bool IsComplete()
        {
            const int CARDS_IN_PLAY = 30;
            var trickCount = CARDS_IN_PLAY / Deck.PlayerCount;
            return _tricks.Count() == trickCount && _tricks.Last().IsComplete();
        }

        public string Summary()
        {
            var pieces = new List<string>();
            pieces.Add(GetBlindSummary());
            pieces.Add(GetBuriedSummary());
            foreach(var trick in Tricks)
                pieces.Add(GetTrickSummary(trick));
            return String.Join(",", pieces);
        }

        private string GetBlindSummary()
        {
            return String.Join("", Deck.Blinds.Select(c => c.ToAbbr()));
        }

        private string GetBuriedSummary()
        {
            if (Leasters)
                return String.Empty;
            var indexOfStartingPlayer = Players.IndexOf(Deck.StartingPlayer);
            var indexOfPicker = Players.IndexOf(Picker);
            var pickerId = indexOfPicker - indexOfStartingPlayer + 1;
            if (pickerId <= 0) pickerId += Deck.PlayerCount;
            return pickerId + String.Join("", Deck.Buried.Select(c => c.ToAbbr()));
        }

        private string GetTrickSummary(ITrick trick)
        {
            var summary = "";
            for (var i = 0; i < 5; ++i)
            {
                var indexOfStartingPlayer = Players.IndexOf(Deck.StartingPlayer);
                var player = indexOfStartingPlayer + i < Deck.PlayerCount ? indexOfStartingPlayer + i : indexOfStartingPlayer + i - Deck.PlayerCount;
                summary += trick.CardsPlayed[Players[player]].ToAbbr();
            }
            return summary;
        }

        protected virtual void OnAddTrickHandler()
        {
            var e = new EventArgs();
            if (OnAddTrick != null)
                OnAddTrick(this, e);
        }

        protected virtual void OnHandEndHandler()
        {
            var e = new EventArgs();
            if (OnHandEnd != null)
                OnHandEnd(this, e);
        }

        public void SetPartner(IPlayer partner, ITrick trick)
        {
            Partner = partner;
            PartnerCardPlayed = new[] { -1, -1 };
            PartnerCardPlayed[0] = Tricks.IndexOf(trick);
            PartnerCardPlayed[1] = trick.QueueRankOfPartner.Value;
        }

        public int PlayerCount
        {
            get { return Deck.PlayerCount; }
        }

        public List<IPlayer> Players
        {
            get { return Deck.Players; }
        }
    }

    public interface IHand
    {
        IDeck Deck { get; }
        IPlayer Picker { get; }
        IPlayer Partner { get; }
        ICard PartnerCard { get; }
        int[] PartnerCardPlayed { get; }
        List<ITrick> Tricks { get; }
        void AddTrick(ITrick trick);
        Dictionary<IPlayer, int> Scores();
        bool IsComplete();
        bool Leasters { get; }
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        IPlayer StartingPlayer { get; }
        event EventHandler<EventArgs> OnAddTrick;
        event EventHandler<EventArgs> OnHandEnd;
        string Summary();
        void SetPartner(IPlayer partner, ITrick trick);
    }

    public class DeckHasHandException : ApplicationException
    {
        public DeckHasHandException(string message)
            : base(message)
        {
        }
    }
}