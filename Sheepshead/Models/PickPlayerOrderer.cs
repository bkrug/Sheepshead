using Sheepshead.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public interface IPickPlayerOrdererInner
    {
        List<IPlayer> PlayersInPickOrder { get; }
        List<IPlayer> PlayersRefusingPick { get; }
    }

    public interface IPickPlayerOrderer
    {
        List<IPlayer> PlayersInPickOrder { get; }
        List<IPlayer> PlayersWithoutPickTurn { get; }
    }

    public class PickPlayerOrdererInner : IPickPlayerOrdererInner
    {
        private IDeck _deck;

        public PickPlayerOrdererInner(IDeck deck)
        {
            _deck = deck;
        }

        public List<IPlayer> PlayersInPickOrder
        {
            get {
                var startIndex = _deck.Players.IndexOf(_deck.StartingPlayer);
                return _deck.Players.Skip(startIndex).Union(_deck.Players.Take(startIndex)).ToList();
            }
        }

        public List<IPlayer> PlayersRefusingPick
        {
            get { return _deck.PlayersRefusingPick;  }
        }
    }

    public class PickPlayerOrderer : IPickPlayerOrderer
    {
        private IPickPlayerOrdererInner _pickPlayerOrderer;

        public PickPlayerOrderer(IPickPlayerOrdererInner pickPlayerOrderer)
        {
            _pickPlayerOrderer = pickPlayerOrderer;
        }

        public List<IPlayer> PlayersInPickOrder
        {
            get { return _pickPlayerOrderer.PlayersInPickOrder; }
        }

        public List<IPlayer> PlayersWithoutPickTurn
        {
            get
            {
                var finishedCount = _pickPlayerOrderer.PlayersRefusingPick.Count();
                return _pickPlayerOrderer.PlayersInPickOrder.Skip(finishedCount).ToList();
            }
        }
    }
}