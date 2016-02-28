using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public interface IPickProcessorOuter
    {
        IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory, ILearningHelperFactory learningHelperFactory);
    }

    public class PickProcessorOuter : IPickProcessorOuter
    {
        public IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory, ILearningHelperFactory learningHelperFactory)
        {
            var picker = deck.PickProcessor.PlayNonHumanPickTurns();
            IHand hand = null;
            if (picker != null)
                hand = deck.PickProcessor.AcceptComputerPicker(picker);
            else if (picker == null && !deck.PlayersWithoutPickTurn.Any())
                hand = handFactory.GetHand(deck, picker, new List<ICard>());
            if (hand != null)
                learningHelperFactory.GetLearningHelper(hand, SaveLocations.HAND_SUMMARY);
            return picker;
        }
    }

    public class PickProcessorOuter2
    { 
        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IDeck deck, IHandFactory handFactory, 
            ILearningHelperFactory learningHelperFactory, IPickProcessorOuter pickProcessorOuter)
        {
            if (deck.PlayersWithoutPickTurn.FirstOrDefault() != human)
                throw new NotPlayersTurnException("This is not the player's turn to pick.");
            IHand hand;
            if (willPick)
            {
                human.Cards.AddRange(deck.Blinds);
                hand = handFactory.GetHand(deck, human, new List<ICard>());
                if (hand != null)
                    learningHelperFactory.GetLearningHelper(hand, SaveLocations.HAND_SUMMARY);
            }
            else
            {
                deck.PlayersRefusingPick.Add(human);
                pickProcessorOuter.PlayNonHumanPickTurns(deck, handFactory, learningHelperFactory);
                hand = deck.Hand;
            }
            return hand;
        }
    }
}