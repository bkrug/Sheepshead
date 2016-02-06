using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using System.Collections.Generic;
using Moq;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Tests.NonUnitTests
{
    [TestClass]
    public class TimeTests
    {
        //[TestMethod]
        public void MoveStat_RunTimeOf_GetWeightedResult()
        {
            {
                //In this case there is too little data to look at one stat.  It will keep combining stats until there have been 1000 tries.
                var testKey1 = new MoveStatUniqueKey()
                {
                    CardWillOverpower = false,
                    OpponentPercentDone = 50,
                    CardPoints = 10,
                    UnknownStrongerCards = 8,
                    HeldStrongerCards = 1
                };
                var testResult1 = new MoveStat()
                {
                    TricksTried = 601,
                    TricksWon = 43,
                    HandsTried = 602,
                    HandsWon = 97
                };
                var dict = new Dictionary<MoveStatUniqueKey, MoveStat>()
                {
                    { testKey1, testResult1 }
                };
                var mockRepository = new Mock<IMoveStatRepository>();
                mockRepository.Setup(m => m.GetRecordedResults(It.IsAny<MoveStatUniqueKey>()))
                    .Returns((MoveStatUniqueKey givenKey) => dict.ContainsKey(givenKey) ? dict[givenKey] : new MoveStat());
                var predictor = new MoveStatResultPredictor(mockRepository.Object);
                var startTime = DateTime.Now;
                for (var i = 0; i < 100; ++i)
                    predictor.GetWeightedStat(testKey1);
                var duration = (DateTime.Now - startTime).TotalSeconds;
            }
        }
    }
}