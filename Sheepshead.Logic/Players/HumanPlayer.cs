using Sheepshead.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model.Players
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
            Name = name;
            return Id;
        }

        public HumanPlayer(Participant participant) : base(participant)
        {
            Participant.Guid = Guid.NewGuid();
        }
    }

    public interface IHumanPlayer : IPlayer
    {
        bool AssignedToClient { get; }
        Guid Id { get; }
        Guid AssignToClient(string name);
    }
}