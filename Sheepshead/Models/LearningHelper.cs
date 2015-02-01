using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public interface ILearningHelper
    {
        MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard);
        void EndTrick(ITrick trick);
        void OnHandEnd(ITrick trick);
    }

    public class LearningHelper : ILearningHelper
    {
        private IMoveStatRepository _repository;

        private LearningHelper()
        {
        }

        public LearningHelper(IMoveStatRepository repository)
        {
            _repository = repository;
        }

        public MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard)
        {
            List<IPlayer> playerList = trick.Hand.Players;
            return new MoveStatUniqueKey()
            {
                Picker = trick.QueueRankOfPicker,
                Partner = trick.QueueRankOfPartner,
                Trick = trick.IndexInHand,
                MoveWithinTrick = player.QueueRankInTrick(trick),
                PointsAlreadyInTrick = trick.CardsPlayed.Sum(c => c.Value.Points),
                TotalPointsInPreviousTricks = trick.Hand.Tricks.Where(t => t != trick).Sum(t => t.CardsPlayed.Sum(c => c.Value.Points)),
                PointsInThisCard = legalCard.Points,
                RankOfThisCard = legalCard.Rank,
                PartnerCard = trick.Hand.PartnerCard == legalCard,
                HigherRankingCardsPlayedPreviousTricks =
                    trick.Hand.Tricks
                    .Where(t => t != this)
                    .SelectMany(t => t.CardsPlayed.Select(kvp => kvp.Value))
                    .Count(c => c.Rank < legalCard.Rank),
                HigherRankingCardsPlayedThisTrick =
                    trick.CardsPlayed
                    .Select(kvp => kvp.Value)
                    .Count(c => c.Rank < legalCard.Rank)
            };
        }

        public void EndTrick(ITrick trick)
        {
            foreach (var player in trick.Hand.Players)
            {
                var statKey = trick.LearningKeys[player];
                _repository.IncrementTrickResult(statKey, trick.Winner().Player == this);
            }
            if (trick.Hand.IsComplete())
                trick.Hand.EndHand();
        }

        public void OnHandEnd(ITrick trick)
        {
            foreach (var player in trick.Hand.Deck.Game.Players)
            {
                var statKey = trick.LearningKeys[player];
                _repository.IncrementHandResult(statKey, trick.Hand.Scores()[player] > 0);
            }
        }
    }
}