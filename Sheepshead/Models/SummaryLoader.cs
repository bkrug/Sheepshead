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
    public class SaveLocations
    {
        public static string FIRST_SAVE { get { return @"c:\temp\HandSummaries.txt"; } }
    }

    public class SummaryLoaderSingleton
    {
        private static SummaryLoader _instance = new SummaryLoader(SaveLocations.FIRST_SAVE, 200);
        public static SummaryLoader Instance { get { return _instance; } }
    }

    public interface ISummaryLoader
    {
        CentroidResultPredictor ResultPredictor { get; }
    }

    public class SummaryLoader : ISummaryLoader
    {
        private SummaryLoader()
        {
        }
        public SummaryLoader(string filePath, int numOfGames)
        {
            Load(filePath, 0, numOfGames);
        }
        public SummaryLoader(string filePath, int skip, int numOfGames)
        {
            ResultPredictor = Load(filePath, 0, numOfGames);
        }

        public CentroidResultPredictor ResultPredictor { get; private set; }

        private CentroidResultPredictor Load(string filePath, int skip, int numOfGames)
        {
            var movesAndStats = ReadMoves(filePath, skip, numOfGames);
            var clusterer = new Clusterer(new RandomWrapper());
            var resultsInRooms = clusterer.Cluster(movesAndStats.Select(d => d.Key).ToList());
            var centroidsInRooms = ClusterUtils.GetClusterDictionary(movesAndStats, resultsInRooms);
            var resultPredictor = new CentroidResultPredictor(centroidsInRooms);
            return resultPredictor;
        }

        public static Dictionary<MoveStatUniqueKey, MoveStat> ReadMoves(string filePath, int skip, int numOfGames)
        {
            var moves = new Dictionary<MoveStatUniqueKey, MoveStat>();
            if (!File.Exists(filePath))
                return moves;
            var generator = new KeyGenerator();
            using (var reader = File.OpenText(filePath))
            {
                var i = 0;
                while (!reader.EndOfStream && i++ < skip + numOfGames)
                {
                    var summary = reader.ReadLine();
                    if (i < skip)
                        continue;
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
                        var highestRankInTrick = 32;
                        int? queueRankOfPartner = null;
                        var indexOfWinningPlayer = -1;
                        List<List<ICard>> allHeldCards = hand.Players.Select(p => p.Cards).ToList();
                        var m = 0;
                        foreach (var move in trick.OrderedMoves)
                        {
                            var heldCards = allHeldCards[m++];
                            var key = generator.GenerateKey(
                                            indexOfTrick, indexOfPlayer, queueRankOfPicker, pointsInPreviousTricks,
                                            trick, move.Value, previousTricks, previousCards, heldCards,
                                            ref beforePartnerCardPlayed, ref queueRankOfPartner, ref pointsInTrick,
                                            ref highestRankInTrick, ref indexOfWinningPlayer);
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