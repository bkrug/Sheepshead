﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic.Models
{
    public partial class Hand : IHand
    {
        [NotMapped]
        public IPlayer StartingPlayer
        {
            get { return StartingParticipant?.Player; }
            protected set { StartingParticipant = value?.Participant; }
        }
        [NotMapped]
        public IPlayer Picker {
            get { return PickerParticipant?.Player; }
            protected set { PickerParticipant = value?.Participant; }
        }
        [NotMapped]
        public IPlayer Partner {
            get { return PartnerParticipant?.Player; }
            protected set { PartnerParticipant = value?.Participant; }
        }
        [NotMapped]
        public SheepCard? PartnerCardEnum {
            get { return CardUtil.GetCardFromAbbreviation(PartnerCard); }
            private set { PartnerCard = value.HasValue ? CardUtil.GetAbbreviation(value.Value) : string.Empty; }
        }
        [NotMapped]
        public IGame IGame
        {
            get { return Game; }
            protected set { Game = (Game)value; }
        }
        public IReadOnlyList<SheepCard> Blinds => CardUtil.StringToCardList(BlindCards);
        public IReadOnlyList<SheepCard> Buried => CardUtil.StringToCardList(BuriedCards);
        public List<ITrick> ITricks { get { return Tricks == null ? new List<ITrick>() : Tricks.OrderBy(t => t.SortOrder).OfType<ITrick>().ToList(); } }

        public int PlayerCount => IGame.PlayerCount;
        public List<IPlayer> Players => IGame.Players;
        /// <summary>
        /// Returns a list of players that haven't yet refused to pick (so the list may include the picker and the name is misleading). 
        /// </summary>
        public List<IPlayer> PlayersWithoutPickTurn => PlayerOrderer.PlayersWithoutTurn(Players, StartingPlayer, PlayersRefusingPick);
        public IReadOnlyList<IPlayer> PlayersRefusingPick => 
            PlayerOrderer.PlayersInTurnOrder(Players, StartingPlayer)
            .Where(p => ParticipantsRefusingPick.Any(prp => prp.Participant.Player == p))
            .ToList();
        [NotMapped]
        public IRandomWrapper _random { get; private set; }
        /// <summary>
        /// Returns true when there is no picker and Leasters is off.
        /// </summary>
        public bool MustRedeal => !IGame.LeastersEnabled && !PlayersWithoutPickTurn.Any();
        public bool Leasters { get { return Picker == null; } }

        public Hand(IGame game) : this(game, new RandomWrapper())
        {
        }

        public Hand(IGame game, IRandomWrapper random) : this()
        {
            if (!game.LastHandIsComplete())
                throw new PreviousHandIncompleteException("Cannot add a hand until the prvious one is complete.");
            IGame = game;
            IGame.Hands.Add(this);
            SortOrder = IGame?.IHands.Any() == true ? IGame.IHands.Last().SortOrder + 1 : 0;
            Tricks = new List<Trick>();
            _random = random;
            if (_random != null)
            {
                DealCards(ShuffleCards());
                SetStartingPlayer();
            }
            BuriedCards = string.Empty;
        }

        private Queue<SheepCard> ShuffleCards()
        {
            List<SheepCard> cards = CardUtil.UnshuffledList();
            for (var i = Game.CARDS_IN_DECK - 1; i > 0; --i)
            {
                var j = _random.Next(i);
                var swap = cards[i];
                cards[i] = cards[j];
                cards[j] = swap;
            }
            var queue = new Queue<SheepCard>();
            cards.ForEach(c => queue.Enqueue(c));
            return queue;
        }

        private void DealCards(Queue<SheepCard> cards)
        {
            BlindCards = string.Empty;
            foreach (var player in IGame.Players)
                player.RemoveAllCards();
            switch (IGame.PlayerCount)
            {
                case 3:
                    DealTwoCardsPerPlayer(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealTwoCardsPerPlayer(cards);
                    break;
                case 5:
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    DealOneBlind(cards);
                    DealTwoCardsPerPlayer(cards);
                    break;
            }
        }

        private void DealOneBlind(Queue<SheepCard> cards)
        {
            var newBlind = cards.Dequeue();
            var blindList = Blinds.ToList();
            blindList.Add(newBlind);
            BlindCards = CardUtil.CardListToString(blindList);
        }

        private void DealTwoCardsPerPlayer(Queue<SheepCard> cards)
        {
            foreach (var player in IGame.Players)
            {
                player.AddCard(cards.Dequeue());
                player.AddCard(cards.Dequeue());
            }
        }

        private void SetStartingPlayer()
        {
            var index = IGame.IHands.IndexOf(this);
            var indexOfPlayer = (index == 0)
                ? _random.Next(IGame.PlayerCount)
                : IGame.Players.IndexOf(IGame.IHands.ElementAt(index - 1).StartingPlayer) + 1;
            if (indexOfPlayer == IGame.PlayerCount) indexOfPlayer = 0;
            StartingPlayer = IGame.Players[indexOfPlayer];
        }

        public void AddBuried(SheepCard card) {
            var buriedList = Buried.ToList();
            buriedList.Add(card);
            BuriedCards = CardUtil.CardListToString(buriedList);
        }

        public void PlayerWontPick(IPlayer player)
        {
            ParticipantsRefusingPick = ParticipantsRefusingPick ?? new List<ParticipantRefusingPick>();
            ParticipantsRefusingPick.Add(new ParticipantRefusingPick() {
                Participant = player.Participant,
                Hand = this
            });
        }

        public void SetPicker(IPlayer picker, List<SheepCard> burried)
        {
            PickPhaseComplete = true;
            Picker = picker;
            if (picker != null)
            {
                HandUtils.BuryCards(this, picker, burried);
                PartnerCardEnum = HandUtils.ChoosePartnerCard(this, picker);
            }
        }

        public void GoItAlone()
        {
            PartnerCardEnum = null;
        }

        public void SetPartner(IPlayer partner, ITrick trick)
        {
            Partner = partner;
        }

        public void SetPartnerCard(SheepCard? sheepCard)
        {
            if (IGame.PartnerMethodEnum != PartnerMethod.CalledAce)
                throw new InvalidOperationException("The method SetPartnerCard() is only for 'called ace' games. The partner card is assigned automatically for 'jack of diamonds' games.");
            PartnerCardEnum = sheepCard;
        }

        public IPlayer PresumedParnter
        {
            get
            {
                if (!PartnerCardEnum.HasValue)
                    return null;
                if (Partner != null)
                    return Partner;
                var potentialPartnerGroups = ITricks
                    .Where(t =>
                        t.CardsByPlayer.First().Key != Picker
                        && CardUtil.GetSuit(t.CardsByPlayer.First().Value) == Suit.TRUMP
                    )
                    .Select(t => t.CardsByPlayer.First().Key)
                    .GroupBy(p => p)
                    .OrderByDescending(g => g.Count())
                    .ToList();
                if (!potentialPartnerGroups.Any()
                    || potentialPartnerGroups.Count >= 2 && potentialPartnerGroups[0].Count() == potentialPartnerGroups[1].Count())
                    return null;
                else
                    return potentialPartnerGroups.First().Key;  
            }
        }

        public void AddTrick(ITrick trick)
        {
            if (trick is Trick)
                Tricks.Add((Trick)trick);
        }

        private HandScores _scores = null;
        public HandScores CalculateScores()
        {
            if (ITricks.Count == IGame.TrickCount && ITricks.Last().IsComplete())
                return _scores = (_scores ?? ScoreCalculator.GetScores(this));
            return null;
        }

        public bool IsComplete()
        {
            if (MustRedeal)
                return true;
            return Tricks.Count() == IGame.TrickCount && Tricks.Last().IsComplete();
        }
    }

    public interface IHand
    {
        int Id { get; }
        IGame IGame { get; }
        int SortOrder { get; set; }
        List<IPlayer> Players { get; }
        IPlayer StartingPlayer { get; }
        IPlayer Picker { get; }
        IPlayer Partner { get; }
        SheepCard? PartnerCardEnum { get; }
        bool Leasters { get; }
        IReadOnlyList<SheepCard> Blinds { get; }
        IReadOnlyList<SheepCard> Buried { get; }
        IReadOnlyList<IPlayer> PlayersRefusingPick { get; }
        List<ITrick> ITricks { get; }
        ICollection<Score> ScoreList { get; set; }

        int PlayerCount { get; }
        List<IPlayer> PlayersWithoutPickTurn { get; }
        IPlayer PresumedParnter { get; }
        bool MustRedeal { get; }

        void PlayerWontPick(IPlayer player);
        void SetPicker(IPlayer picker, List<SheepCard> burried);
        bool PickPhaseComplete { get; }
        void AddBuried(SheepCard card);
        void SetPartner(IPlayer partner, ITrick trick);
        void GoItAlone();
        void SetPartnerCard(SheepCard? sheepCard);
        void AddTrick(ITrick trick);
        bool IsComplete();
        HandScores CalculateScores();
    }

    public static class HandUtils
    {
        public static void BuryCards(IHand hand, IPlayer picker, List<SheepCard> burried)
        {
            if (picker != null)
            {
                hand.Blinds.ToList().ForEach(c => {
                    if (!picker.Cards.Contains(c))
                        picker.AddCard(c);
                });
                burried.ForEach(c => {
                    if (picker.Cards.Contains(c))
                        picker.RemoveCard(c);
                });
                burried.ForEach(c => hand.AddBuried(c));
            }
        }

        public static SheepCard? ChoosePartnerCard(IHand hand, IPlayer picker)
        {
            if (hand.IGame.PlayerCount == 3 || hand.IGame.PartnerMethodEnum == PartnerMethod.CalledAce)
                return null;
            var potentialPartnerCards = new[] {
                SheepCard.JACK_DIAMONDS,
                SheepCard.JACK_HEARTS,
                SheepCard.JACK_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.QUEEN_HEARTS,
                SheepCard.QUEEN_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var pickerDoesNotHave = potentialPartnerCards.Where(c => 
                !picker.Cards.Contains(c) 
                && !hand.Blinds.Contains(c) 
                && !hand.Buried.Contains(c));
            return pickerDoesNotHave.Any() ? pickerDoesNotHave.First() : (SheepCard?)null;
        }
    }

    public class PreviousHandIncompleteException : Exception
    {
        public PreviousHandIncompleteException(string message) : base(message) { }
    }
}