using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using System.IO;

namespace Sheepshead.Models
{
    public class LearningHelper
    {
        private IMoveStatRepository _repository;
        private Dictionary<Tuple<ITrick, IPlayer>, MoveStatUniqueKey> _learningKeys = new Dictionary<Tuple<ITrick, IPlayer>, MoveStatUniqueKey>();

        private LearningHelper()
        {
        }

        public LearningHelper(IMoveStatRepository repository, IHand hand)
        {
            _repository = repository;
            hand.OnHandEnd += WriteHandSummary;
        }

        public static MoveStatUniqueKey GenerateKey(ITrick trick, IPlayer player, ICard legalCard)
        {
            var indexOfTrick = trick.Hand.Tricks.IndexOf(trick);
            List<IPlayer> playerList = trick.Hand.Players;
            var previousTricks = trick.Hand.Tricks.Take(indexOfTrick).ToList();
            var beforePartnerCardPlayed = BeforePartnerCardPlayed(trick, indexOfTrick);
            return new MoveStatUniqueKey()
            {
                Picker = trick.QueueRankOfPicker,
                Partner = beforePartnerCardPlayed ? null : trick.QueueRankOfPartner,
                Trick = trick.IndexInHand,
                MoveWithinTrick = player.QueueRankInTrick(trick),
                PointsAlreadyInTrick = trick.CardsPlayed.Sum(c => c.Value.Points),
                TotalPointsInPreviousTricks = previousTricks.Sum(t => t.CardsPlayed.Sum(c => c.Value.Points)),
                PointsInThisCard = legalCard.Points,
                RankOfThisCard = legalCard.Rank,
                PartnerCard = trick.Hand.PartnerCard == legalCard,
                HigherRankingCardsPlayedPreviousTricks =
                    previousTricks
                    .SelectMany(t => t.CardsPlayed.Select(kvp => kvp.Value))
                    .Count(c => c.Rank < legalCard.Rank),
                HigherRankingCardsPlayedThisTrick =
                    trick.CardsPlayed
                    .Select(kvp => kvp.Value)
                    .Count(c => c.Rank < legalCard.Rank)
            };
        }

        private static bool BeforePartnerCardPlayed(ITrick trick, int indexOfTrick)
        {
            if (indexOfTrick < trick.Hand.PartnerCardPlayed[0])
                return true;
            if (indexOfTrick > trick.Hand.PartnerCardPlayed[0])
                return false;
            var partnerPosition = trick.QueueRankOfPartner;
            return !partnerPosition.HasValue || partnerPosition < trick.Hand.PartnerCardPlayed[1];
        }

        private void OnAddTrickToHand(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            hand.Tricks.Last().OnTrickEnd += OnTrickEnd;
        }

        private void OnHandEnd(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            foreach (var trick in hand.Tricks)
            {
                foreach (var player in trick.Players)
                {
                    var tuple = new Tuple<ITrick, IPlayer>(trick, player);
                    var statKey = _learningKeys[tuple];
                    _repository.IncrementHandResult(statKey, trick.Hand.Scores()[player] > 0);
                }
            }
        }

        private void OnTrickMove(object sender, Trick.MoveEventArgs e)
        {
            var trick = (ITrick)sender;
            var tuple = new Tuple<ITrick, IPlayer>(trick, e.Player);
            _learningKeys.Add(tuple, LearningHelper.GenerateKey(trick, e.Player, e.Card));
        }

        private void OnTrickEnd(object sender, EventArgs e)
        {
            var trick = (ITrick)sender;
            foreach (var player in trick.Players)
            {
                var tuple = new Tuple<ITrick, IPlayer>(trick, player);
                var statKey = _learningKeys[tuple];
                _repository.IncrementTrickResult(statKey, trick.Winner().Player == this);
            }
        }

        private static object lockObject = new object();

        private void WriteHandSummary(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            lock (lockObject)
            {
                using (var sw = File.AppendText(SummaryLoader.SAVE_LOCATION))
                {
                    sw.WriteLine(hand.Summary());
                    sw.Flush();
                }
            }
        }
    }
}