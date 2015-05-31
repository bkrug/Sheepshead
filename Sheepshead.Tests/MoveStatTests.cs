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
    public class MoveStatTests
    {
        [TestMethod]
        public void MoveStat_AddStat()
        {
            var repository = MoveStatRepository.Instance;
            repository.UnitTestRefresh();
            var key = new MoveStatUniqueKey()
            {
                CardWillOverpower = false,
                OpponentPercentDone = 50,
                CardPoints = 4,
                UnknownStrongerCards = 8,
                HeldStrongerCards = 1
            };
            var identicalKey = new MoveStatUniqueKey()
            {
                CardWillOverpower = false,
                OpponentPercentDone = 50,
                CardPoints = 4,
                UnknownStrongerCards = 8,
                HeldStrongerCards = 1
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
                CardWillOverpower = false,
                OpponentPercentDone = 50,
                CardPoints = 4,
                UnknownStrongerCards = 8,
                HeldStrongerCards = 1
            };
            var differentKey = new MoveStatUniqueKey()
            {
                CardWillOverpower = false,
                OpponentPercentDone = 51,
                CardPoints = 4,
                UnknownStrongerCards = 8,
                HeldStrongerCards = 1
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
            var testKey = new MoveStatUniqueKey()
            {
                CardWillOverpower = false,
                OpponentPercentDone = 50,
                CardPoints = 10,
                UnknownStrongerCards = 8,
                HeldStrongerCards = 1
            };

            var rnd = new Random(32918);
            {
                MoveStat testResult = new MoveStat()
                {
                    TricksTried = 1000,
                    TricksWon = 134,
                    HandsTried = 1000,
                    HandsWon = 134
                };
                var statCount = 100;
                var dict = GetDictionary(rnd, statCount, testKey, testResult);
                var mockRepository = GetRepositoryMock1(dict);
                var predictor = new StatResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                Assert.AreEqual(testResult.TrickPortionWon.Value, results.TrickPortionWon.Value, 0.01);
                Assert.AreEqual(testResult.HandPortionWon.Value, results.HandPortionWon.Value, 0.01);
            }
            {
                MoveStat testResult = new MoveStat()
                {
                    TricksTried = 1,
                    TricksWon = 1,
                    HandsTried = 1,
                    HandsWon = 1
                };
                var statCount = 100;
                var dict = GetDictionary(rnd, statCount, testKey, testResult);
                var mockRepository = GetRepositoryMock1(dict);
                var predictor = new StatResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var items = dict.Where(kvp =>
                    kvp.Key.CardWillOverpower == false &&
                    kvp.Key.OpponentPercentDone >= 25 && kvp.Key.OpponentPercentDone <= 75 &&
                    kvp.Key.CardPoints >= 4 &&
                    kvp.Key.UnknownStrongerCards >= 3 && kvp.Key.UnknownStrongerCards <= 13 &&
                    kvp.Key.HeldStrongerCards <= 2
                    );
                var expectedTricksWon = (double)items.Sum(i => i.Value.TricksWon) / items.Sum(i => i.Value.TricksTried);
                var expectedHandsWon = (double)items.Sum(i => i.Value.HandsWon) / items.Sum(i => i.Value.HandsTried);
                Assert.AreEqual(expectedTricksWon, results.TrickPortionWon.Value, 0.01);
                Assert.AreEqual(expectedHandsWon, results.HandPortionWon.Value, 0.01);
            }
            {
                var dict = new Dictionary<MoveStatUniqueKey, MoveStat>() 
                {
                    { 
                      testKey,
                      new MoveStat() { TricksWon = 345, TricksTried = 500, HandsWon = 281, HandsTried = 500 }
                    },
                    { 
                      new MoveStatUniqueKey() { CardWillOverpower = false, OpponentPercentDone = 50, CardPoints = 11, UnknownStrongerCards = 8, HeldStrongerCards = 1 },
                      new MoveStat() { TricksWon = 35, TricksTried = 250, HandsWon = 111, HandsTried = 250 }
                    },
                    { 
                      new MoveStatUniqueKey() { CardWillOverpower = false, OpponentPercentDone = 50, CardPoints = 11, UnknownStrongerCards = 6, HeldStrongerCards = 1 },
                      new MoveStat() { TricksWon = 37, TricksTried = 250, HandsWon = 97, HandsTried = 250 }
                    },
                    { 
                      new MoveStatUniqueKey() { CardWillOverpower = false, OpponentPercentDone = 25, CardPoints = 10, UnknownStrongerCards = 8, HeldStrongerCards = 1 },
                      new MoveStat() { TricksWon = 35, TricksTried = 2000, HandsWon = 111, HandsTried = 2000 }
                    },
                    { 
                      new MoveStatUniqueKey() { CardWillOverpower = true, OpponentPercentDone = 0, CardPoints = 1, UnknownStrongerCards = 1, HeldStrongerCards = 5 },
                      new MoveStat() { TricksWon = 35, TricksTried = 3000, HandsWon = 111, HandsTried = 3000 }
                    },
                };
                var mockRepository = GetRepositoryMock1(dict);
                var predictor = new StatResultPredictor(mockRepository.Object);
                var results = predictor.GetWeightedStat(testKey);
                var items = dict.Take(3).ToList();
                var expectedTricksWon = (double)items.Sum(i => i.Value.TricksWon) / items.Sum(i => i.Value.TricksTried);
                var expectedHandsWon = (double)items.Sum(i => i.Value.HandsWon) / items.Sum(i => i.Value.HandsTried);
                Assert.AreEqual(expectedTricksWon, results.TrickPortionWon.Value, 0.01);
                Assert.AreEqual(expectedHandsWon, results.HandPortionWon.Value, 0.01);
            }
        }

        private Dictionary<MoveStatUniqueKey, MoveStat> GetDictionary(Random rnd, int numberOfStats, MoveStatUniqueKey testKey, MoveStat testStat)
        {
            var dict = new Dictionary<MoveStatUniqueKey, MoveStat>();
            dict.Add(testKey, testStat);
            for (var i = 0; i < numberOfStats; ++i)
            {
                var key = new MoveStatUniqueKey()
                {
                    CardPoints = GetCardPoints(rnd),
                    CardWillOverpower = rnd.Next(4) == 0,
                    HeldStrongerCards = rnd.Next(4),
                    OpponentPercentDone = GetOpponentPercentDone(rnd),
                    UnknownStrongerCards = rnd.Next(17),
                };
                if (!dict.ContainsKey(key))
                    dict.Add(key, new MoveStat());
                dict[key].HandsWon += rnd.Next(2);
                dict[key].HandsTried += 1;
                dict[key].TricksWon += rnd.Next(2);
                dict[key].TricksTried += 1;
            }
            return dict;
        }

        private int GetCardPoints(Random rnd)
        {
            var randomValue = rnd.Next(-8, 8);
            var sign = randomValue < 0 ? -1 : 1;
            switch (Math.Abs(randomValue))
            {
                case 7:
                    return 11 * sign;
                case 6:
                    return 10 * sign;
                case 5:
                    return 4 * sign;
                case 4:
                    return 3 * sign;
                case 3:
                    return 2 * sign;
                default:
                    return 0;
            };
        }

        private static int GetOpponentPercentDone(Random rnd)
        {
            var playertype = rnd.Next(5);
            var playerIsPartner = playertype == 1;
            var offenseSide = playertype < 2;
            var partnerKnown = playerIsPartner || rnd.Next(2) == 0;
            int numberOfOpponents = GetNumberOfOpponents(offenseSide, partnerKnown);
            var opponentPercentDone = (int)Math.Round(((double)rnd.Next(numberOfOpponents) / numberOfOpponents) * 100);
            return opponentPercentDone;
        }

        private static int GetNumberOfOpponents(bool offenseSide, bool partnerKnown)
        {
            int numberOfOpponents;
            if (offenseSide && partnerKnown)
                numberOfOpponents = 3;
            else if (offenseSide && !partnerKnown)
                numberOfOpponents = 4;
            else if (!offenseSide && partnerKnown)
                numberOfOpponents = 2;
            else // if (!offenseSide && !partnerKnown)
                numberOfOpponents = 1;
            return numberOfOpponents;
        }

        private Mock<IMoveStatRepository> GetRepositoryMock1(Dictionary<MoveStatUniqueKey, MoveStat> dict)
        {
            var mockRepository = new Mock<IMoveStatRepository>();
            mockRepository.Setup(m => m.GetRecordedResults(It.IsAny<MoveStatUniqueKey>())).Returns((MoveStatUniqueKey key) =>
            {
                if (dict.ContainsKey(key))
                    return dict[key];
                else
                    return new MoveStat();
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
            readerWrapperMock.Setup(m => m.ReadLine()).Returns(() =>
            {
                return savedText.Count > 0 ? savedText.Dequeue() : null;
            });

            //Save some stats to a file
            var repository = MoveStatRepository.Instance;
            repository.UnitTestRefresh();
            var key1 = new MoveStatUniqueKey
            {
                CardWillOverpower = true,
                OpponentPercentDone = 33,
                CardPoints = 11,
                UnknownStrongerCards = 0,
                HeldStrongerCards = 3
            };
            var key2 = new MoveStatUniqueKey
            {
                CardWillOverpower = true,
                OpponentPercentDone = 67,
                CardPoints = 11,
                UnknownStrongerCards = 2,
                HeldStrongerCards = 3
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