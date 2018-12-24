using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;

namespace Sheepshead.Model.Players
{
    public abstract class ComputerPlayer : Player, IComputerPlayer
    {
        public ComputerPlayer(Participant participant):base(participant) { }

        public abstract SheepCard GetMove(ITrick trick);

        public abstract bool WillPick(IHand hand);

        public abstract SheepCard? ChooseCalledAce(IHand hand);

        public List<SheepCard> DropCardsForPick(IHand hand)
        {
            foreach (var card in hand.Blinds.Where(c => !Cards.Contains(c)))
                AddCard(card);
            return DropCardsForPickInternal(hand);
        }

        public virtual bool GoItAlone(IHand hand)
        {
            return false;
        }

        protected abstract List<SheepCard> DropCardsForPickInternal(IHand hand);
    }

    public interface IComputerPlayer : IPlayer
    {
        SheepCard GetMove(ITrick trick);
        bool WillPick(IHand hand);
        List<SheepCard> DropCardsForPick(IHand hand);
        bool GoItAlone(IHand hand);
        SheepCard? ChooseCalledAce(IHand hand);
    }
}