using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Game : LongId, IGame
    {
        public string Name { get; set; }
        public int PlayerCount { get { return _players.Count(); } }
        public int HumanPlayerCount { get { return _players.Count(p => p is IHumanPlayer); } }
        protected List<IPlayer> _players;
        public List<IPlayer> Players { get { return _players.ToList(); } }
        private List<IDeck> _decks = new List<IDeck>();
        public List<IDeck> Decks { get { return _decks; } }

        public Game(long id, List<IPlayer> players)
        {
            _players = players;
            _id = id;
        }

        public void PlayNonHumans(ITrick trick)
        {
            var playersMissed = PlayerCount;
            var playerIndex = Players.IndexOf(trick.StartingPlayer);
            while (trick.CardsPlayed.Keys.Contains(Players[playerIndex]) && playersMissed > 0)
            {
                IncrementPlayerIndex(ref playerIndex);
                --playersMissed;
            }
            for (; !(Players[playerIndex] is HumanPlayer) && playersMissed > 0; IncrementPlayerIndex(ref playerIndex))
            {
                --playersMissed;
                trick.Add(Players[playerIndex], ((ComputerPlayer)Players[playerIndex]).GetMove(trick));
            }
        }

        public IPlayer PlayNonHumans(IDeck deck)
        {
            var playersMissed = PlayerCount;
            var playerIndex = Players.IndexOf(deck.StartingPlayer);
            while (deck.PlayersRefusingPick.Contains(Players[playerIndex]))
            {
                IncrementPlayerIndex(ref playerIndex);
                --playersMissed;
            }
            IComputerPlayer picker = null;
            for (; picker == null && !(Players[playerIndex] is HumanPlayer) && playersMissed > 0; IncrementPlayerIndex(ref playerIndex))
            {
                --playersMissed;
                var curPlayer = (IComputerPlayer)Players[playerIndex];
                if (curPlayer.WillPick(deck))
                {
                    picker = curPlayer;
                    deck.Discards.AddRange(picker.DropCardsForPick(deck));
                }
                else
                    deck.PlayerWontPick(curPlayer);
            }
            return picker;
        }

        private void IncrementPlayerIndex(ref int playerIndex)
        {
            ++playerIndex;
            if (playerIndex >= PlayerCount)
                playerIndex = 0;
        }

        public void RearrangePlayers()
        {
            var rnd = new Random();
            for (var i = PlayerCount - 1; i > 0; --i)
            {
                var j = rnd.Next(i);
                var swap = Players[i];
                Players[i] = Players[j];
                Players[j] = swap;
            }
        }

        public bool LastDeckIsComplete()
        {
            var lastDeck = Decks.LastOrDefault();
            return lastDeck == null || lastDeck.Hand != null && lastDeck.Hand.IsComplete();
        }
    }

    public class TooManyPlayersException : ApplicationException
    {
        public TooManyPlayersException(string message) : base(message) { }
    }

    public class TooManyHumanPlayersException : TooManyPlayersException
    {
        public TooManyHumanPlayersException(string message) : base(message) { }
    }

    public class ObjectInListException : ApplicationException
    {
        public ObjectInListException(string message) : base(message) { }
    }

    public interface IGame : ILongId
    {
        string Name { get; set; }
        int HumanPlayerCount { get; }
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        List<IDeck> Decks { get; }
        void PlayNonHumans(ITrick trick);
        IPlayer PlayNonHumans(IDeck deck);
        void RearrangePlayers();
        bool LastDeckIsComplete();
    }
}