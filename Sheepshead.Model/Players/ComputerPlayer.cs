using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Model.Players
{
    public abstract class ComputerPlayer : Player, IComputerPlayer
    {
        public abstract SheepCard GetMove(ITrick trick);

        public abstract bool WillPick(IHand deck);

        public abstract SheepCard? ChooseCalledAce(IHand deck);

        public List<SheepCard> DropCardsForPick(IHand deck)
        {
            foreach (var card in deck.Blinds.Where(c => !Cards.Contains(c)))
                Cards.Add(card);
            return DropCardsForPickInternal(deck);
        }

        public virtual bool GoItAlone(IHand deck)
        {
            return false;
        }

        protected abstract List<SheepCard> DropCardsForPickInternal(IHand deck);
    }

    public interface IComputerPlayer : IPlayer
    {
        SheepCard GetMove(ITrick trick);
        bool WillPick(IHand deck);
        List<SheepCard> DropCardsForPick(IHand deck);
        bool GoItAlone(IHand deck);
        SheepCard? ChooseCalledAce(IHand deck);
    }
}