using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models.Players
{
/*
Heuristic For Whether to play the card = 
	GamesWon% + (TricksWon% / (2 ^ (abs(GamesWon% - 50) / 25) * 2) - 25
*/

    public class LearningPlayer : BasicPlayer
    {
        private Dictionary<ITrick, MoveStatUniqueKey> _keys = new Dictionary<ITrick, MoveStatUniqueKey>();

        public override ICard GetMove(ITrick trick)
        {
            var repository = MoveStatRepository.Instance;
            var legalCards = this.Cards.Where(c => trick.IsLegalAddition(c, this)).ToList();
            var results = new Dictionary<ICard, MoveStat>();
            var playerList = trick.Hand.Deck.Game.Players;
            foreach(var legalCard in legalCards) 
            {
                var key = GenerateKey(trick, playerList, legalCard);
                var predictor = new ResultPredictor(repository);
                var result = predictor.GetWeightedStat(key);
                results.Add(legalCard, result);
            }
            var orderedResults = results
                .OrderByDescending(r => r.Value.GamePortionWon)
                .ThenByDescending(r => r.Value.TrickPortionWon);
            var selectedCard = orderedResults.First().Key;
            _keys.Add(trick, GenerateKey(trick, playerList, selectedCard));
            return selectedCard;
        }

        private MoveStatUniqueKey GenerateKey(ITrick trick, List<IPlayer> playerList, ICard legalCard)
        {
            return new MoveStatUniqueKey()
            {
                Picker = playerList.IndexOf(trick.Hand.Picker),
                Partner = trick.Hand.Partner != null ? (int?)playerList.IndexOf(trick.Hand.Partner) : null,
                Trick = trick.Hand.Tricks.Count(),
                MoveWithinTrick = QueueRankInTrick(trick),
                PointsAlreadyInTrick = trick.CardsPlayed.Sum(c => c.Value.Points),
                TotalPointsInPreviousTricks = trick.Hand.Tricks.Where(t => t != trick).Sum(t => t.CardsPlayed.Sum(c => c.Value.Points)),
                PointsInThisCard = legalCard.Points,
                RankOfThisCard = legalCard.Rank,
                PartnerCard = trick.Hand.PartnerCard == legalCard,
                HigherRankingCardsPlayedPreviousTricks = trick.Hand.Tricks.Where(t => t != trick).SelectMany(t => t.CardsPlayed.Select(kvp => kvp.Value)).Count(c => c.Rank > legalCard.Rank),
                HigherRankingCardsPlayedThisTrick = trick.CardsPlayed.Select(kvp => kvp.Value).Count(c => c.Rank > legalCard.Rank)
            };
        }

        public void OnTrickEnd(ITrick trick)
        {
            var statKey = _keys[trick];
            var repository = MoveStatRepository.Instance;
            repository.IncrementTrickResult(statKey, trick.Winner().Player == this);
        }

        public void OnHandEnd(IHand hand)
        {
            foreach (var trick in hand.Tricks)
            {
                var statKey = _keys[trick];
                var repository = MoveStatRepository.Instance;
                repository.IncrementHandResult(statKey, trick.Winner().Player == this);
            }
        }
    }
}