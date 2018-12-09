using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model.Players
{
    public class AdvancedPlayer : IntermediatePlayer
    {
        ILeasterStateAnalyzer _leasterStateAnalyzer;
        IGameStateAnalyzer _gameStateAnalyzer;
        IPlayCreator _playCreator;

        public AdvancedPlayer()
        {
            _leasterStateAnalyzer = new LeasterStateAnalyzer();
            _gameStateAnalyzer = new GameStateAnalyzer();
            _playCreator = new PlayCreator();
        }

        public override bool WillPick(IHand deck)
        {
            var playerQueueRankInTrick = QueueRankInDeck(deck);
            var middleQueueRankInTrick = (deck.PlayerCount + 1) / 2;
            var trumpCount = this.Cards.Count(c => CardUtil.GetSuit(c) == Suit.TRUMP);
            var willPick = playerQueueRankInTrick > middleQueueRankInTrick && trumpCount >= 3
                || playerQueueRankInTrick == middleQueueRankInTrick && trumpCount >= 4
                || trumpCount >= 5;
            return willPick;
        }

        protected override List<SheepCard> DropCardsForPickInternal(IHand deck)
        {
            return Cards
                .OrderBy(c => CardUtil.GetSuit(c) != Suit.TRUMP ? 1 : 2)
                .ThenByDescending(c => CardUtil.GetPoints(c))
                .Take(2)
                .ToList();
        }

        public override bool GoItAlone(IHand deck)
        {
            var trumpCount = Cards.Count(c => CardUtil.GetSuit(c) == Suit.TRUMP);
            var queenJackCount = Cards.Count(c => new[] { CardType.JACK, CardType.QUEEN }.Contains(CardUtil.GetFace(c)));
            var queenCount = Cards.Count(c => CardUtil.GetFace(c) == CardType.QUEEN);
            return trumpCount >= 5 && queenJackCount >= 3 && queenCount >= 2;
        }

        public override SheepCard GetMove(ITrick trick)
        {
            if (Cards.Count == 1)
                return Cards.Single();
            if (trick.Hand.Leasters)
                return PlayLeasterMove(trick);
            if (trick.StartingPlayer == this)
                return GetLeadMove(trick);
            else
                return GetLaterMove(trick);
        }

        private SheepCard GetLeadMove(ITrick trick)
        {
            if (trick.Hand.Picker == this || IamPartner(trick))
            {
                if (Cards.Average(c => CardUtil.GetRank(c)) > 10)
                    return Cards
                        .Where(c => trick.IsLegalAddition(c, this))
                        .OrderBy(c => CardUtil.GetSuit(c) == Suit.TRUMP ? 1 : 2)
                        .ThenByDescending(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
                else
                    return Cards
                        .Where(c => trick.IsLegalAddition(c, this))
                        .OrderBy(c => CardUtil.GetSuit(c) == Suit.TRUMP ? 1 : 2)
                        .ThenBy(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
            }
            else
            {
                if (trick.Hand.Game.PartnerMethod == PartnerMethod.CalledAce && trick.Hand.PartnerCard.HasValue)
                {
                    var partnerCardSuit = CardUtil.GetSuit(trick.Hand.PartnerCard.Value);
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) == partnerCardSuit ? 1 : 2)
                        .ThenBy(c => CardUtil.GetSuit(c) != Suit.TRUMP ? 1 : 2)
                        .ThenByDescending(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
                }
                else
                {
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) != Suit.TRUMP ? 1 : 2)
                        .ThenByDescending(c => CardUtil.GetRank(c))
                        .FirstOrDefault();
                }
            }
        }

        private SheepCard GetLaterMove(ITrick trick)
        {
            if (_gameStateAnalyzer.AllOpponentsHavePlayed(this, trick) == true)
            {
                if (_gameStateAnalyzer.MySideWinning(this, trick))
                {
                    return _playCreator.GiveAwayPoints(this, trick);
                }
                else
                {
                    if (_gameStateAnalyzer.ICanWinTrick(this, trick))
                        return _playCreator.PlayWeakestWin(this, trick);
                    else
                        return _playCreator.GiveAwayLeastPower(this, trick);
                }
            }
            else
            {
                if (_gameStateAnalyzer.MySideWinning(this, trick))
                {
                    return _playCreator.GiveAwayPoints(this, trick);
                }
                else
                {
                    if (_gameStateAnalyzer.ICanWinTrick(this, trick))
                        return _playCreator.PlayStrongestWin(this, trick);
                    else
                        return _playCreator.GiveAwayLeastPower(this, trick);
                }
            }
        }

        private SheepCard PlayLeasterMove(ITrick trick)
        {
            if (_leasterStateAnalyzer.CanIWin(this, trick))
            {
                if (_leasterStateAnalyzer.CanILoose(this, trick))
                {
                    if (_leasterStateAnalyzer.EarlyInTrick(trick))
                    {
                        return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                    }
                    else
                    {
                        if (_leasterStateAnalyzer.HaveIAlreadyWon(this, trick))
                        {
                            return _playCreator.PlayStrongestLoosingCard(this, trick);
                        }
                        else
                        {
                            if (_leasterStateAnalyzer.HaveHighPointsBeenPlayed(trick))
                                return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                            else
                                return _playCreator.PlayStrongestWin(this, trick);
                        }
                    }
                }
                else
                {
                    return _playCreator.PlayStrongestWin(this, trick);
                }
            }
            else
            {
                if (_leasterStateAnalyzer.HaveIAlreadyWon(this, trick))
                {
                    return _playCreator.PlayStrongestLoosingCard(this, trick);
                }
                else
                {
                    return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                }
            }
        }
    }
}