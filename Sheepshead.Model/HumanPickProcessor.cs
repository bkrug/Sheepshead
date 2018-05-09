using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;


namespace Sheepshead.Models
{
    public class HumanPickProcessor
    { 
        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IDeck deck, IHandFactory handFactory, 
            IPickProcessor pickProcessorOuter)
        {
            if (deck.PlayersWithoutPickTurn.FirstOrDefault() != human)
                throw new NotPlayersTurnException("This is not the player's turn to pick.");
            IHand hand;
            if (willPick)
            {
                human.Cards.AddRange(deck.Blinds);
                hand = handFactory.GetHand(deck, human, new List<SheepCard>());
            }
            else
            {
                deck.PlayersRefusingPick.Add(human);
                pickProcessorOuter.PlayNonHumanPickTurns(deck, handFactory);
                hand = deck.Hand;
            }
            return hand;
        }
    }
}