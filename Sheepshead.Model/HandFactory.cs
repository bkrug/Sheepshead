using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    public interface IHandFactory
    {
        IHand GetHand(IHand deck, IPlayer picker, List<SheepCard> buried);
    }

    public class HandFactory : IHandFactory
    {
        public IHand GetHand(IHand deck, IPlayer picker, List<SheepCard> buried)
        {
            deck.SetPicker(picker, buried);
            return deck;
        }
    }
}