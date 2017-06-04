using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public abstract class ComputerPlayer : Player, IComputerPlayer
    {
        public abstract ICard GetMove(ITrick trick);

        public abstract bool WillPick(IDeck deck);

        public List<ICard> DropCardsForPick(IDeck deck)
        {
            foreach (var card in deck.Blinds.Where(c => !Cards.Contains(c)))
                Cards.Add(card);
            return DropCardsForPickInternal(deck);
        }

        protected abstract List<ICard> DropCardsForPickInternal(IDeck deck);
    }

    public interface IComputerPlayer : IPlayer
    {
        ICard GetMove(ITrick trick);
        bool WillPick(IDeck deck);
        List<ICard> DropCardsForPick(IDeck deck);
    }
}