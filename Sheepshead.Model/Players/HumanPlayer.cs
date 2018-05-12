using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public class HumanPlayer : Player, IHumanPlayer
    {
        public bool AssignedToClient { get; private set; } = false;
        public Guid Id { get; } = Guid.NewGuid();
        public Guid AssignToClient()
        {
            AssignedToClient = true;
            return Id;
        }

        public HumanPlayer()
        {
        }
    }

    public interface IHumanPlayer : IPlayer
    {
        bool AssignedToClient { get; }
        Guid Id { get; }
        Guid AssignToClient();
    }
}