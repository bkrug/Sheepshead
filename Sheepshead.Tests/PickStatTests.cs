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

        [TestMethod]
        public void PickStatGuesser_GetRecordedResult()
        {
            var guesser = new PickStatGuesser();
            var getStat = guesser.MakeGuess(new PickStatUniqueKey()
            {
                TrumpCount = 0,
                AvgTrumpRank = 14,
                PointsInHand = 0,
                TotalCardsWithPoints = 0
            });
            Assert.IsTrue(getStat.AvgPassedPoints < 0, "If no actual data is available, the computer should guess that there is no chance of winning with this stat.");
            Assert.IsTrue(getStat.AvgPassedPoints > getStat.AvgPickPoints, "If no actual data is available, the computer should guess that picking is worse than passing.");

            var getStat1 = guesser.MakeGuess(new PickStatUniqueKey()
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
        public void PickStatResultPredictor_GetWeightedStat_WithoutGuess()
        {
            var key1 = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                PointsInHand = 3,
                TotalCardsWithPoints = 1
            };
            var key2 = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                PointsInHand = 4,
                TotalCardsWithPoints = 1
            };
            var stat1 = new PickStat()
            {
                HandsPicked = 5000,
                HandsPassed = 5000,
                TotalPickPoints = 1000,
                TotalPassedPoints = 500
            };
            var stat2 = new PickStat()
            {
                HandsPicked = 500,
                HandsPassed = 500,
                TotalPickPoints = 182,
                TotalPassedPoints = 234
            }; 
            var guessRepositoryMock = new Mock<IPickStatGuesser>();
            //To simplify the test, the guess repository will always return a result that doesn't affect other results.
            guessRepositoryMock.Setup(m => m.MakeGuess(It.IsAny<PickStatUniqueKey>())).Returns(new PickStat());
            var repositoryMock = new Mock<IPickStatRepository>();
            var predictor = new PickStatResultPredictor(repositoryMock.Object, guessRepositoryMock.Object);

            repositoryMock.Setup(m => m.GetRecordedResults(It.IsAny<PickStatUniqueKey>())).Returns(new PickStat());
            repositoryMock.Setup(m => m.GetRecordedResults(key1)).Returns(stat1);
            repositoryMock.Setup(m => m.GetRecordedResults(key2)).Returns(stat2);
            var gotStat = predictor.GetWeightedStat(key1);
            Assert.AreEqual(stat1.HandsPicked, gotStat.HandsPicked, 
                "Since there is so much data for key1 and stat1, we don't need to look at close by statistics to make a prediction.");
            Assert.AreEqual(stat1.HandsPassed, gotStat.HandsPassed);
            Assert.AreEqual(stat1.TotalPickPoints, gotStat.TotalPickPoints);
            Assert.AreEqual(stat1.TotalPassedPoints, gotStat.TotalPassedPoints);

            stat1 = new PickStat()
            {
                HandsPicked = 100,
                HandsPassed = 100,
                TotalPickPoints = 32,
                TotalPassedPoints = 54
            };
            repositoryMock.Setup(m => m.GetRecordedResults(key1)).Returns(stat1);
            repositoryMock.Setup(m => m.GetRecordedResults(key2)).Returns(stat2);
            gotStat = predictor.GetWeightedStat(key1);
            Assert.AreEqual(stat1.HandsPicked + stat2.HandsPicked, gotStat.HandsPicked,
                "Due to limited data, we now expect the results to include data from more than one data point.");
            Assert.AreEqual(stat1.HandsPassed + stat2.HandsPassed, gotStat.HandsPassed);
            Assert.AreEqual(stat1.TotalPickPoints + stat2.TotalPickPoints, gotStat.TotalPickPoints);
            Assert.AreEqual(stat1.TotalPassedPoints + stat2.TotalPassedPoints, gotStat.TotalPassedPoints);
        }

        [TestMethod]
        public void PickStatResultPredictor_GetWeightedStat_WithGuess()
        {
            var key1 = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                PointsInHand = 3,
                TotalCardsWithPoints = 1
            };
            var key2 = new PickStatUniqueKey()
            {
                TrumpCount = 3,
                AvgTrumpRank = 5,
                PointsInHand = 4,
                TotalCardsWithPoints = 1
            };
            var stat1 = new PickStat()
            {
                HandsPicked = 100,
                HandsPassed = 100,
                TotalPickPoints = 32,
                TotalPassedPoints = 54
            };
            var stat2 = new PickStat()
            {
                HandsPicked = 500,
                HandsPassed = 500,
                TotalPickPoints = 182,
                TotalPassedPoints = 234
            }; 
            var guessStat1 = new PickStat() {
                HandsPicked = 5,
                HandsPassed = 5,
                TotalPickPoints = 3,
                TotalPassedPoints = 2
            };
            var guessStat2 = new PickStat()
            {
                HandsPicked = 57892,
                HandsPassed = 29875,
                TotalPickPoints = -2975893,
                TotalPassedPoints = 2987109,
            };
            var guessRepositoryMock = new Mock<IPickStatGuesser>();
            guessRepositoryMock.Setup(m => m.MakeGuess(It.IsAny<PickStatUniqueKey>())).Returns(new PickStat());
            guessRepositoryMock.Setup(m => m.MakeGuess(key1)).Returns(guessStat1);
            guessRepositoryMock.Setup(m => m.MakeGuess(key2)).Returns(guessStat2);
            var repositoryMock = new Mock<IPickStatRepository>();
            repositoryMock.Setup(m => m.GetRecordedResults(It.IsAny<PickStatUniqueKey>())).Returns(new PickStat());
            repositoryMock.Setup(m => m.GetRecordedResults(key1)).Returns(stat1);
            repositoryMock.Setup(m => m.GetRecordedResults(key2)).Returns(stat2);
            var predictor = new PickStatResultPredictor(repositoryMock.Object, guessRepositoryMock.Object);

            var gotStat = predictor.GetWeightedStat(key1);
            Assert.AreEqual(stat1.HandsPicked + stat2.HandsPicked + guessStat1.HandsPicked, gotStat.HandsPicked,
                "There is limited data, so the result should be a combination of two real stats and also of one guess.");
            Assert.AreEqual(stat1.HandsPassed + stat2.HandsPassed + guessStat1.HandsPassed, gotStat.HandsPassed,
                "Notice that we do not want guessStat2 to be included in the results.  The guesses should not be very influential after enough data has been collected.");
            Assert.AreEqual(stat1.TotalPickPoints + stat2.TotalPickPoints + guessStat1.TotalPickPoints, gotStat.TotalPickPoints);
            Assert.AreEqual(stat1.TotalPassedPoints + stat2.TotalPassedPoints + guessStat1.TotalPassedPoints, gotStat.TotalPassedPoints);
        }
    }
}