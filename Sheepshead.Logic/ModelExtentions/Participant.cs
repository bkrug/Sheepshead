using System;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic.Models
{
    public partial class Participant
    {
        public const string TYPE_HUMAN = "H";
        public const string TYPE_SIMPLE = "S";
        public const string TYPE_INTERMEDIATE = "I";
        public const string TYPE_ADVANCED = "A";
        public const string TYPE_EXPERIMENTAL = "X";

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
                case TYPE_EXPERIMENTAL:
                    throw new Exception("No experimental player is currently known to exist.");
                default:
                    return new HumanPlayer(this);
            }
        }

        public void SetPlayer(IPlayer player)
        {
            _player = player;
        }
    }
}