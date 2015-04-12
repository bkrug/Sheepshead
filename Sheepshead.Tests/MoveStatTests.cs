using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using System.Collections.Generic;
using Moq;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Tests
{
    [TestClass]
    public class MoveStatTests
    {
        [TestMethod]
        public void MoveStat_AddStat()
        {
            var repository = MoveStatRepository.Instance;
            repository.UnitTestRefresh();
            var key = new MoveStatUniqueKey()
            {
                Picker = 2,
                Partner = null,
                Trick = 1,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 13,
                TotalPointsInPreviousTricks = 0,

                PointsInThisCard = 15,
                RankOfThisCard = 8,
                PartnerCard = false,
                HigherRankingCardsPlayedThisTrick = 0,
                HigherRankingCardsPlayedPreviousTricks = 12
            };
            var identicalKey = new MoveStatUniqueKey()
            {
                Picker = 2,
                Partner = null,
                Trick = 1,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 13,
                TotalPointsInPreviousTricks = 0,

                PointsInThisCard = 15,
                RankOfThisCard = 8,
                PartnerCard = false,
                HigherRankingCardsPlayedThisTrick = 0,
                HigherRankingCardsPlayedPreviousTricks = 12
            };
            repository.IncrementTrickResult(key, true);
            repository.IncrementHandResult(key, false);
            var result = repository.GetRecordedResults(key);
            var result2 = repository.GetRecordedResults(identicalKey);

            Assert.AreEqual(1, result.TrickPortionWon);
            Assert.AreEqual(0, result.HandPortionWon);
            Assert.IsNotNull(result2, "Using a different instance of an identical key should return the same result");
            Assert.AreEqual(1, result2.TrickPortionWon);
            Assert.AreEqual(0, result2.HandPortionWon);
        }

        [TestMethod]
        public void MoveStat_GetResultWithNoEntry()
        {
            var repository = MoveStatRepository.Instance;
            repository.UnitTestRefresh();
            var key = new MoveStatUniqueKey()
            {
                Picker = 2,
                Partner = null,
                Trick = 1,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 13,
                TotalPointsInPreviousTricks = 0,

                PointsInThisCard = 15,
                RankOfThisCard = 8,
                PartnerCard = false,
                HigherRankingCardsPlayedThisTrick = 0,
                HigherRankingCardsPlayedPreviousTricks = 12
            };
            var differentKey = new MoveStatUniqueKey()
            {
                Picker = 3,
                Partner = null,
                Trick = 1,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 13,
                TotalPointsInPreviousTricks = 0,

                PointsInThisCard = 15,
                RankOfThisCard = 8,
                PartnerCard = false,
                HigherRankingCardsPlayedThisTrick = 0,
                HigherRankingCardsPlayedPreviousTricks = 12
            };
            repository.IncrementTrickResult(key, true);
            repository.IncrementHandResult(key, false);
            var result = repository.GetRecordedResults(key);
            var result2 = repository.GetRecordedResults(differentKey);

            Assert.AreEqual(1, result.TrickPortionWon);
            Assert.AreEqual(0, result.HandPortionWon);
            Assert.IsNotNull(result2, "Using a different key should let us know that there were no results.");
            Assert.AreEqual(0, result2.TricksTried);
            Assert.AreEqual(0, result2.HandsTried);
            Assert.AreEqual(null, result2.TrickPortionWon);
            Assert.AreEqual(null, result2.HandPortionWon);
        }

        const double testKeyTricks = .6;
        const double testKeyHands = .4;
        const double key1Tricks = .3;
        const double key1Hands = .5;
        const double key2Tricks = .45;
        const double key2Hands = .6;
        const double key3Tricks = .41;
        const double key3Hands = .55;
        const double differentKeyTricks = .12;
        const double differetnKeyHands = .32;

        [TestMethod]
        public void MoveStat_GetWeightedResult()
        {
            var testKey = new MoveStatUniqueKey() {
                Picker = 2,
                Partner = null,
                Trick = 3,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 5,
                TotalPointsInPreviousTricks = 34,
                PointsInThisCard = 10,
                RankOfThisCard = 8,
                PartnerCard = false,
                HigherRankingCardsPlayedPreviousTricks = 5,
                HigherRankingCardsPlayedThisTrick = 0
            };
            var key1 = testKey;
            key1.Picker = 1;
            var key2 = testKey;
            key2.PointsInThisCard = 9;
            var key3 = testKey;
            key3.HigherRankingCardsPlayedPreviousTricks = 7;
            var veryDifferentKey = new MoveStatUniqueKey() {
                Picker = 4,
                Partner = null,
                Trick = 3,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 0,
                TotalPointsInPreviousTricks = 60,
                PointsInThisCard = 10,
                RankOfThisCard = 8,
                PartnerCard = false,
                HigherRankingCardsPlayedPreviousTricks = 15,
                HigherRankingCardsPlayedThisTrick = 3
            };
            var avgKeyTricks = (key1Tricks + key2Tricks + key3Tricks) / 3;
            var avgKeyHands = (key1Hands + key2Hands + key3Hands) / 3;
            {
                var gamesPlayed = 10;
                var mockRepository = GetRepositoryMock(gamesPlayed, testKey, key1, key2, key3, veryDifferentKey);
                var predictor = new ResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var expectedTricksWon = (testKeyTricks + 0.75 * avgKeyTricks) / 1.75 * gamesPlayed;
                var expectedHandsWon = (testKeyHands + 0.75 * avgKeyHands) / 1.75 * gamesPlayed;
                Assert.AreEqual(expectedTricksWon, results.TricksWon, 0.99);
                Assert.AreEqual(expectedHandsWon, results.HandsWon, 0.99);
            }
            {
                var gamesPlayed = 100;
                var mockRepository = GetRepositoryMock(gamesPlayed, testKey, key1, key2, key3, veryDifferentKey);
                var predictor = new ResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var expectedTricksWon = (testKeyTricks + 0.5 * avgKeyTricks) / 1.5 * gamesPlayed;
                var expectedHandsWon = (testKeyHands + 0.5 * avgKeyHands) / 1.5 * gamesPlayed;
                Assert.AreEqual(expectedTricksWon, results.TricksWon, 0.99);
                Assert.AreEqual(expectedHandsWon, results.HandsWon, 0.99);
            }
            {
                var gamesPlayed = 1000;
                var mockRepository = GetRepositoryMock(gamesPlayed, testKey, key1, key2, key3, veryDifferentKey);
                var predictor = new ResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var expectedTricksWon = (testKeyTricks + 0.25 * avgKeyTricks) / 1.25 * gamesPlayed;
                var expectedHandsWon = (testKeyHands + 0.25 * avgKeyHands) / 1.25 * gamesPlayed;
                Assert.AreEqual(expectedTricksWon, results.TricksWon, 0.99);
                Assert.AreEqual(expectedHandsWon, results.HandsWon, 0.99);
            }
            {
                var gamesPlayed = 10000;
                var mockRepository = GetRepositoryMock(gamesPlayed, testKey, key1, key2, key3, veryDifferentKey);
                var predictor = new ResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var expectedTricksWon = testKeyTricks * gamesPlayed;
                var expectedHandsWon = testKeyHands * gamesPlayed;
                Assert.AreEqual(expectedTricksWon, results.TricksWon, 0.99, "At 10,000 games or higher, similar results should be ignored");
                Assert.AreEqual(expectedHandsWon, results.HandsWon, 0.99, "At 10,000 games or higher, similar results should be ignored");
            }
            {
                var gamesPlayed = 100000;
                var mockRepository = GetRepositoryMock(gamesPlayed, testKey, key1, key2, key3, veryDifferentKey);
                var predictor = new ResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var expectedTricksWon = testKeyTricks * gamesPlayed;
                var expectedHandsWon = testKeyHands * gamesPlayed;
                Assert.AreEqual(expectedTricksWon, results.TricksWon, 0.99);
                Assert.AreEqual(expectedHandsWon, results.HandsWon, 0.99);
            }
        }

        private Mock<IMoveStatRepository> GetRepositoryMock(int gamesPlayed,
            MoveStatUniqueKey testKey, MoveStatUniqueKey key1, MoveStatUniqueKey key2,
            MoveStatUniqueKey key3, MoveStatUniqueKey veryDifferentKey)
        {
            var mockRepository = new Mock<IMoveStatRepository>();
            var genericStat = new MoveStat()
            {
                TricksWon = 0,
                TricksTried = 0,
                HandsWon = 0,
                HandsTried = 0
            };
            var testStat = new MoveStat()
            {
                TricksWon = (int)(testKeyTricks * gamesPlayed),
                TricksTried = gamesPlayed,
                HandsWon = (int)(testKeyHands * gamesPlayed),
                HandsTried = gamesPlayed
            };
            var key1Stat = new MoveStat()
            {
                TricksWon = (int)(key1Tricks * 10000),
                TricksTried = (10000),
                HandsWon = (int)(key1Hands * (10000)),
                HandsTried = (10000)
            };
            var key2Stat = new MoveStat()
            {
                TricksWon = (int)(key2Tricks * (10000)),
                TricksTried = (10000),
                HandsWon = (int)(key2Hands * (10000)),
                HandsTried = (10000)
            };
            var key3Stat = new MoveStat()
            {
                TricksWon = (int)(key3Tricks * (10000)),
                TricksTried = (10000),
                HandsWon = (int)(key3Hands * (10000)),
                HandsTried = (10000)
            };
            var veryDifferentStat = new MoveStat()
            {
                TricksWon = (int)(differentKeyTricks * (gamesPlayed * 1.5)),
                TricksTried = (int)(gamesPlayed * 1.5),
                HandsWon = (int)(differetnKeyHands * (gamesPlayed * 1.5)),
                HandsTried = (int)(gamesPlayed * 1.5)
            };
            mockRepository.Setup(m => m.GetRecordedResults(It.IsAny<MoveStatUniqueKey>())).Returns((MoveStatUniqueKey key) =>
            {
                if (key.Equals(testKey))
                    return testStat;
                if (key.Equals(key1))
                    return key1Stat;
                if (key.Equals(key2))
                    return key2Stat;
                if (key.Equals(key3))
                    return key3Stat;
                if (key.Equals(veryDifferentKey))
                    return veryDifferentStat;
                return genericStat;
            });
            return mockRepository;
        }

        [TestMethod]
        public void MoveState_Save()
        {
            var savedText = new Queue<string>();

            var writerWrapperMock = new Mock<IStreamWriterWrapper>();
            writerWrapperMock.Setup(m => m.WriteLine(It.IsAny<string>())).Callback((string text) =>
            {
                savedText.Enqueue(text);
            });

            var readerWrapperMock = new Mock<IStreamReaderWrapper>();
            readerWrapperMock.Setup(m => m.ReadLine()).Returns(() => {
                return savedText.Count > 0 ? savedText.Dequeue() : null;
            });

            //Save some stats to a file
            var repository = MoveStatRepository.Instance;
            repository.UnitTestRefresh();
            var key1 = new MoveStatUniqueKey { 
                Picker = 3,
                Partner = null,
                Trick = 4,
                MoveWithinTrick = 2,
                PointsAlreadyInTrick = 0,
                TotalPointsInPreviousTricks = 42,
                PointsInThisCard = 4,
                RankOfThisCard = 5,
                PartnerCard = false,
                HigherRankingCardsPlayedPreviousTricks = 23,
                HigherRankingCardsPlayedThisTrick = 10
            };
            var key2 = new MoveStatUniqueKey
            {
                Picker = 3,
                Partner = 3,
                Trick = 2,
                MoveWithinTrick = 2,
                PointsAlreadyInTrick = 0,
                TotalPointsInPreviousTricks = 42,
                PointsInThisCard = 4,
                RankOfThisCard = 5,
                PartnerCard = true,
                HigherRankingCardsPlayedPreviousTricks = 23,
                HigherRankingCardsPlayedThisTrick = 10
            };
            repository.IncrementTrickResult(key1, false);
            repository.IncrementHandResult(key1, true);
            repository.IncrementTrickResult(key2, true);
            repository.IncrementHandResult(key2, false);
            repository.SaveToFile(writerWrapperMock.Object);

            Assert.AreEqual(4, savedText.Count, "Last operation should have put something in the file");

            //Pretend to restart program
            repository.UnitTestRefresh();
            Assert.AreEqual(0, repository.GetRecordedResults(key1).HandsTried, "There should no longer be any recorded results.");

            //Recover Saved Results
            repository = MoveStatRepository.FromFile(readerWrapperMock.Object);
            Assert.AreEqual(0, repository.GetRecordedResults(key1).TricksWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).TricksTried);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).HandsWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).HandsTried);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).TricksWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).TricksTried);
            Assert.AreEqual(0, repository.GetRecordedResults(key2).HandsWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).HandsTried);
        }
    }
}
