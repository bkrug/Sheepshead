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

        private LearningPlayer() { }

        public LearningPlayer(IKeyGenerator generator)
        {
            _generator = generator;
        }

        public override ICard GetMove(ITrick trick)
        {
            var predictor = SummaryLoader.Instance.ResultPredictor;
            var legalCards = this.Cards.Where(c => trick.IsLegalAddition(c, this)).ToList();
            var results = new Dictionary<ICard, MoveStat>();
            foreach(var legalCard in legalCards) 
            {
                var key = _generator.GenerateKey(trick, this, legalCard);
                var result = predictor.GetPrediction(key);
                results.Add(legalCard, result);
            }
            var orderedResults = results
                .OrderByDescending(r => r.Value.HandPortionWon)
                .ThenByDescending(r => r.Value.TrickPortionWon);
            var selectedCard = orderedResults.First().Key;
            return selectedCard;
        }
    }
}