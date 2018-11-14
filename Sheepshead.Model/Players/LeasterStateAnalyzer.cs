﻿using Sheepshead.Models;
using Sheepshead.Models.Players;
using System.Linq;

namespace Sheepshead.Models.Players
{
    public interface ILeasterStateAnalyzer
    {
        bool CanIWin(IPlayer thisPlayer, ITrick trick);
        bool CanILoose(IPlayer thisPlayer, ITrick trick);
        bool EarlyInTrick(ITrick trick);
        bool HaveIAlreadyWon(IPlayer thisPlayer, ITrick trick);
        bool HaveAnyPowerCards(IPlayer thisPlayer, ITrick trick);
        bool HaveTwoPowerCards(IPlayer thisPlayer, ITrick trick);
        bool HaveHighPointsBeenPlayed(ITrick trick);
    }

    public class LeasterStateAnalyzer : ILeasterStateAnalyzer
    {
        public bool CanIWin(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer)).ToList();
            return GameStateUtils.GetCardsThatCouldWin(trick, legalCards).Any();
        }

        public bool CanILoose(IPlayer thisPlayer, ITrick trick)
        {
            var legalCards = thisPlayer.Cards.Where(c => trick.IsLegalAddition(c, thisPlayer)).ToList();
            var winnableCards = GameStateUtils.GetCardsThatCouldWin(trick, legalCards).ToList();
            var looseableCards = legalCards.Except(winnableCards);
            return looseableCards.Any();
        }

        public bool EarlyInTrick(ITrick trick)
        {
            if (trick.Hand.Deck.Game.PlayerCount == 3)
                return trick.CardsPlayed.Count < 2;
            else
                return trick.CardsPlayed.Count < 3;
        }

        public bool HaveIAlreadyWon(IPlayer thisPlayer, ITrick trick)
        {
            return trick.Hand.Tricks
                .Where(t => t.CardsPlayed.Count == trick.Hand.Deck.Game.PlayerCount)
                .Any(t => t.Winner().Player == thisPlayer);
        }

        public bool HaveAnyPowerCards(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool HaveTwoPowerCards(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool HaveHighPointsBeenPlayed(ITrick trick)
        {
            return trick.CardsPlayed.Sum(cp => CardUtil.GetPoints(cp.Value)) >= 10;
        }
    }
}
