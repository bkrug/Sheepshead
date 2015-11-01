﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models.Players
{
    public class LearningPlayer : BasicPlayer
    {
        private IMoveKeyGenerator _moveKeyGenerator;
        private IPickKeyGenerator _pickKeyGenerator;
        private IStatResultPredictor _movePredictor;
        private IPickResultPredictor _pickPredictor;
        private IBuryKeyGenerator _buryKeyGenerator;
        private IBuryResultPredictor _buryPredictor;

        private LearningPlayer() { }

        public LearningPlayer(IMoveKeyGenerator moveKeyGenerator, IStatResultPredictor movePredictor, IPickKeyGenerator pickKeyGenerator, IPickResultPredictor pickPredictor,
            IBuryKeyGenerator buryKeyGenerator, IBuryResultPredictor buryPredictor)
        {
            _moveKeyGenerator = moveKeyGenerator;
            _movePredictor = movePredictor;
            _pickKeyGenerator = pickKeyGenerator;
            _pickPredictor = pickPredictor;
            _buryKeyGenerator = buryKeyGenerator;
            _buryPredictor = buryPredictor;
        }

        public override ICard GetMove(ITrick trick)
        {
            var legalCards = this.Cards.Where(c => trick.IsLegalAddition(c, this)).ToList();
            var results = new Dictionary<ICard, MoveStat>();
            foreach(var legalCard in legalCards) 
            {
                var key = _moveKeyGenerator.GenerateKey(trick, this, legalCard);
                var result = _movePredictor.GetWeightedStat(key);
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

        protected override List<ICard> DropCardsForPickInternal(IDeck deck)
        {
            List<ICard> buriedList = null;
            BuryStat bestStat = new BuryStat() { HandsPicked = 1, TotalPoints = -4 }; //Returns AvgPoints of -4.  Lower than possible.
            var cards = deck.Blinds.Union(Cards).ToList();
            for (int i = 0; i <= cards.Count - 2; i++)
                for (int j = i + 1; j <= cards.Count - 1; j++)
                {
                    var testBuried = new List<ICard>() { cards[i], cards[j] };
                    var testHand = Cards.ToList();
                    testHand.Remove(cards[i]);
                    testHand.Remove(cards[j]);
                    var newKey = _buryKeyGenerator.GenerateKey(testHand, testBuried);
                    var newStat = _buryPredictor.GetWeightedStat(newKey);
                    if (newStat.AvgPickPoints > bestStat.AvgPickPoints)
                    {
                        buriedList = testBuried;
                        bestStat = newStat;
                    }
                }
            return buriedList;
        }
    }
}