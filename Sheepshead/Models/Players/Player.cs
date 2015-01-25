using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players
{
    public class Player : IPlayer
    {
        private List<ICard> _hand = new List<ICard>();
        public virtual string Name { get { return String.Empty; } }
        public List<ICard> Cards { get { return _hand; } }
    }

    public interface IPlayer
    {
        string Name { get; }
        List<ICard> Cards { get; }
    }
}