using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public class Hand : IHand
    {
        public IDeck Deck { get; private set; }
        public IPlayer Picker { get; private set; }
        public IPlayer Partner { set; get; }
        public ICard PartnerCard { get; private set; }
        private List<ITrick> _tricks = new List<ITrick>();
        public List<ITrick> Tricks { get { return _tricks.ToList(); } }
        public IPlayer StartingPlayer { get { return Deck.StartingPlayer; } }

        public bool Leasters { get { return Picker == null; } }

        public Hand(IDeck deck, IPlayer picker, List<ICard> droppedCards)
        {
            Deck = deck;
            if (Deck.Hand != null)
                throw new DeckHasHandException("Must add a hand to a deck without one.");
            Deck.Hand = this;
            Picker = picker;
            if (picker != null)
            {
                picker.Cards.AddRange(deck.Blinds.Where(c => !picker.Cards.Contains(c)));
                picker.Cards.Where(c => droppedCards.Contains(c)).ToList().ForEach(c => picker.Cards.Remove(c));
                PartnerCard = ChoosePartnerCard(picker);
            }
        }

        private ICard ChoosePartnerCard(IPlayer picker)
        {
            var cri = CardRepository.Instance;
            var potentialPartnerCards = new[] { 
                cri[StandardSuite.DIAMONDS, CardType.JACK],
                cri[StandardSuite.HEARTS, CardType.JACK],
                cri[StandardSuite.SPADES, CardType.JACK],
                cri[StandardSuite.CLUBS, CardType.JACK],
                cri[StandardSuite.DIAMONDS, CardType.QUEEN] 
            };
            var partnerCard = potentialPartnerCards.First(c => !picker.Cards.Contains(c));
            return partnerCard;
        }

        public void AddTrick(ITrick trick)
        {
            _tricks.Add(trick);
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

        public void EndHand()
        {
            foreach (var trick in Tricks)
                trick.OnHandEnd();
        }

        public bool IsComplete()
        {
            const int CARDS_IN_PLAY = 30;
            var trickCount = CARDS_IN_PLAY / Deck.PlayerCount;
            return _tricks.Count() == trickCount && _tricks.Last().IsComplete();
        }

        public int PlayerCount
        {
            get { return Deck.Game.PlayerCount; }
        }

        public List<IPlayer> Players
        {
            get { return Deck.Game.Players; }
        }
    }

    public interface IHand
    {
        IDeck Deck { get; }
        IPlayer Picker { get; }
        IPlayer Partner { set; get; }
        ICard PartnerCard { get; }
        List<ITrick> Tricks { get; }
        void AddTrick(ITrick trick);
        Dictionary<IPlayer, int> Scores();
        bool IsComplete();
        bool Leasters { get; }
        void EndHand();
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        IPlayer StartingPlayer { get; }
    }

    public class DeckHasHandException : ApplicationException
    {
        public DeckHasHandException(string message)
            : base(message)
        {
        }
    }
}