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
            var startPlayerIndex = Players.IndexOf(trick.StartingPlayer);
            var playerIndex = startPlayerIndex;
            while (trick.CardsPlayed.Keys.Contains(Players[playerIndex]))
                IncrementPlayerIndex(ref playerIndex );
            for (; playerIndex != trick.Hand.Deck.Game.PlayerCount && !(Players[playerIndex] is HumanPlayer); IncrementPlayerIndex(ref playerIndex))
                trick.Add(Players[playerIndex], ((ComputerPlayer)Players[playerIndex]).GetMove(trick));
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
        void PlayNonHumans(ITrick trick);
        List<IDeck> Decks { get; }
        void RearrangePlayers();
    }
}