using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public interface IPickProcessor
    {
        IHand AcceptComputerPicker(IComputerPlayer picker);
        void LetHumanPick(IHumanPlayer human, bool willPick);
        IPlayer PlayNonHumanPickTurns();
    }

    public class PickProcessor : IPickProcessor
    {
        private IDeck _deck;
        private IHandFactory _handFactory;

        public PickProcessor(IDeck currentDeck, IHandFactory handFactory)
        {
            _deck = currentDeck;
            _handFactory = handFactory;
        }

        public void LetHumanPick(IHumanPlayer human, bool willPick)
        {
            if (_deck.PlayersWithoutPickTurn.FirstOrDefault() != human)
                throw new NotPlayersTurnException("This is not the player's turn to pick.");
            if (willPick)
                human.Cards.AddRange(_deck.Blinds);
            else
                _deck.PlayersRefusingPick.Add(human);
        }

        public IPlayer PlayNonHumanPickTurns()
        {
            if (!(_deck.PlayersWithoutPickTurn.FirstOrDefault() is IComputerPlayer))
                throw new NotPlayersTurnException("Next player must be a computer player.");
            foreach (var player in _deck.PlayersWithoutPickTurn)
            {
                var computerPlayer = player as IComputerPlayer;
                if (computerPlayer == null)
                    return null; //Must be human's turn.
                if (computerPlayer.WillPick(_deck))
                    return computerPlayer;
                else
                    _deck.PlayersRefusingPick.Add(computerPlayer);
            }
            return null;
        }

        public IHand AcceptComputerPicker(IComputerPlayer picker)
        {
            var buriedCards = picker.DropCardsForPick(_deck);
            return _handFactory.GetHand(_deck, picker, buriedCards);
        }
    }
}