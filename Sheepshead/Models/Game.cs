using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Game : LongId, IGame
    {
        public string Name { get; set; }
        public int NumberOfPlayers { get; set; }
        public int HumanPlayers { get; set; }

        public Game(long id)
        {
            _id = id;
        }
    }

    public interface IGame : ILongId
    {
        string Name { get; set; }
        int NumberOfPlayers { get; set; }
        int HumanPlayers { get; set; }
    }
}