using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic
{
    public interface IPickProcessor
    {
        IComputerPlayer PlayNonHumanPickTurns(IHand hand);
        void BuryCards(IHand hand, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone);
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IHand hand, IPickProcessor pickProcessorOuter);
    }

    public class PickProcessor : IPickProcessor
    {
        public PickProcessor()
        {
        }

        public IComputerPlayer PlayNonHumanPickTurns(IHand hand)
        {
            var picker = PlayNonHumanPickTurnsPrivate(hand);
            if (picker != null)
                AcceptComputerPicker(hand, picker);
            else if (picker == null && !hand.PlayersWithoutPickTurn.Any())
                AcceptLeasters(hand);
            return picker;
        }

        private IComputerPlayer PlayNonHumanPickTurnsPrivate(IHand hand)
        {
            foreach (var player in hand.PlayersWithoutPickTurn.ToList())
            {
                var computerPlayer = player as IComputerPlayer;
                if (computerPlayer == null)
                    return null; //Must be human's turn.
                hand.PlayersWithoutPickTurn.Remove(player);
                if (computerPlayer.WillPick(hand))
                    return computerPlayer;
                else
                    hand.PlayerWontPick(computerPlayer);
            }
            return null;
        }

        private void AcceptComputerPicker(IHand hand, IComputerPlayer picker)
        {
            var buriedCards = picker.DropCardsForPick(hand);
            //TODO: set the buried property from within SetPicker
            hand.AddBuried(buriedCards[0]);
            hand.AddBuried(buriedCards[1]);
            hand.SetPicker(picker, buriedCards);
            if (hand.IGame.PlayerCount == 3 || picker.GoItAlone(hand))
                return;
            if (hand.IGame.PartnerMethodEnum == PartnerMethod.CalledAce)
            {
                var partnerCard = picker.ChooseCalledAce(hand);
                hand.SetPartnerCard(partnerCard);
            }
        }

        private void AcceptLeasters(IHand hand)
        {
            if (hand.IGame.LeastersEnabled)
                hand.SetPicker(null, new List<SheepCard>());
        }

        /// <summary>
        /// Use this method to bury cards in a Jack-of-Diamonds game.
        /// </summary>
        public void BuryCards(IHand hand, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone)
        {
            if (hand.Picker != picker)
                throw new NotPlayersTurnException("A non-picker cannot bury cards.");
            cardsToBury.ForEach(c => picker.RemoveCard(c));
            cardsToBury.ForEach(c => hand.AddBuried(c));
            if (goItAlone)
                hand.GoItAlone();
        }

        /// <summary>
        /// Use this method to bury cards in a Called-Ace game.
        /// </summary>
        public void BuryCards(IHand hand, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone, SheepCard partnerCard)
        {
            if (picker.Cards.Contains(partnerCard))
                throw new ArgumentException("Picker has the parner card");
            if (!picker.Cards.Any(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(partnerCard)))
                throw new ArgumentException($"Picker does not have a card in the {CardUtil.GetSuit(partnerCard).ToString()} suit");
            if (!_validCalledAceCards.Contains(partnerCard))
                throw new ArgumentException($"{CardUtil.GetAbbreviation(partnerCard)} is not a valid partner card.");
            hand.SetPartnerCard(partnerCard);
            BuryCards(hand, picker, cardsToBury, goItAlone);
        }
        private static List<SheepCard> _validCalledAceCards = new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_HEARTS, SheepCard.ACE_SPADES };

        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IHand hand, IPickProcessor pickProcessorOuter)
        {
            if (hand.PlayersWithoutPickTurn.FirstOrDefault() != human)
                throw new NotPlayersTurnException("This is not the player's turn to pick.");
            if (willPick)
            {
                hand.Blinds.ToList().ForEach(c => human.AddCard(c));
                hand.SetPicker(human, new List<SheepCard>());
            }
            else
            {
                hand.PlayerWontPick(human);
                pickProcessorOuter.PlayNonHumanPickTurns(hand);
            }
            return hand;
        }
    }
}