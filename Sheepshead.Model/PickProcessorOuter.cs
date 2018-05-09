﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;


namespace Sheepshead.Models
{
    public interface IPickProcessorOuter
    {
        IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory);
        void BuryCards(IDeck deck, IHumanPlayer picker, List<SheepCard> cardsToBury);
    }

    public class PickProcessorOuter : IPickProcessorOuter
    {
        public IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory)
        {
            var pickProcessor = new PickProcessor(deck, handFactory);
            var picker = pickProcessor.PlayNonHumanPickTurns();
            IHand hand = null;
            if (picker != null)
                hand = pickProcessor.AcceptComputerPicker(picker);
            else if (picker == null && !deck.PlayersWithoutPickTurn.Any())
                hand = handFactory.GetHand(deck, picker, new List<SheepCard>());
            return picker;
        }

        public void BuryCards(IDeck deck, IHumanPlayer picker, List<SheepCard> cardsToBury)
        {
            if (deck.Hand?.Picker != picker)
                throw new NotPlayersTurnException("A non-picker cannot bury cards.");
            cardsToBury.ForEach(c => picker.Cards.Remove(c));
            cardsToBury.ForEach(c => deck.Buried.Add(c));
        }
    }

    public class HumanPickProcessor
    { 
        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IDeck deck, IHandFactory handFactory, 
            IPickProcessorOuter pickProcessorOuter)
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