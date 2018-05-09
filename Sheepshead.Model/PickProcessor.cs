using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public interface IPickProcessor
    {
        IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory);
    }

    public class PickProcessor : IPickProcessor
    {
        public PickProcessor()
        {
        }

        public IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory)
        {
            var picker = PlayNonHumanPickTurns(deck);
            IHand hand = null;
            if (picker != null)
                hand = AcceptComputerPicker(deck, picker, handFactory);
            else if (picker == null && !deck.PlayersWithoutPickTurn.Any())
                hand = handFactory.GetHand(deck, picker, new List<SheepCard>());
            return picker;
        }

        private IComputerPlayer PlayNonHumanPickTurns(IDeck deck)
        {
            if (!(deck.PlayersWithoutPickTurn.FirstOrDefault() is IComputerPlayer))
                throw new NotPlayersTurnException("Next player must be a computer player.");
            foreach (var player in deck.PlayersWithoutPickTurn.ToList())
            {
                var computerPlayer = player as IComputerPlayer;
                if (computerPlayer == null)
                    return null; //Must be human's turn.
                deck.PlayersWithoutPickTurn.Remove(player);
                if (computerPlayer.WillPick(deck))
                    return computerPlayer;
                else
                    deck.PlayersRefusingPick.Add(computerPlayer);
            }
            return null;
        }

        private IHand AcceptComputerPicker(IDeck deck, IComputerPlayer picker, IHandFactory handFactory)
        {
            var buriedCards = picker.DropCardsForPick(deck);
            deck.Buried = buriedCards;
            return handFactory.GetHand(deck, picker, buriedCards);
        }
    }
}