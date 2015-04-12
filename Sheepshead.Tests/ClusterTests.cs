using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests
{
    [TestClass]
    public class ClusterTests
    {
        [TestMethod]
        public void GetClusterDictionary()
        {
            var clusterResult = GetClusterResult();
            var expectedDict = new Dictionary<MoveStatCentroid, MoveStat>() {
                { centroid1, new MoveStat() { TricksTried = 4, TricksWon = 4, HandsTried = 4, HandsWon = 0 } },
                { centroid2, new MoveStat() { TricksTried = 3, TricksWon = 0, HandsTried = 3, HandsWon = 3 } },
                { centroid3, new MoveStat() { TricksTried = 3, TricksWon = 2, HandsTried = 3, HandsWon = 2 } },
            };
            var repositoryMock = GetStatRepository(clusterResult, 4);
            var actualDict = ClusterUtils.GetClusterDictionary(repositoryMock, clusterResult);
            Assert.AreEqual(expectedDict.Count, actualDict.Count, "Dictionaries must be the same size.");
            var i = 0;
            foreach (var key in expectedDict.Keys)
            {
                Assert.AreEqual(expectedDict[key].TrickPortionWon, actualDict[key].TrickPortionWon, "Trick portion won must match during iteration " + i + ".");
                Assert.AreEqual(expectedDict[key].HandPortionWon, actualDict[key].HandPortionWon, "Hand portion won must match during iteration " + i + ".");
                ++i;
            }
        }

        private ClusterResult GetClusterResult()
        {
            return new ClusterResult()
            {
                Data = new List<MoveStatUniqueKey>()
                {
                    GetNearbyKey(centroid2, 2.6),
                    GetNearbyKey(centroid3, 1.6),
                    GetNearbyKey(centroid1, -0.6),
                    GetNearbyKey(centroid1, -1.2),
                    GetNearbyKey(centroid3, 0.75),
                    GetNearbyKey(centroid2, 0.3),
                    GetNearbyKey(centroid3, -1.4),
                    GetNearbyKey(centroid1, -0.8),
                    GetNearbyKey(centroid2, 1.7),
                    GetNearbyKey(centroid1, -1.6),
                },
                ClusterIndecies = new int[] { 1, 2, 0, 0, 2, 1, 2, 0, 1, 0 },
                Centroids = new List<MoveStatCentroid>() { centroid1, centroid2, centroid3 }
            };
        }

        private IMoveStatRepository GetStatRepository(ClusterResult clusterResult, int indexOfSecondKeyInLastCentroid)
        {
            var mockResults = new Dictionary<MoveStatUniqueKey, MoveStat>();
            for (var i = 0; i < clusterResult.Data.Count(); ++i)
            {
                var cluster = clusterResult.ClusterIndecies[i];
                mockResults.Add(clusterResult.Data[i], new MoveStat()
                {
                    TricksTried = 1,
                    TricksWon = cluster == 0 ? 1 : cluster == 1 ? 0 : i <= indexOfSecondKeyInLastCentroid ? 1 : 0,
                    HandsTried = 1,
                    HandsWon = cluster == 0 ? 0 : cluster == 1 ? 1 : i <= indexOfSecondKeyInLastCentroid ? 1 : 0
                });
            }
            var repositoryMock = new Mock<IMoveStatRepository>();
            repositoryMock
                .Setup(m => m.GetRecordedResults(It.IsAny<MoveStatUniqueKey>()))
                .Returns((MoveStatUniqueKey key) => { return mockResults[key]; });
            return repositoryMock.Object;
        }

        [TestMethod]
        public void CentroidResultPrediction()
        {
            var dict = new Dictionary<MoveStatCentroid, MoveStat>() {
                { centroid1, stat1 },
                { centroid2, stat2 },
                { centroid3, stat3 },
            };
            var predictor = new CentroidResultPredictor(dict);

            Assert.AreSame(stat1, predictor.GetPrediction(GetNearbyKey(centroid1, 2.6)));
            Assert.AreSame(stat2, predictor.GetPrediction(GetNearbyKey(centroid2, -0.8)));
            Assert.AreSame(stat3, predictor.GetPrediction(GetNearbyKey(centroid3, 0.92)));
            Assert.AreSame(stat1, predictor.GetPrediction(GetNearbyKey(centroid1, 0.4)));
            Assert.AreSame(stat2, predictor.GetPrediction(GetNearbyKey(centroid2, 1.9)));
            Assert.AreSame(stat3, predictor.GetPrediction(GetNearbyKey(centroid3, -1.5)));
        }

        private MoveStatUniqueKey GetNearbyKey(MoveStatCentroid centroid, double offset)
        {
            return new MoveStatUniqueKey()
            {
                Picker = Round(centroid.Picker + offset * .95),
                Partner = null,
                Trick = Round(centroid.Trick + offset * -0.5),
                MoveWithinTrick = Round(centroid.MoveWithinTrick + offset * -1.5),
                PointsAlreadyInTrick = Round(centroid.PointsAlreadyInTrick + offset * 1.2),
                TotalPointsInPreviousTricks = Round(centroid.TotalPointsInPreviousTricks + offset * 2.7),

                PointsInThisCard = Round(centroid.PointsInThisCard + offset * -1.1),
                RankOfThisCard = Round(centroid.RankOfThisCard + offset * -0.8),
                PartnerCard = Round(centroid.PartnerCard + offset * 0.04) == 1,
                HigherRankingCardsPlayedPreviousTricks = Round(centroid.HigherRankingCardsPlayedPreviousTricks + offset * -2.1),
                HigherRankingCardsPlayedThisTrick = Round(centroid.HigherRankingCardsPlayedThisTrick + offset * 1.5)
            };
        }

        private int Round(double d)
        {
            return (int)Math.Round(d, 0);
        }

        private MoveStatCentroid centroid1 = new MoveStatCentroid()
        {
            Picker = 3.2,
            Partner = null,
            Trick = 4.3,
            MoveWithinTrick = 0.4,
            PointsAlreadyInTrick = 1.4,
            TotalPointsInPreviousTricks = 181.9,

            PointsInThisCard = 12.3,
            RankOfThisCard = 4.1,
            PartnerCard = 0.2,
            HigherRankingCardsPlayedPreviousTricks = 25.1,
            HigherRankingCardsPlayedThisTrick = 0.3
        };
        private MoveStatCentroid centroid2 = new MoveStatCentroid()
        {
            Picker = 1.2,
            Partner = null,
            Trick = 2.1,
            MoveWithinTrick = 4.8,
            PointsAlreadyInTrick = 11.2,
            TotalPointsInPreviousTricks = 34.6,

            PointsInThisCard = 4.1,
            RankOfThisCard = 7.8,
            PartnerCard = 0.6,
            HigherRankingCardsPlayedPreviousTricks = 3.1,
            HigherRankingCardsPlayedThisTrick = 2.1
        };
        private MoveStatCentroid centroid3 = new MoveStatCentroid()
        {
            Picker = 1.9,
            Partner = null,
            Trick = 0.4,
            MoveWithinTrick = 3.8,
            PointsAlreadyInTrick = 12.1,
            TotalPointsInPreviousTricks = 0.4,

            PointsInThisCard = 4.4,
            RankOfThisCard = 9.1,
            PartnerCard = 0.44,
            HigherRankingCardsPlayedPreviousTricks = 0.2,
            HigherRankingCardsPlayedThisTrick = 2.1
        };
        private MoveStat stat1 = new MoveStat()
        {
            TricksWon = 4,
            TricksTried = 12,
            HandsWon = 9,
            HandsTried = 12
        };
        private MoveStat stat2 = new MoveStat()
        {
            TricksWon = 11,
            TricksTried = 12,
            HandsWon = 3,
            HandsTried = 12
        };
        private MoveStat stat3 = new MoveStat()
        {
            TricksWon = 7,
            TricksTried = 18,
            HandsWon = 9,
            HandsTried = 18
        };
    }
}
