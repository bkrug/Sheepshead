using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Deck : IDeck
    {
        public IGame Game { get; private set; }

        public Deck(IGame game)
        {
            Game = game;
        }
    }

    public interface IDeck
    {
        IGame Game { get; }
    }
}