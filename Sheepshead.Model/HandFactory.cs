using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    public interface IHandFactory
    {
        IHand GetHand(IDeck deck, IPlayer picker, List<SheepCard> buried);
    }

    public class HandFactory : IHandFactory
    {
        public IHand GetHand(IDeck deck, IPlayer picker, List<SheepCard> buried)
        {
            return new Hand(deck, picker, buried);
        }
    }
}