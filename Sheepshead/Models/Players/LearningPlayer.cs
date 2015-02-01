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
                var key = trick.GenerateKey(this, legalCard);
                var predictor = new ResultPredictor(repository);
                var result = predictor.GetWeightedStat(key);
                results.Add(legalCard, result);
            }
            var orderedResults = results
                .OrderByDescending(r => r.Value.GamePortionWon)
                .ThenByDescending(r => r.Value.TrickPortionWon);
            var selectedCard = orderedResults.First().Key;
            return selectedCard;
        }
    }
}