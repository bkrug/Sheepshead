using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    public interface IPickProcessor
    {
        IComputerPlayer PlayNonHumanPickTurns(IHand deck, IHandFactory handFactory);
        void BuryCards(IHand deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone);
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IHand deck, IHandFactory handFactory, IPickProcessor pickProcessorOuter);
    }

    public class PickProcessor : IPickProcessor
    {
        public PickProcessor()
        {
        }

        public IComputerPlayer PlayNonHumanPickTurns(IHand deck, IHandFactory handFactory)
        {
            var picker = PlayNonHumanPickTurns(deck);
            if (picker != null)
                AcceptComputerPicker(deck, picker, handFactory);
            else if (picker == null && !deck.PlayersWithoutPickTurn.Any())
                AcceptLeasters(deck, handFactory);
            return picker;
        }

        private IComputerPlayer PlayNonHumanPickTurns(IHand deck)
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

        private void AcceptComputerPicker(IHand deck, IComputerPlayer picker, IHandFactory handFactory)
        {
            var buriedCards = picker.DropCardsForPick(deck);
            //TODO: set the buried property from within SetPicker
            deck.Buried = buriedCards;
            deck.SetPicker(picker, buriedCards);
            if (deck.Game.PlayerCount == 3 || picker.GoItAlone(deck))
                return;
            if (deck.Game.PartnerMethod == PartnerMethod.CalledAce)
            {
                var partnerCard = picker.ChooseCalledAce(deck);
                deck.SetPartnerCard(partnerCard);
            }
        }

        private void AcceptLeasters(IHand deck, IHandFactory handFactory)
        {
            if (deck.Game.LeastersEnabled)
                deck.SetPicker(null, new List<SheepCard>());
        }

        /// <summary>
        /// Use this method to bury cards in a Jack-of-Diamonds game.
        /// </summary>
        public void BuryCards(IHand deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone)
        {
            if (deck.Picker != picker)
                throw new NotPlayersTurnException("A non-picker cannot bury cards.");
            cardsToBury.ForEach(c => picker.Cards.Remove(c));
            cardsToBury.ForEach(c => deck.Buried.Add(c));
            if (goItAlone)
                deck.GoItAlone();
        }

        /// <summary>
        /// Use this method to bury cards in a Called-Ace game.
        /// </summary>
        public void BuryCards(IHand deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone, SheepCard partnerCard)
        {
            if (picker.Cards.Contains(partnerCard))
                throw new ArgumentException("Picker has the parner card");
            if (!picker.Cards.Any(c => CardUtil.GetSuit(c) == CardUtil.GetSuit(partnerCard)))
                throw new ArgumentException($"Picker does not have a card in the {CardUtil.GetSuit(partnerCard).ToString()} suit");
            if (!_validCalledAceCards.Contains(partnerCard))
                throw new ArgumentException($"{CardUtil.ToAbbr(partnerCard)} is not a valid partner card.");
            deck.SetPartnerCard(partnerCard);
            BuryCards(deck, picker, cardsToBury, goItAlone);
        }
        private static List<SheepCard> _validCalledAceCards = new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_HEARTS, SheepCard.ACE_SPADES };

        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick, IHand hand, IHandFactory handFactory, IPickProcessor pickProcessorOuter)
        {
            if (hand.PlayersWithoutPickTurn.FirstOrDefault() != human)
                throw new NotPlayersTurnException("This is not the player's turn to pick.");
            if (willPick)
            {
                human.Cards.AddRange(hand.Blinds);
                hand.SetPicker(human, new List<SheepCard>());
            }
            else
            {
                hand.PlayersRefusingPick.Add(human);
                pickProcessorOuter.PlayNonHumanPickTurns(hand, handFactory);
            }
            return hand;
        }
    }
}