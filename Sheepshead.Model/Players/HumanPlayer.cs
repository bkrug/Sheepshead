using Sheepshead.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Model.Players
{
    public class HumanPlayer : Player, IHumanPlayer
    {
        public bool AssignedToClient { get; private set; } = false;
        public Guid Id { get; } = Guid.NewGuid();
        public Guid AssignToClient(string name)
        {
            AssignedToClient = true;
            Name = name;
            return Id;
        }

        public HumanPlayer(Participant participant) : base(participant)
        {
        }
    }

    public interface IHumanPlayer : IPlayer
    {
        bool AssignedToClient { get; }
        Guid Id { get; }
        Guid AssignToClient(string name);
    }
}