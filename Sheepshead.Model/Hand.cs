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
        public SheepCard? PartnerCard { get; private set; }
        public int[] PartnerCardPlayed { get; private set; }
        private List<ITrick> _tricks = new List<ITrick>();
        public List<ITrick> Tricks { get { return _tricks.ToList(); } }
        public IPlayer StartingPlayer { get { return Deck.StartingPlayer; } }
        public event EventHandler<EventArgs> OnAddTrick;
        public event EventHandler<EventArgs> OnHandEnd;
        public int PlayerCount => Deck.PlayerCount;
        public List<IPlayer> Players => Deck.Players;

        public bool Leasters { get { return Picker == null; } }

        public Hand(IDeck deck, IPlayer picker, List<SheepCard> droppedCards)
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
                if (Deck.PlayerCount == 5)
                    PartnerCard = ChoosePartnerCard(picker);
            }
            PartnerCardPlayed = null;
        }

        private SheepCard ChoosePartnerCard(IPlayer picker)
        {
            var potentialPartnerCards = new[] {
                SheepCard.JACK_DIAMONDS,
                SheepCard.JACK_HEARTS,
                SheepCard.JACK_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.QUEEN_HEARTS,
                SheepCard.QUEEN_SPADES
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

        public HandScores Scores()
        {
            if (!Leasters)
                return GetNonLeasterScores();
            else
                return GetLeasterScores();
        }

        private HandScores GetNonLeasterScores()
        {
            var handScores = new HandScores()
            {
                Points = new Dictionary<IPlayer, int>(),
                Coins = new Dictionary<IPlayer, int>()
            };
            AssignNonLeasterPoints(handScores, out int defensePoints, out bool challengersWonOneTrick);
            int defensiveCoins = CalculateDefensiveCoins(defensePoints, challengersWonOneTrick);
            AssignNonLeasterCoins(handScores, challengersWonOneTrick, defensiveCoins);
            return handScores;
        }

        private void AssignNonLeasterPoints(HandScores handScores, out int defensePoints, out bool challengersWonOneTrick)
        {
            handScores.Points.Add(Picker, Deck.Buried.Sum(c => CardUtil.GetPoints(c)));
            defensePoints = 0;
            challengersWonOneTrick = false;
            foreach (var trick in _tricks)
            {
                var winnerData = trick.Winner();
                if (winnerData?.Player != Picker && winnerData?.Player != Partner)
                    defensePoints += winnerData.Points;
                else
                    challengersWonOneTrick = true;
                if (winnerData?.Player != null)
                {
                    if (handScores.Points.ContainsKey(winnerData.Player))
                        handScores.Points[winnerData.Player] += winnerData.Points;
                    else
                        handScores.Points.Add(winnerData.Player, winnerData.Points);
                }
            }
        }

        private static int CalculateDefensiveCoins(int defensePoints, bool challengersWonOneTrick)
        {
            int defensiveCoins;
            if (!challengersWonOneTrick)
                defensiveCoins = 3;
            else if (defensePoints >= 90)
                defensiveCoins = 2;
            else if (defensePoints >= 60)
                defensiveCoins = 1;
            else if (defensePoints >= 30)
                defensiveCoins = -1;
            else if (defensePoints > 0)
                defensiveCoins = -2;
            else
                defensiveCoins = -3;
            return defensiveCoins;
        }

        private void AssignNonLeasterCoins(HandScores handScores, bool challengersWonOneTrick, int defensiveCoins)
        {
            Deck.Players
                .Except(new List<IPlayer>() { Partner, Picker })
                .ToList()
                .ForEach(p => handScores.Coins.Add(p, defensiveCoins));
            var totalDefensiveCoins = handScores.Coins.Sum(c => c.Value);
            if (Partner == null)
                handScores.Coins.Add(Picker, -totalDefensiveCoins);
            else if (!challengersWonOneTrick)
            {
                handScores.Coins.Add(Picker, -totalDefensiveCoins);
                handScores.Coins.Add(Partner, 0);
            }
            else
            {
                handScores.Coins.Add(Picker, -totalDefensiveCoins * 2 / 3);
                handScores.Coins.Add(Partner, -totalDefensiveCoins / 3);
            }
        }

        private HandScores GetLeasterScores()
        {
            var trickPoints = Tricks.Select(t => t.Winner())
                                    .GroupBy(t => t.Player)
                                    .ToDictionary(g => g.Key, g => g.Sum(wd => wd.Points));

            var leasterWinner = trickPoints.OrderBy(c => c.Value).First().Key;
            var trickCoins = Deck.Players.ToDictionary(p => p, p => p == leasterWinner ? Deck.PlayerCount - 1 : -1 );

            return new HandScores()
            {
                Coins = trickCoins,
                Points = trickPoints
            };
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
            return String.Join("", Deck.Blinds.Select(c => CardUtil.ToAbbr(c)));
        }

        private string GetBuriedSummary()
        {
            if (Leasters)
                return String.Empty;
            var indexOfStartingPlayer = Players.IndexOf(Deck.StartingPlayer);
            var indexOfPicker = Players.IndexOf(Picker);
            var pickerId = indexOfPicker - indexOfStartingPlayer + 1;
            if (pickerId <= 0) pickerId += Deck.PlayerCount;
            return pickerId + String.Join("", Deck.Buried.Select(c => CardUtil.ToAbbr(c)));
        }

        private string GetTrickSummary(ITrick trick)
        {
            var summary = "";
            for (var i = 0; i < 5; ++i)
            {
                var indexOfStartingPlayer = Players.IndexOf(Deck.StartingPlayer);
                var player = indexOfStartingPlayer + i < Deck.PlayerCount ? indexOfStartingPlayer + i : indexOfStartingPlayer + i - Deck.PlayerCount;
                summary += CardUtil.ToAbbr(trick.CardsPlayed[Players[player]]);
            }
            return summary;
        }

        protected virtual void OnAddTrickHandler()
        {
            var e = new EventArgs();
            OnAddTrick?.Invoke(this, e);
        }

        protected virtual void OnHandEndHandler()
        {
            var e = new EventArgs();
            OnHandEnd?.Invoke(this, e);
        }

        public void SetPartner(IPlayer partner, ITrick trick)
        {
            Partner = partner;
            if (trick == null)
                return;
            PartnerCardPlayed = new[] { -1, -1 };
            PartnerCardPlayed[0] = Tricks.IndexOf(trick);
            PartnerCardPlayed[1] = trick.QueueRankOfPartner.Value;
        }

        public void GoItAlone()
        {
            PartnerCard = null;
        }
    }

    public interface IHand
    {
        IDeck Deck { get; }
        IPlayer Picker { get; }
        IPlayer Partner { get; }
        SheepCard? PartnerCard { get; }
        void GoItAlone();
        int[] PartnerCardPlayed { get; }
        List<ITrick> Tricks { get; }
        void AddTrick(ITrick trick);
        HandScores Scores();
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
}