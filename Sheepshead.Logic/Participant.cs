using System;
using Sheepshead.Model.Players;

namespace Sheepshead.Logic.Models
{
    public partial class Participant
    {
        public const string TYPE_HUMAN = "H";
        public const string TYPE_SIMPLE = "S";
        public const string TYPE_INTERMEDIATE = "I";
        public const string TYPE_ADVANCED = "A";

        private IPlayer _player;
        public IPlayer Player => _player ?? (_player = InitPlayer());
        private IPlayer InitPlayer()
        {
            switch (Type)
            {
                case TYPE_SIMPLE:
                    return new SimplePlayer(this);
                case TYPE_INTERMEDIATE:
                    return new IntermediatePlayer(this);
                case TYPE_ADVANCED:
                    return new AdvancedPlayer(this);
                default:
                    return new HumanPlayer(this);
            }
        }
    }
}