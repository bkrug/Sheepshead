using System;
using System.Collections.Generic;
using Sheepshead.Model;
using Sheepshead.Model.Players;

namespace Sheepshead.Tests.PlayerMocks
{
    class ComputerPlayerPickingMock : IComputerPlayer
    {
        private bool _doesPick;
        private SheepCard? _calledAceCard;
        public string Name => throw new NotImplementedException();

        public List<SheepCard> Cards => throw new NotImplementedException();

        string IPlayer.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ComputerPlayerPickingMock(bool doesPick, SheepCard? calledAceCard = null)
        {
            _doesPick = doesPick;
            _calledAceCard = calledAceCard;
        }

        public List<SheepCard> DropCardsForPick(IDeck deck)
        {
            return new List<SheepCard>() { 0, (SheepCard)1 };
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

        public SheepCard? ChooseCalledAce(IDeck deck)
        {
            return _calledAceCard;
        }

        public List<SheepCard> LegalCalledAces(IDeck deck)
        {
            throw new NotImplementedException();
        }

        public bool GoItAlone(IDeck deck)
        {
            return false;
        }
    }
}
