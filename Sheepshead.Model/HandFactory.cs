using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public interface IHandFactory
    {
        IHand GetHand(IDeck deck, IPlayer picker, List<ICard> buried);
    }

    public class HandFactory : IHandFactory
    {
        public IHand GetHand(IDeck deck, IPlayer picker, List<ICard> buried)
        {
            return new Hand(deck, picker, buried);
        }
    }
}