using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models.Players
{
    public class LearningPlayer : BasicPlayer
    {
        private IKeyGenerator _moveKeyGenerator;
        private IPickKeyGenerator _pickKeyGenerator;
        private IStatResultPredictor _predictor;
        private IPickResultPredictor _pickPredictor;

        private LearningPlayer() { }

        public LearningPlayer(IKeyGenerator moveKeyGenerator, IStatResultPredictor predictor, IPickKeyGenerator pickKeyGenerator, IPickResultPredictor pickPredictor)
        {
            _moveKeyGenerator = moveKeyGenerator;
            _predictor = predictor;
            _pickPredictor = pickPredictor;
            _pickKeyGenerator = pickKeyGenerator;
        }

        public override ICard GetMove(ITrick trick)
        {
            var legalCards = this.Cards.Where(c => trick.IsLegalAddition(c, this)).ToList();
            var results = new Dictionary<ICard, MoveStat>();
            foreach(var legalCard in legalCards) 
            {
                var key = _moveKeyGenerator.GenerateKey(trick, this, legalCard);
                var result = _predictor.GetWeightedStat(key);
                if (result != null && result.HandsTried > 0 && result.TricksTried > 0)
                    results.Add(legalCard, result);
            }
            ICard selectedCard;
            if (results.Any())
            {
                var handOrderedResults = results
                    .OrderByDescending(r => r.Value.HandPortionWon);
                var firstResult = handOrderedResults.First();
                var closeResults = results
                    .Where(r => Math.Abs((double)(r.Value.HandPortionWon - firstResult.Value.HandPortionWon)) < 0.1);
                var trickOrderedResults = closeResults
                    .OrderByDescending(r => r.Value.TrickPortionWon);
                selectedCard = trickOrderedResults.First().Key;
                OnMoveHandler(trick, selectedCard);
            }
            else
                selectedCard = base.GetMove(trick);
            return selectedCard;
        }

        public override bool WillPick(IDeck deck)
        {
            var pickKey = _pickKeyGenerator.GenerateKey(deck.Hand, this);
            var historicScores = _pickPredictor.GetWeightedStat(pickKey);
            return historicScores.AvgPickPoints > historicScores.AvgPassedPoints;
        }
    }
}