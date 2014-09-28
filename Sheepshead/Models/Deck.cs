using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Deck : IDeck
    {
        const int PLAYER_COUNT = 5;
        const int CARDS_IN_DECK = 32;
        const int BLIND_COUNT = 2;
        private List<IPlayer> _playersRefusingPick;

        public IGame Game { get; private set; }
        public List<ICard> Blinds { get; private set; }
        public List<ICard> Discards { get; set; }
        public IHand Hand { get; set; }
        public List<IPlayer> PlayersRefusingPick { get { return _playersRefusingPick; } }
        public IPlayer StartingPlayer { get; private set; }

        public Deck(IGame game)
        {
            var lastDeck = game.Decks.LastOrDefault();
            if (lastDeck != null && (lastDeck.Hand == null || !lastDeck.Hand.IsComplete()))
                throw new PreviousDeckIncompleteException("Cannot add a deck until the prvious one is incomplete.");
            Game = game;
            var cards = ShuffleCards();
            DealCards(cards);
            game.Decks.Add(this);
        }

        private Queue<ICard> ShuffleCards()
        {
            List<ICard> cards = CardRepository.Instance.UnshuffledList();
            var rnd = new Random();
            for (var i = CARDS_IN_DECK - 1; i > 0; --i)
            {
                var j = rnd.Next(i);
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
            var totalRounds = (CARDS_IN_DECK - BLIND_COUNT) / PLAYER_COUNT / 2;
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

        public void PlayerWontPick(IPlayer player)
        {
            _playersRefusingPick.Add(player);
        }
    }

    public interface IDeck
    {
        List<ICard> Blinds { get; }
        List<ICard> Discards { get; set; }
        IGame Game { get; }
        IHand Hand { get; set; }
        List<IPlayer> PlayersRefusingPick { get; }
        void PlayerWontPick(IPlayer player);
        IPlayer StartingPlayer { get; }
    }

    public class PreviousDeckIncompleteException : ApplicationException
    {
        public PreviousDeckIncompleteException(string message) : base(message) { }
    }
}