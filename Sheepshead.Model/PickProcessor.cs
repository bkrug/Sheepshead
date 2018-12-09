using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Model.Players;

namespace Sheepshead.Model
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
            if (picker != null)
                AcceptComputerPicker(deck, picker, handFactory);
            else if (picker == null && !deck.PlayersWithoutPickTurn.Any())
                AcceptLeasters(deck, handFactory);
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

        private void AcceptComputerPicker(IDeck deck, IComputerPlayer picker, IHandFactory handFactory)
        {
            var buriedCards = picker.DropCardsForPick(deck);
            deck.Buried = buriedCards;
            handFactory.GetHand(deck, picker, buriedCards);
            if (deck.Game.PlayerCount == 3 || picker.GoItAlone(deck))
                return;
            if (deck.Game.PartnerMethod == PartnerMethod.CalledAce)
            {
                var partnerCard = picker.ChooseCalledAce(deck);
                deck.SetPartnerCard(partnerCard);
            }
        }

        private void AcceptLeasters(IDeck deck, IHandFactory handFactory)
        {
            if (deck.Game.LeastersEnabled)
                handFactory.GetHand(deck, null, new List<SheepCard>());
        }

        /// <summary>
        /// Use this method to bury cards in a Jack-of-Diamonds game.
        /// </summary>
        public void BuryCards(IDeck deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone)
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
        public void BuryCards(IDeck deck, IHumanPlayer picker, List<SheepCard> cardsToBury, bool goItAlone, SheepCard partnerCard)
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
                hand = deck;
            }
            return hand;
        }
    }
}