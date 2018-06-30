using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public interface IPickProcessor
    {
        IComputerPlayer PlayNonHumanPickTurns(IDeck deck, IHandFactory handFactory);
        void BuryCards(IDeck deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone);
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IDeck deck, IHandFactory handFactory, IPickProcessor pickProcessorOuter);
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

        public void BuryCards(IDeck deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone)
        {
            if (deck.Hand?.Picker != picker)
                throw new NotPlayersTurnException("A non-picker cannot bury cards.");
            cardsToBury.ForEach(c => picker.Cards.Remove(c));
            cardsToBury.ForEach(c => deck.Buried.Add(c));
            if (goItAlone)
                deck.Hand.GoItAlone();
        }

        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IDeck deck, IHandFactory handFactory, IPickProcessor pickProcessorOuter)
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