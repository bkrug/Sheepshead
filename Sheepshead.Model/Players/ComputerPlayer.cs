using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public abstract class ComputerPlayer : Player, IComputerPlayer
    {
        public abstract SheepCard GetMove(ITrick trick);

        public abstract bool WillPick(IDeck deck);

        public abstract SheepCard? ChooseCalledAce(IDeck deck);

        public List<SheepCard> DropCardsForPick(IDeck deck)
        {
            foreach (var card in deck.Blinds.Where(c => !Cards.Contains(c)))
                Cards.Add(card);
            return DropCardsForPickInternal(deck);
        }

        protected abstract List<SheepCard> DropCardsForPickInternal(IDeck deck);
    }

    public interface IComputerPlayer : IPlayer
    {
        SheepCard GetMove(ITrick trick);
        bool WillPick(IDeck deck);
        List<SheepCard> DropCardsForPick(IDeck deck);
        SheepCard? ChooseCalledAce(IDeck deck);
    }
}