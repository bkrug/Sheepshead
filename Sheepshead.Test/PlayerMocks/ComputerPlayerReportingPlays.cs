using System;
using System.Collections.Generic;
using Sheepshead.Logic;
using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

namespace Sheepshead.Tests.PlayerMocks
{
    class ComputerPlayerReportingPlays : IComputerPlayer
    {
        public bool MadeMove { get; set; }
        private SheepCard _moveToMake;
        string IPlayer.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IReadOnlyList<SheepCard> IPlayer.Cards => throw new NotImplementedException();
        public Participant Participant { get; }

        public ComputerPlayerReportingPlays(SheepCard moveToMake)
        {
            Participant = new Participant();
            Participant.SetPlayer(this);
            _moveToMake = moveToMake;
        }

        public List<SheepCard> DropCardsForPick(IHand hand)
        {
            throw new NotImplementedException();
        }

        public SheepCard GetMove(ITrick trick)
        {
            MadeMove = true;
            return _moveToMake;
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
            throw new NotImplementedException();
        }

        public SheepCard? ChooseCalledAce(IHand hand)
        {
            throw new NotImplementedException();
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
