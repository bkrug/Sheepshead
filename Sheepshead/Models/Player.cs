using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Player : IPlayer
    {
        private List<Card> _hand = new List<Card>();
        public string Name { get { return String.Empty; } }
        public List<Card> Cards { get { return _hand; } }
    }

    public interface IPlayer
    {
        string Name { get; }
        List<Card> Cards { get; }
    }
}