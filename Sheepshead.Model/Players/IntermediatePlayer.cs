using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model.Players
{
    public class IntermediatePlayer : ComputerPlayer
    {
        ILeasterStateAnalyzer _leasterStateAnalyzer;
        IGameStateAnalyzer _gameStateAnalyzer;
        IPlayCreator _playCreator;

        public IntermediatePlayer()
        {
            _leasterStateAnalyzer = new LeasterStateAnalyzer();
            _gameStateAnalyzer = new GameStateAnalyzer();
            _playCreator = new PlayCreator();
        }

        public override bool WillPick(IDeck deck)
        {
            var highPointCards = Cards.Count(c => CardUtil.GetPoints(c) >= 10);
            var avgRank = Cards.Average(c => CardUtil.GetRank(c));
            var playerQueueRankInTrick = QueueRankInDeck(deck);
            var middleQueueRankInTrick = (deck.PlayerCount + 1) / 2;

            if (deck.PlayerCount == 5)
            {
                var willPick = avgRank <= 6
                    || avgRank <= 13 && highPointCards > 2
                    || avgRank <= 13 && playerQueueRankInTrick > middleQueueRankInTrick;
                return willPick;
            }
            else
            {
                var willPick = avgRank <= 8
                    || avgRank <= 16 && highPointCards > 2
                    || avgRank <= 16 && playerQueueRankInTrick > middleQueueRankInTrick;
                return willPick;
            }
        }

        protected override List<SheepCard> DropCardsForPickInternal(IDeck deck)
        {
            return new BuriedCardSelector(Cards).CardsToBury;
        }

        public override SheepCard? ChooseCalledAce(IDeck deck)
        {
            var acceptableSuits = LegalCalledAceSuits(deck);
            if (!acceptableSuits.LegalSuits.Any())
                return null;
            var selectedSuit = acceptableSuits.LegalSuits
                .OrderBy(g => g.Count())
                .First()
                .Key;
            return GetCardOfSuit(acceptableSuits.CardType, selectedSuit);
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
                return Cards
                    .Where(c => trick.IsLegalAddition(c, this))
                    .OrderBy(c => CardUtil.GetSuit(c) == Suit.TRUMP ? 1 : 2)
                    .FirstOrDefault();
            }
            else
            {
                if (trick.Hand.Deck.Game.PartnerMethod == PartnerMethod.CalledAce && trick.Hand.PartnerCard.HasValue)
                {
                    var partnerCardSuit = CardUtil.GetSuit(trick.Hand.PartnerCard.Value);
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) == partnerCardSuit ? 1 : 2)
                        .FirstOrDefault();
                }
                else
                {
                    return Cards
                        .OrderBy(c => CardUtil.GetSuit(c) != Suit.TRUMP ? 1 : 2)
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
                    if (_gameStateAnalyzer.UnplayedCardsBeatPlayedCards(this, trick))
                    {
                        if (_gameStateAnalyzer.UnplayedCardsBeatMyCards(this, trick))
                            return _playCreator.GiveAwayLeastPower(this, trick);
                        else
                            return _playCreator.PlayStrongestWin(this, trick);
                    }
                    else
                    {
                        return _playCreator.GiveAwayPoints(this, trick);
                    }
                }
                else
                {
                    if (_gameStateAnalyzer.ICanWinTrick(this, trick))
                    {
                        if (_gameStateAnalyzer.UnplayedCardsBeatMyCards(this, trick))
                            return _playCreator.GiveAwayLeastPower(this, trick);
                        else
                            return _playCreator.PlayStrongestWin(this, trick);
                    }
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
                        if (_leasterStateAnalyzer.HaveIAlreadyWon(this, trick))
                            return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                        else
                            return _playCreator.PlayStrongestLoosingCard(this, trick);
                    }
                    else
                    {
                        if (_leasterStateAnalyzer.HaveIAlreadyWon(this, trick))
                        {
                            if (_leasterStateAnalyzer.HaveAnyPowerCards(this, trick))
                            {
                                if (_leasterStateAnalyzer.HaveHighPointsBeenPlayed(trick))
                                    return _playCreator.PlayStrongestLoosingCard(this, trick);
                                else
                                    return _playCreator.PlayStrongestWin(this, trick);
                            }
                            else
                            {
                                return _playCreator.PlayStrongestLoosingCard(this, trick);
                            }
                        }
                        else
                        {
                            if (_leasterStateAnalyzer.HaveAnyPowerCards(this, trick))
                            {
                                if (_leasterStateAnalyzer.HaveHighPointsBeenPlayed(trick))
                                {
                                    return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                                }
                                else
                                {
                                    if (_leasterStateAnalyzer.HaveTwoPowerCards(this, trick))
                                        return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                                    else
                                        return _playCreator.PlayStrongestWin(this, trick);
                                }
                            }
                            else
                            {
                                return _playCreator.PlayStrongestWin(this, trick);
                            }
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
                    if (_leasterStateAnalyzer.HaveAnyPowerCards(this, trick))
                        return _playCreator.PlayStrongestLoosingCard(this, trick);
                    else
                        return _playCreator.GiveAwayPoints(this, trick);
                }
                else
                {
                    if (_leasterStateAnalyzer.HaveTwoPowerCards(this, trick))
                        return _playCreator.PlaySecondStrongestLoosingCard(this, trick);
                    else
                        return _playCreator.GiveAwayPoints(this, trick);
                }
            }
        }
    }
}