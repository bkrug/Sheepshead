﻿using System;
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
            var generator = new KeyGenerator();
            using (var reader = File.OpenText(SAVE_LOCATION))
            {
                var i = 0;
                while (!reader.EndOfStream)
                {
                    var summary = reader.ReadLine();
                    var hand = SummaryReader.FromSummary(summary);
                    var indexOfTrick = 0;
                    var handWinner = hand.Scores().Where(s => s.Value > 0).Select(s => s.Key).ToList();
                    var pointsInPreviousTricks = 0;
                    var previousTricks = new List<ITrick>();
                    var beforePartnerCardPlayed = true;
                    foreach (var trick in hand.Tricks)
                    {
                        var trickWinner = trick.Winner().Player;
                        var indexOfPlayer = 0;
                        var queueRankOfPicker = trick.QueueRankOfPicker;
                        var previousCards = new List<ICard>();
                        var pointsInTrick = 0;
                        int? queueRankOfPartner = null;
                        foreach (var move in trick.OrderedMoves)
                        {
                            var key = generator.GenerateKey(
                                            indexOfTrick, indexOfPlayer, queueRankOfPicker, pointsInPreviousTricks,
                                            trick, move.Value, previousTricks, previousCards,
                                            ref beforePartnerCardPlayed, ref queueRankOfPartner, ref pointsInTrick);
                            RecordMove(moves, handWinner, trickWinner, move.Key, key);
                            ++indexOfTrick;
                            ++indexOfPlayer;
                            previousCards.Add(move.Value);
                        }
                        pointsInPreviousTricks += pointsInTrick;
                        previousTricks.Add(trick);
                    }
                }
            }
            return moves;
        }

        private static void RecordMove(Dictionary<MoveStatUniqueKey, MoveStat> moves, List<IPlayer> handWinner, IPlayer trickWinner, IPlayer player, MoveStatUniqueKey key)
        {
            if (!moves.ContainsKey(key))
                moves.Add(key, new MoveStat()
                {
                    TricksTried = 1,
                    TricksWon = trickWinner == player ? 1 : 0,
                    HandsTried = 1,
                    HandsWon = handWinner.Contains(player) ? 1 : 0
                });
            else
            {
                moves[key].TricksTried += 1;
                moves[key].TricksWon += trickWinner == player ? 1 : 0;
                moves[key].HandsTried += 1;
                moves[key].HandsWon += handWinner.Contains(player) ? 1 : 0;
            }
        }
    }
}