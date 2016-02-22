using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Models
{
    public class Deck : IDeck
    {
        private List<IPlayer> _playersRefusingPick = new List<IPlayer>();

        public IGame Game { get; private set; }
        public List<ICard> Blinds { get; private set; }
        public List<ICard> Buried { get; set; }
        public IHand Hand { get; set; }
        public List<IPlayer> PlayersRefusingPick { get { return _playersRefusingPick.ToList(); } }
        public IPlayer StartingPlayer { get; private set; }
        public IRandomWrapper _random { get; private set; }

        public Deck(IGame game, IRandomWrapper random)
        {
            if (!game.LastDeckIsComplete())
                throw new PreviousDeckIncompleteException("Cannot add a deck until the prvious one is complete.");
            _random = random;
            Game = game;
            var cards = ShuffleCards();
            DealCards(cards);
            game.Decks.Add(this);
            SetStartingPlayer();
            Buried = new List<ICard>();
        }

        private Queue<ICard> ShuffleCards()
        {
            List<ICard> cards = CardRepository.Instance.UnshuffledList();
            for (var i = Sheepshead.Models.Game.CARDS_IN_DECK - 1; i > 0; --i)
            {
                var j = _random.Next(i);
                var swap = cards[i];
                cards[i] = cards[j];
                cards[j] = swap;
            }
            var queue = new Queue<ICard>();
            cards.ForEach(c => queue.Enqueue(c));
            return queue;
        }

        private void DealCards(Queue<ICard> cards) 
        {
            foreach (var player in Game.Players)
                player.Cards.RemoveAll(c => true);
            var totalTricks = (int)(Sheepshead.Models.Game.CARDS_IN_DECK / Game.PlayerCount);
            var totalRounds = totalTricks / 2;
            Blinds = new List<ICard>();
            for (var round = 0; round < totalRounds; ++round)
            {
                if (round > 0)
                    Blinds.Add(cards.Dequeue());
                foreach (var player in Game.Players)
                {
                    player.Cards.Add(cards.Dequeue());
                    player.Cards.Add(cards.Dequeue());
                }
            }
        }

        private void SetStartingPlayer()
        {
            var index = Game.Decks.IndexOf(this);
            var indexOfPlayer = (index == 0)
                ? _random.Next(Game.PlayerCount) 
                : Game.Players.IndexOf(Game.Decks[index - 1].StartingPlayer) + 1;
            if (indexOfPlayer == Game.PlayerCount) indexOfPlayer = 0;
            StartingPlayer = Game.Players[indexOfPlayer];
        }

        public void PlayerWontPick(IPlayer player)
        {
            _playersRefusingPick.Add(player);
        }

        public int PlayerCount
        {
            get { return Game.PlayerCount; }
        }

        public List<IPlayer> Players
        {
            get { return Game.Players; }
        }

        public List<IPlayer> PlayersWithoutPickTurn
        {
            get { return PickPlayerOrderer.PlayersWithoutPickTurn; }
        }

        private IPickPlayerOrderer _pickPlayerOrderer;
        public IPickPlayerOrderer PickPlayerOrderer
        {
            get { return _pickPlayerOrderer ?? (_pickPlayerOrderer = new PickPlayerOrderer(new PickPlayerOrdererInner(this))); }
        }

        private IPickProcessor _pickProcessor;
        public IPickProcessor PickProcessor
        {
            get { return _pickProcessor ?? (_pickProcessor = new PickProcessor(this, new HandFactory())); }
        }
    }

    public interface IDeck
    {
        List<ICard> Blinds { get; }
        List<ICard> Buried { get; set; }
        IGame Game { get; }
        IHand Hand { get; set; }
        List<IPlayer> PlayersRefusingPick { get; }
        void PlayerWontPick(IPlayer player);
        IPlayer StartingPlayer { get; }
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        List<IPlayer> PlayersWithoutPickTurn { get; }
        IPickPlayerOrderer PickPlayerOrderer { get; }
        IPickProcessor PickProcessor { get; }
    }

    public class PreviousDeckIncompleteException : Exception
    {
        public PreviousDeckIncompleteException(string message) : base(message) { }
    }
}