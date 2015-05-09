using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Models
{

    public interface ISummaryLoader
    {
        CentroidResultPredictor ResultPredictor { get; }
    }

    public class SummaryLoader : ISummaryLoader
    {
        private static SummaryLoader _instance = new SummaryLoader();
        public static string SAVE_LOCATION { get { return @"c:\temp\HandSummaries.txt"; } }

        private SummaryLoader()
        {
            ResultPredictor = Load();
        }

        public static SummaryLoader Instance { get { return _instance; } }

        public CentroidResultPredictor ResultPredictor { get; private set; }

        private CentroidResultPredictor Load()
        {
            var keys = ReadMoves();
            var clusterer = new Clusterer((int)Math.Round(Math.Sqrt(keys.Count())), new RandomWrapper());
            var clusterResult = clusterer.Cluster(keys.Select(d => d.Key).ToList());
            var clusterDictionary = ClusterUtils.GetClusterDictionary(keys, clusterResult);
            var resultPredictor = new CentroidResultPredictor(clusterDictionary);
            return resultPredictor;
        }

        private static Dictionary<MoveStatUniqueKey, MoveStat> ReadMoves()
        {
            var moves = new Dictionary<MoveStatUniqueKey, MoveStat>();
            if (!File.Exists(SAVE_LOCATION))
                return moves;
            using (var reader = File.OpenText(SAVE_LOCATION))
            {
                while (!reader.EndOfStream)
                {
                    var summary = reader.ReadLine();
                    var hand = SummaryReader.FromSummary(summary);
                    var handWinner = hand.Scores().Where(s => s.Value > 0).Select(s => s.Key).ToList();
                    foreach (var trick in hand.Tricks)
                    {
                        var trickWinner = trick.Winner().Player;
                        foreach (var move in trick.OrderedMoves)
                        {
                            var key = LearningHelper.GenerateKey(trick, move.Key, move.Value);
                            if (!moves.ContainsKey(key))
                                moves.Add(key, new MoveStat()
                                {
                                    TricksTried = 1,
                                    TricksWon = trickWinner == move.Key ? 1 : 0,
                                    HandsTried = 1,
                                    HandsWon = handWinner.Contains(move.Key) ? 1 : 0
                                });
                            else
                            {
                                moves[key].TricksTried += 1;
                                moves[key].TricksWon += trickWinner == move.Key ? 1 : 0;
                                moves[key].HandsTried += 1;
                                moves[key].HandsWon += handWinner.Contains(move.Key) ? 1 : 0;
                            }
                        }
                    }
                }
            }
            return moves;
        }
    }
}