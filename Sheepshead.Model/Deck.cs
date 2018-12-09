using System;
using System.Linq;
using System.Collections.Generic;
using Sheepshead.Model.Players;
using Sheepshead.Model.Wrappers;

namespace Sheepshead.Model
{
    public partial class Hand : IHand
    {
        public IGame Game { get; private set; }
        public List<SheepCard> Blinds { get; private set; } = new List<SheepCard>();
        public List<SheepCard> Buried { get; set; } = new List<SheepCard>();
        public List<IPlayer> PlayersRefusingPick { get; } = new List<IPlayer>();
        public IPlayer StartingPlayer { get; private set; }
        public IRandomWrapper _random { get; private set; }
        public bool PickPhaseComplete { get; private set; }
        /// <summary>
        /// Returns true when there is no picker and Leasters is off.
        /// </summary>
        public bool MustRedeal => !Game.LeastersEnabled && !PlayersWithoutPickTurn.Any();

        public Hand(IGame game) : this(game, new RandomWrapper())
        {
        }

        public Hand(IGame game, IRandomWrapper random)
        {
            if (!game.LastDeckIsComplete())
                throw new PreviousDeckIncompleteException("Cannot add a deck until the prvious one is complete.");
            Game = game;
            Game.Decks.Add(this);
            _random = random;
            if (_random != null)
            {
                DealCards(ShuffleCards());
                SetStartingPlayer();
            }
            Buried = new List<SheepCard>();
        }

        private Queue<SheepCard> ShuffleCards()
        {
            List<SheepCard> cards = CardUtil.UnshuffledList();
            for (var i = Model.Game.CARDS_IN_DECK - 1; i > 0; --i)
            {
                var j = _random.Next(i);
                var swap = cards[i];
                cards[i] = cards[j];
                cards[j] = swap;
            }
            var queue = new Queue<SheepCard>();
            cards.ForEach(c => queue.Enqueue(c));
            return queue;
        }

        private void DealCards(Queue<SheepCard> cards) 
        {
            Blinds = new List<SheepCard>();
            foreach (var player in Game.Players)
                player.Cards.RemoveAll(c => true);
            switch (Game.PlayerCount)
            {
                case 3:
                    DealTwoCardsPerPlayer(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealTwoCardsPerPlayer(cards);
                    break;
                case 5:
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    break;
            }
        }

        private void DealOneBlind(Queue<SheepCard> cards)
        {
            Blinds.Add(cards.Dequeue());
        }

        private void DealTwoCardsPerPlayer(Queue<SheepCard> cards)
        {
            foreach (var player in Game.Players)
            {
                player.Cards.Add(cards.Dequeue());
                player.Cards.Add(cards.Dequeue());
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
            PlayersRefusingPick.Add(player);
        }

        public List<IPlayer> PlayersInTurnOrder => PickPlayerOrderer.PlayersInTurnOrder(Players, StartingPlayer);
        public List<IPlayer> PlayersWithoutPickTurn => PickPlayerOrderer.PlayersWithoutTurn(PlayersInTurnOrder, PlayersRefusingPick);

        private IPlayerOrderer _pickPlayerOrderer;
        public IPlayerOrderer PickPlayerOrderer
        {
            get { return _pickPlayerOrderer ?? (_pickPlayerOrderer = new PlayerOrderer()); }
        }
    }

    public class PreviousDeckIncompleteException : Exception
    {
        public PreviousDeckIncompleteException(string message) : base(message) { }
    }
}