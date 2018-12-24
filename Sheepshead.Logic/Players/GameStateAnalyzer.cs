using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;

namespace Sheepshead.Model.Players
{
    public interface IGameStateAnalyzer
    {
        bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick);
        bool MySideWinning(IPlayer thisPlayer, ITrick trick);
        bool ICanWinTrick(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsBeatPlayedCards(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsBeatMyCards(IPlayer thisPlayer, ITrick trick);
    }

    public class GameStateAnalyzer : IGameStateAnalyzer
    {
        public bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick)
        {
            if (trick.CardsPlayed.Count == trick.IHand.IGame.PlayerCount - 1)
                return true;
            var playerIsPartner = PlayerKnowsSelfToBePartner(thisPlayer, trick);
            var playerIsOffense = trick.IHand.Picker == thisPlayer || playerIsPartner;
            if (playerIsOffense)
            {
                var opponentCount = trick.IHand.PartnerCardEnum.HasValue
                    ? trick.IHand.IGame.PlayerCount - 2
                    : trick.IHand.IGame.PlayerCount - 1;
                var opponentsWithTurns = playerIsPartner
                    ? trick.CardsPlayed.Keys.Count(p => trick.IHand.Picker != p && thisPlayer != p)
                    : trick.CardsPlayed.Keys.Count(p => trick.IHand.Picker != p && trick.IHand.PresumedParnter != p);
                if (opponentsWithTurns < opponentCount)
                    return false;
                if (!playerIsPartner && trick.IHand.PartnerCardEnum.HasValue && trick.IHand.PresumedParnter == null)
                    return null;
                return true;
            }
            else
            {
                if (!trick.CardsPlayed.ContainsKey(trick.IHand.Picker))
                    return false;
                if (trick.IHand.PresumedParnter != null && !trick.CardsPlayed.ContainsKey(trick.IHand.PresumedParnter))
                    return false;
                if (trick.IHand.PartnerCardEnum.HasValue && trick.IHand.PresumedParnter == null)
                    return null;
                return true;
            }
        }

        public bool MySideWinning(IPlayer thisPlayer, ITrick trick)
        {
            var winningPlay = GameStateUtils.GetWinningPlay(trick);
            if (trick.IHand.Picker == thisPlayer)
                return winningPlay.Key == trick.IHand.PresumedParnter;
            if (PlayerKnowsSelfToBePartner(thisPlayer, trick))
                return winningPlay.Key == trick.IHand.Picker;
            return winningPlay.Key != trick.IHand.Picker 
                && winningPlay.Key != trick.IHand.PresumedParnter
                && !(trick.IHand.PartnerCardEnum.HasValue && trick.IHand.PresumedParnter == null);
        }

        private static bool PlayerKnowsSelfToBePartner(IPlayer thisPlayer, ITrick trick)
        {
            return trick.IHand.Partner == thisPlayer
                || trick.IHand.PartnerCardEnum.HasValue && thisPlayer.Cards.Contains(trick.IHand.PartnerCardEnum.Value);
        }

        public bool ICanWinTrick(IPlayer thisPlayer, ITrick trick)
        {
            var playableCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer));
            return GameStateUtils.GetCardsThatCouldWin(trick, playableCards).Any();
        }

        public bool UnplayedCardsBeatPlayedCards(IPlayer thisPlayer, ITrick trick)
        {
            var unrevealedCards = GetUnrevealedCards(thisPlayer, trick);
            return GameStateUtils.GetCardsThatCouldWin(trick, unrevealedCards).Any();
        }

        private static IEnumerable<SheepCard> GetUnrevealedCards(IPlayer thisPlayer, ITrick trick)
        {
            var revealedAndPlayersOwnCards = trick.IHand.ITricks.SelectMany(t => t.CardsPlayed.Values).Union(thisPlayer.Cards);
            var allCards = Enum.GetValues(typeof(SheepCard)).OfType<SheepCard>();
            var unrevealedCards = allCards.Except(revealedAndPlayersOwnCards);
            return unrevealedCards;
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