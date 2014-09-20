using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Game : LongId, IGame
    {
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int PlayerCount { get { return _players.Count(); } }
        public int MaxHumanPlayers { get; set; }
        public int HumanPlayerCount { get { return _players.Count(p => p is IHumanPlayer); } }
        protected List<IPlayer> _players = new List<IPlayer>();
        public List<IPlayer> Players { get { return _players.ToList(); } }

        public Game(long id)
        {
            _id = id;
        }

        public void AddPlayer(IPlayer player)
        {
            if (_players.Contains(player))
                throw new ObjectInListException("Player is already in list.");
            if (PlayerCount >= MaxPlayers)
                throw new TooManyPlayersException("This game allows a maximum of " + MaxPlayers + " players.");
            if (HumanPlayerCount >= MaxHumanPlayers)
                throw new TooManyHumanPlayersException("This game allows a maximum of " + MaxHumanPlayers + " human players.");
            _players.Add(player);
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
        int MaxPlayers { get; set; }
        int MaxHumanPlayers { get; set; }
        int HumanPlayerCount { get; }
        int PlayerCount { get; }
        void AddPlayer(IPlayer player);
        List<IPlayer> Players { get; }
    }
}