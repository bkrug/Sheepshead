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

        public IGame Game { get; private set; }
        public List<ICard> Blinds { get; private set; }
        public List<ICard> Discards { get; set; }

        public Deck(IGame game)
        {
            Game = game;
            var cards = ShuffleCards();
            DealCards(cards);
        }

        private Queue<ICard> ShuffleCards()
        {
            List<ICard> cards = CardRepository.Instance.UnshuffledList();
            var rnd = new Random();
            for (var i = CARDS_IN_DECK; i > 1; --i)
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
            var totalRounds = (CARDS_IN_DECK - BLIND_COUNT) / PLAYER_COUNT;
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
    }

    public interface IDeck
    {
        List<ICard> Blinds { get; }
        List<ICard> Discards { get; set; }
        IGame Game { get; }
    }
}