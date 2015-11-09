using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models.Players.Stats
{
    public interface IMoveKeyGenerator
    {
        MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard);
    }

    public class MoveKeyGenerator : BaseMoveKeyGenerator, IMoveKeyGenerator
    {
        public MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard)
        {
            var indexOfTrick = trick.Hand.Tricks.IndexOf(trick);
            var queueRankOfPlayer = player.QueueRankInTrick(trick);
            var previousTricks = trick.Hand.Tricks.Take(indexOfTrick).ToList();
            var startSuit = trick.OrderedMoves.Any() ? (Suit?)CardRepository.GetSuit(trick.OrderedMoves.First().Value) : null;
            var curWinPair = GetCurrentWinner(trick, player, legalCard, queueRankOfPlayer, startSuit);
            var isOffenseSide = trick.Hand.Picker == player || trick.Hand.Partner == player || player.Cards.Contains(trick.PartnerCard);
            var onWinningSide = OnWinningSide(trick, player, curWinPair.Key, isOffenseSide);
            var cardWillOverpower = CardWillOverpower(legalCard, startSuit, onWinningSide, curWinPair.Value);
            var willBeOnWinningSide = onWinningSide || cardWillOverpower;
            return new MoveStatUniqueKey()
            {
                CardWillOverpower = cardWillOverpower,
                OpponentPercentDone = OpponentPortionDone(queueRankOfPlayer, trick, player, isOffenseSide, indexOfTrick),
                CardPoints = (willBeOnWinningSide ? 1 : -1) * legalCard.Points,
                UnknownStrongerCards = StrongerUnknownCards(trick, player, legalCard, queueRankOfPlayer, previousTricks),
                HeldStrongerCards = StrongerHeldCards(player, legalCard, previousTricks, trick)
            };
        }

        private KeyValuePair<IPlayer, ICard> GetCurrentWinner(ITrick trick, IPlayer player, ICard legalCard, int queueRankOfPlayer, Suit? startSuit)
        {
            var precedingMoves = trick.OrderedMoves.Take(queueRankOfPlayer - 1).ToList();
            if (!precedingMoves.Any())
                return new KeyValuePair<IPlayer, ICard>(null, null);
            var curWinPair = precedingMoves
                                .Where(c => CardRepository.GetSuit(c.Value) == startSuit || CardRepository.GetSuit(c.Value) == Suit.TRUMP)
                                .OrderBy(c => c.Value.Rank)
                                .FirstOrDefault();
            return curWinPair;
        }

        private static bool OnWinningSide(ITrick trick, IPlayer player, IPlayer curWinner, bool isOffenseSide)
        {
            var winnerIsOffense = trick.Hand.Picker == curWinner || trick.Hand.Partner == curWinner;
            var onWinningSide = curWinner != null && winnerIsOffense == isOffenseSide;
            return onWinningSide;
        }

        private static bool CardWillOverpower(ICard legalCard, Suit? startSuit, bool onWinningSide, ICard winningCard)
        {
            if (onWinningSide)
                return false;
            if (winningCard == null)
                return true;
            var cardSuite = CardRepository.GetSuit(legalCard);
            var cardWillOverpower = !onWinningSide && legalCard.Rank < winningCard.Rank && (cardSuite == Suit.TRUMP || cardSuite == startSuit);
            return cardWillOverpower;
        }

        private static int OpponentPortionDone(int queueRankOfPlayer, ITrick trick, IPlayer player, bool isOffenseSide, int indexOfTrick)
        {
            var partnerKnown = !PartnerUnknown(trick, indexOfTrick);
            var pickerDone = queueRankOfPlayer > trick.QueueRankOfPicker;
            var partnerDone = partnerKnown ? queueRankOfPlayer > trick.QueueRankOfPartner : (bool?)null;
            var isParnter = player.Cards.Contains(trick.PartnerCard) || trick.QueueRankOfPartner == queueRankOfPlayer;
            var offenseDone = (pickerDone ? 1 : 0) + (partnerDone == true ? 1 : 0);
            if (queueRankOfPlayer == trick.PlayerCount)
                return 100;
            if (isOffenseSide)
            {
                var opponentCount = partnerKnown || isParnter ? trick.PlayerCount - 2 : trick.PlayerCount - 1;
                return (int)Math.Round((double)((queueRankOfPlayer - 1) - offenseDone) / opponentCount * 100);
            }
            else
            {
                var opponentCount = partnerKnown ? 2 : 1;
                return (int)Math.Round((double)offenseDone / opponentCount * 100);
            }
        }

        private static bool PartnerUnknown(ITrick trick, int indexOfTrick)
        {
            if (trick.Hand.PartnerCardPlayed == null)
                return true;
            if (indexOfTrick < trick.Hand.PartnerCardPlayed[0])
                return true;
            if (indexOfTrick > trick.Hand.PartnerCardPlayed[0])
                return false;
            var partnerPosition = trick.QueueRankOfPartner;
            return !partnerPosition.HasValue || partnerPosition < trick.Hand.PartnerCardPlayed[1];
        }
    }

    public abstract class BaseMoveKeyGenerator
    {
        protected static int StrongerUnknownCards(ITrick trick, IPlayer player, ICard legalCard, int queueRankOfPlayer, List<ITrick> previousTricks)
        {
            var strongerCards = StrongerCards(legalCard);
            var knownStrongerCards = KnownStrongerCards(trick, player, legalCard, queueRankOfPlayer, previousTricks);
            var strongerUnknownCards = strongerCards - knownStrongerCards;
            return strongerUnknownCards;
        }

        private static int StrongerCards(ICard legalCard)
        {
            var curCardSuit = CardRepository.GetSuit(legalCard);
            var morePowerfulCards = 0;
            if (curCardSuit == Suit.TRUMP)
                morePowerfulCards = legalCard.Rank - 1;
            else
                morePowerfulCards = CountMorePowerOfSuit(legalCard, morePowerfulCards);
            return morePowerfulCards;
        }

        private static int CountMorePowerOfSuit(ICard legalCard, int morePowerfulCards)
        {
            const int totalTrump = 8 + 6;
            morePowerfulCards = totalTrump;
            switch (legalCard.CardType)
            {
                case CardType.ACE:
                    morePowerfulCards += 0;
                    break;
                case CardType.N10:
                    morePowerfulCards += 1;
                    break;
                case CardType.KING:
                    morePowerfulCards += 2;
                    break;
                case CardType.N9:
                    morePowerfulCards += 3;
                    break;
                case CardType.N8:
                    morePowerfulCards += 4;
                    break;
                default:
                    morePowerfulCards += 5;
                    break;
            }
            return morePowerfulCards;
        }

        private static int KnownStrongerCards(ITrick trick, IPlayer player, ICard legalCard, int queueRankOfPlayer, List<ITrick> previousTricks)
        {
            var knownCards = previousTricks.SelectMany(t => t.OrderedMoves.Select(m => m.Value)).ToList();
            knownCards.AddRange(trick.OrderedMoves.Take(queueRankOfPlayer - 1).Select(m => m.Value));
            if (trick.Hand.Picker == player)
                knownCards.AddRange(trick.Hand.Deck.Buried);
            var playerCards = player.Cards.Union(trick.Hand.Tricks.SelectMany(t => t.CardsPlayed.Where(m => m.Key == player).Select(m => m.Value)));
            knownCards = knownCards.Union(playerCards).ToList();
            var knownStrongerCards = knownCards
                                        .Where(c => CardRepository.GetSuit(c) == CardRepository.GetSuit(legalCard) || CardRepository.GetSuit(c) == Suit.TRUMP)
                                        .Count(c => c.Rank < legalCard.Rank);
            return knownStrongerCards;
        }

        protected static int StrongerHeldCards(IPlayer player, ICard legalCard, List<ITrick> previousTricks, ITrick trick)
        {
            var heldCards = player.Cards.Union(trick.Hand.Tricks.SelectMany(t => t.CardsPlayed.Where(m => m.Key == player).Select(m => m.Value))).ToList();
            foreach (var card in previousTricks.Select(t => t.CardsPlayed[player]))
                heldCards.Remove(card);
            heldCards.Remove(legalCard);
            var morePowerfulHeld = heldCards.Where(c => CardRepository.GetSuit(c) == Suit.TRUMP || CardRepository.GetSuit(c) == CardRepository.GetSuit(legalCard))
                                              .Count(c => c.Rank < legalCard.Rank);
            return morePowerfulHeld;
        }
    }
}