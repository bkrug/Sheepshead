using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using System.IO;

namespace Sheepshead.Models.Players.Stats
{
    public class LearningHelper
    {
        private string _saveLocation;

        private LearningHelper()
        {
        }

        public LearningHelper(IHand hand, string saveLocation)
        {
            _saveLocation = saveLocation;
            hand.OnHandEnd += WriteHandSummary;
            hand.OnHandEnd += UpdateMoveStats;
        }

        private static object lockObject = new object();

        private void WriteHandSummary(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            lock (lockObject)
            {
                using (var sw = File.AppendText(_saveLocation))
                {
                    sw.WriteLine(hand.Summary());
                    sw.Flush();
                }
            }
        }

        private void UpdateMoveStats(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            var generator = new MoveKeyGenerator();
            var handWinners = hand.Scores().Where(s => s.Value > 0).Select(s => s.Key).ToList();
            foreach (var trick in hand.Tricks)
            {
                var trickWinner = trick.Winner();
                var offenseWon = hand.Picker == trickWinner || hand.Partner == trickWinner;
                foreach (var move in trick.OrderedMoves.Where(m => m.Key is LearningPlayer))
                {
                    var player = move.Key;
                    var card = move.Value;
                    var key = generator.GenerateKey(trick, player, card);
                    var playerIsOffense = hand.Picker == player || hand.Partner == player;
                    RepositoryRepository.Instance.MoveStatRepository.IncrementTrickResult(key, offenseWon == playerIsOffense);
                    RepositoryRepository.Instance.MoveStatRepository.IncrementHandResult(key, handWinners.Contains(player));
                }
            }
        }
    }
}