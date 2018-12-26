using Sheepshead.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Logic.Players
{
    public class HumanPlayer : Player, IHumanPlayer
    {
        public bool AssignedToClient {
            get { return Participant.AssignedToClient; }
            private set { Participant.AssignedToClient = value; } }
        public Guid Id => Participant.Guid;
        public Guid AssignToClient(string name)
        {
            AssignedToClient = true;
            Participant.Guid = Guid.NewGuid();
            Name = name;
            return Participant.Guid;
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