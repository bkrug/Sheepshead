using System;
using System.Collections.Generic;
using Sheepshead.Model;
using Sheepshead.Model.Models;
using Sheepshead.Model.Players;

namespace Sheepshead.Tests.PlayerMocks
{
    class ComputerPlayerPickingMock : IComputerPlayer
    {
        private bool _doesPick;
        private SheepCard? _calledAceCard;
        string IPlayer.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IReadOnlyList<SheepCard> IPlayer.Cards => throw new NotImplementedException();

        public ComputerPlayerPickingMock(bool doesPick, SheepCard? calledAceCard = null)
        {
            _doesPick = doesPick;
            _calledAceCard = calledAceCard;
        }

        public List<SheepCard> DropCardsForPick(IHand hand)
        {
            return new List<SheepCard>() { 0, (SheepCard)1 };
        }

        public SheepCard GetMove(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public int QueueRankInHand(IHand hand)
        {
            throw new NotImplementedException();
        }

        public int QueueRankInTrick(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public bool WillPick(IHand hand)
        {
            return _doesPick;
        }

        public SheepCard? ChooseCalledAce(IHand hand)
        {
            return _calledAceCard;
        }

        public List<SheepCard> LegalCalledAces(IHand hand)
        {
            throw new NotImplementedException();
        }

        public bool GoItAlone(IHand hand)
        {
            return false;
        }

        public void AddCard(SheepCard card)
        {
            throw new NotImplementedException();
        }

        public void RemoveCard(SheepCard card)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllCards()
        {
            throw new NotImplementedException();
        }

        public void AddCardRange(List<SheepCard> cards)
        {
            throw new NotImplementedException();
        }
    }
}
