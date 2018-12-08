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
        public int[] PartnerCardPlayed { get; private set; }
        private List<ITrick> _tricks = new List<ITrick>();
        public List<ITrick> Tricks { get { return _tricks.ToList(); } }
        public IPlayer StartingPlayer { get { return Deck.StartingPlayer; } }
        public event EventHandler<EventArgs> OnAddTrick;
        public event EventHandler<EventArgs> OnHandEnd;
        public int PlayerCount => Deck.PlayerCount;
        public List<IPlayer> Players => Deck.Players;

        public bool Leasters { get { return Picker == null; } }

        public Hand(IDeck deck, IPlayer picker, List<SheepCard> burried)
        {
            Deck = deck;
            if (Deck.Hand != null)
                throw new DeckHasHandException("The specified deck is already associated with a hand.");
            Deck.Hand = this;
            Picker = picker;
            if (picker != null)
            {
                picker.Cards.AddRange(deck.Blinds.Where(c => !picker.Cards.Contains(c)));
                if (Deck.PlayerCount == 5 && deck.Game.PartnerMethod == PartnerMethod.JackOfDiamonds)
                    PartnerCard = ChoosePartnerCard(picker);
                picker.Cards.Where(c => burried.Contains(c)).ToList().ForEach(c => picker.Cards.Remove(c));
            }
            PartnerCardPlayed = null;
        }

        //TODO: Look for players who lead with trump.
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
            OnAddTrickHandler();
            if (_tricks.Count == (int)(Game.CARDS_IN_DECK / Deck.PlayerCount))
                trick.OnTrickEnd += (Object sender, EventArgs e) => { OnHandEndHandler(); };
        }

        private HandScores _scores = null;
        public HandScores Scores()
        {
            if (Tricks.Count == Deck.Game.TrickCount && Tricks.Last().IsComplete())
                return _scores = (_scores ?? InternalScores());
            return null;
        }

        //Only public for unit testing.
        public HandScores InternalScores()
        {
            if (!Leasters)
                return GetNonLeasterScores();
            else
                return GetLeasterScores();
        }

        private HandScores GetNonLeasterScores()
        {
            var handScores = new HandScores();
            handScores.Points = AssignNonLeasterPoints(out int defensePoints, out bool challengersWonOneTrick, out bool defenseWonOneTrick);
            int defensiveCoins = CalculateDefensiveCoins(defensePoints, challengersWonOneTrick, defenseWonOneTrick);
            handScores.Coins = AssignNonLeasterCoins(challengersWonOneTrick, defensiveCoins);
            return handScores;
        }

        private Dictionary<IPlayer, int> AssignNonLeasterPoints(out int defensePoints, out bool challengersWonOneTrick, out bool defenseWonOneTrick)
        {
            var handPoints = new Dictionary<IPlayer, int>();
            handPoints.Add(Picker, Deck.Buried.Sum(c => CardUtil.GetPoints(c)));
            defensePoints = 0;
            challengersWonOneTrick = false;
            defenseWonOneTrick = false;
            foreach (var trick in _tricks)
            {
                var winnerData = trick.Winner();
                if (winnerData?.Player == Picker || winnerData?.Player == Partner)
                    challengersWonOneTrick = true;
                else
                {
                    defensePoints += winnerData.Points;
                    defenseWonOneTrick = true;
                }
                if (winnerData?.Player != null)
                {
                    if (handPoints.ContainsKey(winnerData.Player))
                        handPoints[winnerData.Player] += winnerData.Points;
                    else
                        handPoints.Add(winnerData.Player, winnerData.Points);
                }
            }
            return handPoints;
        }

        private static int CalculateDefensiveCoins(int defensePoints, bool challengersWonOneTrick, bool defenseWonOneTrick)
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
            else if (defenseWonOneTrick)
                defensiveCoins = -2;
            else
                defensiveCoins = -3;
            return defensiveCoins;
        }

        private Dictionary<IPlayer, int> AssignNonLeasterCoins(bool challengersWonOneTrick, int defensiveCoins)
        {
            var handCoins = new Dictionary<IPlayer, int>();
            if (Partner == Picker) {
                var partnerCard = PartnerCard.HasValue ? Enum.GetName(typeof(SheepCard), PartnerCard.Value) : "no parter card.";
                throw new Exception("Picker and Partner are the same person! " + partnerCard);
            }
            Deck.Players
                .Except(new List<IPlayer>() { Partner, Picker })
                .ToList()
                .ForEach(p => handCoins.Add(p, defensiveCoins));
            var totalDefensiveCoins = handCoins.Sum(c => c.Value);
            if (Partner == null)
                handCoins.Add(Picker, -totalDefensiveCoins);
            else if (!challengersWonOneTrick)
            {
                handCoins.Add(Picker, -totalDefensiveCoins);
                handCoins.Add(Partner, 0);
            }
            else
            {
                handCoins.Add(Picker, -totalDefensiveCoins * 2 / 3);
                handCoins.Add(Partner, -totalDefensiveCoins / 3);
            }
            return handCoins;
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
}