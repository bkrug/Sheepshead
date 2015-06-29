using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using System.Collections.Generic;
using Moq;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PickStatTests
    {
        [TestMethod]
        public void PickStat_AddStat()
        {
            var repository = new PickStatRepository();
            var key = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                TrumpStdDeviation = 3,
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            var identicalKey = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                TrumpStdDeviation = 3,
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            repository.IncrementPickResult(key, true);
            repository.IncrementPassResult(key, false);
            var result = repository.GetRecordedResults(key);
            var result2 = repository.GetRecordedResults(identicalKey);

            Assert.AreEqual(1, result.PicksWon);
            Assert.AreEqual(0, result.PassedWon);
            Assert.IsNotNull(result2, "Using a different instance of an identical key should return the same result");
            Assert.AreEqual(1, result2.PicksWon);
            Assert.AreEqual(0, result2.PassedWon);
        }

        [TestMethod]
        public void PickStat_GetResultWithNoEntry()
        {
            var repository = new PickStatRepository();
            repository.UnitTestRefresh();
            var key = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                TrumpStdDeviation = 3,
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            var differentKey = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                TrumpStdDeviation = 2,
                PointsInHand = 10,
                TotalCardsWithPoints = 5
            };
            repository.IncrementPickResult(key, true);
            repository.IncrementPassResult(key, false);
            var result = repository.GetRecordedResults(key);
            var result2 = repository.GetRecordedResults(differentKey);

            Assert.AreEqual(1, result.PicksWon);
            Assert.AreEqual(0, result.PassedWon);
            Assert.IsNotNull(result2, "Using a different key should let us know that there were no results.");
            Assert.AreEqual(0, result2.PicksWon);
            Assert.AreEqual(0, result2.PassedWon);
            Assert.AreEqual(null, result2.PickPortionWon);
            Assert.AreEqual(null, result2.PassedPortionWon);
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
        public void PickStat_Save()
        {
            var savedText = new Queue<string>();

            var writerWrapperMock = new Mock<IStreamWriterWrapper>();
            writerWrapperMock.Setup(m => m.WriteLine(It.IsAny<string>())).Callback((string text) =>
            {
                savedText.Enqueue(text);
            });

            var readerWrapperMock = new Mock<IStreamReaderWrapper>();
            readerWrapperMock.Setup(m => m.ReadLine()).Returns(() =>
            {
                return savedText.Count > 0 ? savedText.Dequeue() : null;
            });

            //Save some stats to a file
            var repository = new PickStatRepository();
            repository.UnitTestRefresh();
            var key1 = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                TrumpStdDeviation = 3,
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            var key2 = new PickStatUniqueKey()
            {
                TrumpCount = 2,
                AvgTrumpRank = 6,
                TrumpStdDeviation = 1,
                PointsInHand = 20,
                TotalCardsWithPoints = 3
            };
            repository.IncrementPickResult(key1, false);
            repository.IncrementPassResult(key1, true);
            repository.IncrementPickResult(key2, true);
            repository.IncrementPassResult(key2, false);
            repository.SaveToFile(writerWrapperMock.Object);

            Assert.AreEqual(4, savedText.Count, "Last operation should have put something in the file");

            //Pretend to restart program
            repository.UnitTestRefresh();
            Assert.AreEqual(0, repository.GetRecordedResults(key1).HandsPicked, "There should no longer be any recorded results.");

            //Recover Saved Results
            repository = PickStatRepository.FromFile(readerWrapperMock.Object);
            Assert.AreEqual(0, repository.GetRecordedResults(key1).PicksWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).HandsPicked);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).PassedWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).HandsPicked);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).PicksWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).HandsPassed);
            Assert.AreEqual(0, repository.GetRecordedResults(key2).PassedWon);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).HandsPicked);
        }
    }
}