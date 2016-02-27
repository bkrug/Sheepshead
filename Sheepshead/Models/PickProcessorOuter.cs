using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public class PickProcessorOuter
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
                learningHelperFactory.GetLearningHelper(hand, SaveLocations.FIRST_SAVE);
            return picker;
        }
    }
}