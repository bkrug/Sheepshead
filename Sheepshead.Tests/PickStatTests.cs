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
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            var identicalKey = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            repository.IncrementPickResult(key, 1);
            repository.IncrementPassResult(key, -2);
            var result = repository.GetRecordedResults(key);
            var result2 = repository.GetRecordedResults(identicalKey);

            Assert.AreEqual(1, result.TotalPickPoints);
            Assert.AreEqual(-2, result.TotalPassedPoints);
            Assert.IsNotNull(result2, "Using a different instance of an identical key should return the same result");
            Assert.AreEqual(1, result2.TotalPickPoints);
            Assert.AreEqual(-2, result2.TotalPassedPoints);
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
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            var differentKey = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                PointsInHand = 10,
                TotalCardsWithPoints = 5
            };
            repository.IncrementPickResult(key, 2);
            repository.IncrementPassResult(key, -1);
            var result = repository.GetRecordedResults(key);
            var result2 = repository.GetRecordedResults(differentKey);

            Assert.AreEqual(2, result.TotalPickPoints);
            Assert.AreEqual(-1, result.TotalPassedPoints);
            Assert.IsNotNull(result2, "Using a different key should let us know that there were no results.");
            Assert.AreEqual(0, result2.TotalPickPoints);
            Assert.AreEqual(0, result2.TotalPassedPoints);
            Assert.AreEqual(null, result2.AvgPickPoints);
            Assert.AreEqual(null, result2.AvgPassedPoints);
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
                PointsInHand = 15,
                TotalCardsWithPoints = 5
            };
            var key2 = new PickStatUniqueKey()
            {
                TrumpCount = 2,
                AvgTrumpRank = 6,
                PointsInHand = 20,
                TotalCardsWithPoints = 3
            };
            repository.IncrementPickResult(key1, -2);
            repository.IncrementPassResult(key1, 0);
            repository.IncrementPickResult(key2, 1);
            repository.IncrementPassResult(key2, -1);
            repository.SaveToFile(writerWrapperMock.Object);

            Assert.AreEqual(4, savedText.Count, "Last operation should have put something in the file");

            //Pretend to restart program
            repository.UnitTestRefresh();
            Assert.AreEqual(0, repository.GetRecordedResults(key1).HandsPicked, "There should no longer be any recorded results.");

            //Recover Saved Results
            repository = PickStatRepository.FromFile(readerWrapperMock.Object);
            Assert.AreEqual(-2, repository.GetRecordedResults(key1).TotalPickPoints);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).HandsPicked);
            Assert.AreEqual(0, repository.GetRecordedResults(key1).TotalPassedPoints);
            Assert.AreEqual(1, repository.GetRecordedResults(key1).HandsPicked);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).TotalPickPoints);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).HandsPassed);
            Assert.AreEqual(-1, repository.GetRecordedResults(key2).TotalPassedPoints);
            Assert.AreEqual(1, repository.GetRecordedResults(key2).HandsPicked);
        }

        //[TestMethod]
        public void PickStatResultPredictor_BeforeDataAdded()
        {
            var repositoryMock = new Mock<PickStatRepository>();
            var predictor = new PickStatResultPredictor(repositoryMock.Object);
            var getStat = predictor.GetWeightedStat(new PickStatUniqueKey()
            {
                TrumpCount = 0,
                AvgTrumpRank = 0,
                PointsInHand = 0,
                TotalCardsWithPoints = 0
            });
            Assert.IsTrue(getStat.AvgPassedPoints < 0, "If no actual data is available, the computer should guess that there is no chance of winning with this stat.");
            Assert.IsTrue(getStat.AvgPassedPoints > getStat.AvgPickPoints, "If no actual data is available, the computer should guess that picking is worse than passing.");

            var getStat1 = predictor.GetWeightedStat(new PickStatUniqueKey()
            {
                TrumpCount = 4,
                AvgTrumpRank = 3,
                PointsInHand = 11 + 11 + 3 + 3 + 3 + 3,
                TotalCardsWithPoints = 6
            });
            Assert.IsTrue(getStat1.AvgPickPoints > 0, "If no actual data is available, the computer should guess that there is a high chance of winning with this stat.");
            Assert.IsTrue(getStat1.AvgPassedPoints < getStat1.AvgPickPoints, "The system should guess that with this hand, passing gives few points on average than picking.");
        }

        [TestMethod]
        public void PickStatResultPredictor_GetWeightedStat()
        {
            var repositoryMock = new Mock<IPickStatRepository>();
            var predictor = new PickStatResultPredictor(repositoryMock.Object);
            var getStat = predictor.GetWeightedStat(new PickStatUniqueKey()
            {
                TrumpCount = 0,
                AvgTrumpRank = 0,
                PointsInHand = 0,
                TotalCardsWithPoints = 0
            });

        }
    }
}