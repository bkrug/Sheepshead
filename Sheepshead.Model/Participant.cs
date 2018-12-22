using System;
using Sheepshead.Model.Players;

namespace Sheepshead.Model.Models
{
    public partial class Participant
    {
        private IPlayer _player;
        public IPlayer Player => _player ?? (_player = InitPlayer());
        private IPlayer InitPlayer()
        {
            switch (Type)
            {
                case "S":
                    return new SimplePlayer();
                case "I":
                    return new IntermediatePlayer();
                case "A":
                    return new AdvancedPlayer();
                default:
                    return new HumanPlayer();
            }
        }
    }
}