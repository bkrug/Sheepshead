using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Models.Players
{
    public interface IGameStateAnalyzer
    {
        bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick);
        bool MySideWinning(IPlayer thisPlayer, ITrick trick);
        List<SheepCard> MyCardsThatCanWin(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsBeatPlayedCards(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsBeatMyCards(IPlayer thisPlayer, ITrick trick);
    }

    public class GameStateAnalyzer : IGameStateAnalyzer
    {
        public bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick)
        {
            if (trick.CardsPlayed.Count == trick.Hand.Deck.Game.PlayerCount - 1)
                return true;
            var playerIsPartner = PlayerKnowsSelfToBePartner(thisPlayer, trick);
            var playerIsOffense = trick.Hand.Picker == thisPlayer || playerIsPartner;
            if (playerIsOffense)
            {
                var opponentCount = trick.Hand.PartnerCard.HasValue
                    ? trick.Hand.Deck.Game.PlayerCount - 2
                    : trick.Hand.Deck.Game.PlayerCount - 1;
                var opponentsWithTurns = playerIsPartner
                    ? trick.CardsPlayed.Keys.Count(p => trick.Hand.Picker != p && thisPlayer != p)
                    : trick.CardsPlayed.Keys.Count(p => trick.Hand.Picker != p && trick.Hand.PresumedParnter != p);
                if (opponentsWithTurns < opponentCount)
                    return false;
                if (!playerIsPartner && trick.Hand.PartnerCard.HasValue && trick.Hand.PresumedParnter == null)
                    return null;
                return true;
            }
            else
            {
                if (!trick.CardsPlayed.ContainsKey(trick.Hand.Picker))
                    return false;
                if (trick.Hand.PresumedParnter != null && !trick.CardsPlayed.ContainsKey(trick.Hand.PresumedParnter))
                    return false;
                if (trick.Hand.PartnerCard.HasValue && trick.Hand.PresumedParnter == null)
                    return null;
                return true;
            }
        }

        public bool MySideWinning(IPlayer thisPlayer, ITrick trick)
        {
            var winningPlay = GetWinningPlay(trick);
            if (trick.Hand.Picker == thisPlayer)
                return winningPlay.Key == trick.Hand.PresumedParnter;
            if (PlayerKnowsSelfToBePartner(thisPlayer, trick))
                return winningPlay.Key == trick.Hand.Picker;
            return winningPlay.Key != trick.Hand.Picker 
                && winningPlay.Key != trick.Hand.PresumedParnter
                && !(trick.Hand.PartnerCard.HasValue && trick.Hand.PresumedParnter == null);
        }

        private static bool PlayerKnowsSelfToBePartner(IPlayer thisPlayer, ITrick trick)
        {
            return trick.Hand.Partner == thisPlayer
                || trick.Hand.PartnerCard.HasValue && thisPlayer.Cards.Contains(trick.Hand.PartnerCard.Value);
        }

        public List<SheepCard> MyCardsThatCanWin(IPlayer thisPlayer, ITrick trick)
        {
            return GetCardsThatCouldWin(trick, thisPlayer.Cards)
                .Where(c => trick.IsLegalAddition(c, thisPlayer))
                .ToList();
        }

        public bool UnplayedCardsBeatPlayedCards(IPlayer thisPlayer, ITrick trick)
        {
            var unrevealedCards = GetUnrevealedCards(thisPlayer, trick);
            return GetCardsThatCouldWin(trick, unrevealedCards).Any();
        }

        private static IEnumerable<SheepCard> GetUnrevealedCards(IPlayer thisPlayer, ITrick trick)
        {
            var revealedAndPlayersOwnCards = trick.Hand.Tricks.SelectMany(t => t.CardsPlayed.Values).Union(thisPlayer.Cards);
            var allCards = Enum.GetValues(typeof(SheepCard)).OfType<SheepCard>();
            var unrevealedCards = allCards.Except(revealedAndPlayersOwnCards);
            return unrevealedCards;
        }

        private static IEnumerable<SheepCard> GetCardsThatCouldWin(ITrick trick, IEnumerable<SheepCard> comparisonCards)
        {
            var startSuit = CardUtil.GetSuit(trick.CardsPlayed.First().Value);
            var winningCard = GetWinningPlay(trick).Value;
            var winningCardRank = CardUtil.GetRank(winningCard);
            if (CardUtil.GetSuit(winningCard) == Suit.TRUMP)
                return comparisonCards.Where(c => CardUtil.GetRank(c) < winningCardRank);
            else
                return comparisonCards.Where(c =>
                    CardUtil.GetSuit(c) == Suit.TRUMP
                    || CardUtil.GetSuit(c) == startSuit && CardUtil.GetRank(c) < winningCardRank
                );
        }

        private static KeyValuePair<IPlayer, SheepCard> GetWinningPlay(ITrick trick)
        {
            var startSuit = CardUtil.GetSuit(trick.CardsPlayed.First().Value);
            var winningPlay = trick.CardsPlayed
                .OrderBy(cp => CardUtil.GetSuit(cp.Value) == Suit.TRUMP ? 1 : 2)
                .ThenBy(cp => CardUtil.GetSuit(cp.Value) == startSuit ? 1 : 2)
                .ThenBy(cp => CardUtil.GetRank(cp.Value))
                .First();
            return winningPlay;
        }

        public bool UnplayedCardsBeatMyCards(IPlayer thisPlayer, ITrick trick)
        {
            var playableCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer));
            var unrevealedCards = GetUnrevealedCards(thisPlayer, trick);
            var startSuit = CardUtil.GetSuit(trick.CardsPlayed.First().Value);
            var strongestUnrevealedCard = GetStrongestCard(unrevealedCards, startSuit);
            var strongestOfMyCards = GetStrongestCard(playableCards, startSuit);
            var strongestCard = GetStrongestCard(new List<SheepCard>() { strongestUnrevealedCard, strongestOfMyCards }, startSuit);
            return strongestCard == strongestUnrevealedCard;
        }

        private static SheepCard GetStrongestCard(IEnumerable<SheepCard> unrevealedCards, Suit startSuit)
        {
            return unrevealedCards
                .OrderBy(c => CardUtil.GetSuit(c) == Suit.TRUMP ? 1 : 2)
                .ThenBy(c => CardUtil.GetSuit(c) == startSuit ? 1 : 2)
                .ThenBy(c => CardUtil.GetRank(c))
                .First();
        }
    }
}