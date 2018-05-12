using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public class HumanPlayer : Player, IHumanPlayer
    {
        public HumanPlayer()
        {
        }

        public override string Name { get; set; }
    }

    public interface IHumanPlayer : IPlayer
    {
    }
}