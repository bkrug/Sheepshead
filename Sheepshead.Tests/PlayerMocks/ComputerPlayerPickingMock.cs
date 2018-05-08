using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests.PlayerMocks
{
    class ComputerPlayerPickingMock : IComputerPlayer
    {
        private bool _doesPick;
        public string Name => throw new NotImplementedException();

        public List<SheepCard> Cards => throw new NotImplementedException();

        public ComputerPlayerPickingMock(bool doesPick)
        {
            _doesPick = doesPick;
        }

        public List<SheepCard> DropCardsForPick(IDeck deck)
        {
            return new List<SheepCard>();
        }

        public SheepCard GetMove(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public int QueueRankInDeck(IDeck deck)
        {
            throw new NotImplementedException();
        }

        public int QueueRankInTrick(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public bool WillPick(IDeck deck)
        {
            return _doesPick;
        }
    }
}
