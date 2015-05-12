using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models.Players
{
    public class LearningPlayer : BasicPlayer
    {
        private Dictionary<ITrick, MoveStatUniqueKey> _keys = new Dictionary<ITrick, MoveStatUniqueKey>();
        private IKeyGenerator _generator;
        private ICentroidResultPredictor _predictor;

        private LearningPlayer() { }

        public LearningPlayer(IKeyGenerator generator, ICentroidResultPredictor predictor)
        {
            _generator = generator;
            _predictor = predictor;
        }

        public override ICard GetMove(ITrick trick)
        {
            var legalCards = this.Cards.Where(c => trick.IsLegalAddition(c, this)).ToList();
            var results = new Dictionary<ICard, MoveStat>();
            foreach(var legalCard in legalCards) 
            {
                var key = _generator.GenerateKey(trick, this, legalCard);
                var result = _predictor.GetPrediction(key);
                results.Add(legalCard, result);
            }
            var handOrderedResults = results
                .OrderByDescending(r => r.Value.HandPortionWon);
            var firstResult = handOrderedResults.First();
            var closeResults = results
                .Where(r => Math.Abs((double)(r.Value.HandPortionWon - firstResult.Value.HandPortionWon)) < 0.1);
            var trickOrderedResults = closeResults
                .OrderByDescending(r => r.Value.TrickPortionWon);
            var selectedCard = trickOrderedResults.First().Key;
            return selectedCard;
        }
    }
}