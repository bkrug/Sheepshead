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
        private IPickStatRepository _pickStatRepository;
        private IMoveStatRepository _moveStatRepository;
        private IBuryStatRepository _buryStatRepository;
        private ILeasterStatRepository _leasterStatRepository;

        private LearningHelper()
        {
        }

        public LearningHelper(IHand hand, string saveLocation, IPickStatRepository pickStatRepository, IMoveStatRepository moveStatRepository, IBuryStatRepository buryStatRepository, 
            ILeasterStatRepository leasterStatRepository)
        {
            _saveLocation = saveLocation;
            hand.OnHandEnd += WriteHandSummary;
            hand.OnHandEnd += UpdateMoveStats;
            hand.OnHandEnd += UpdatePickStats;
            hand.OnHandEnd += UpdateBuryStats;
            _pickStatRepository = pickStatRepository;
            _moveStatRepository = moveStatRepository;
            _buryStatRepository = buryStatRepository;
            _leasterStatRepository = leasterStatRepository;
        }

        public LearningHelper(IHand hand, string saveLocaiton) 
            : this(hand, saveLocaiton, RepositoryRepository.Instance.PickStatRepository, RepositoryRepository.Instance.MoveStatRepository, RepositoryRepository.Instance.BuryStatRepository, 
                        RepositoryRepository.Instance.LeasterStatRepository)
        {
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
            var leasterGenerator = new LeasterKeyGenerator();
            var handWinners = hand.Scores().Where(s => s.Value > 0).Select(s => s.Key).ToList();
            foreach (var trick in hand.Tricks)
            {
                var trickWinner = trick.Winner();
                var offenseWon = hand.Picker == trickWinner || hand.Partner == trickWinner;
                foreach (var move in trick.OrderedMoves.Where(m => m.Key is LearningPlayer || m.Key is RecordingPlayer))
                {
                    var player = move.Key;
                    var card = move.Value;
                    if (!hand.Leasters)
                    {
                        var key = generator.GenerateKey(trick, player, card);
                        var playerIsOffense = hand.Picker == player || hand.Partner == player;
                        _moveStatRepository.IncrementTrickResult(key, offenseWon == playerIsOffense);
                        _moveStatRepository.IncrementHandResult(key, handWinners.Contains(player));
                    }
                    else
                    {
                        var key = leasterGenerator.GenerateKey(trick, player, card);
                        _leasterStatRepository.IncrementHandResult(key, trickWinner == player);
                    }
                }
            }
        }

        private void UpdatePickStats(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            var generator = new PickKeyGenerator();
            var scores = hand.Scores();
            foreach (var player in hand.Players.Where(m => m is LearningPlayer || m is RecordingPlayer))
            {
                var key = generator.GenerateKey(hand, player);
                if (hand.Picker == player)
                    _pickStatRepository.IncrementPickResult(key, scores[player]);
                else
                    _pickStatRepository.IncrementPassResult(key, scores[player]);
            }
        }

        private void UpdateBuryStats(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            var generator = new BuryKeyGenerator();
            var scores = hand.Scores();
            if(hand.Picker is LearningPlayer || hand.Picker is RecordingPlayer)
            {
                var key = generator.GenerateKey(hand.Deck);
                _buryStatRepository.IncrementResult(key, scores[hand.Picker]);
            }
        }
    }
}