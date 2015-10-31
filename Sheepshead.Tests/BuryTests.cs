using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests
{
    [TestClass]
    public class BuryTests
    {
        private List<ICard> _cardsHeld = new List<ICard> { 
            CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], //Rank = 4
            CardRepository.Instance[StandardSuite.DIAMONDS, CardType.JACK], //Rank = 8
            CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN], //Rank = 3
            CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7], //Rank = 14
            CardRepository.Instance[StandardSuite.SPADES, CardType.N10], //Rank = 16
            CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] //Rank = 15
        };
        private List<ICard> _buried = new List<ICard>
        {
            CardRepository.Instance[StandardSuite.SPADES, CardType.ACE], //Rank = 16
            CardRepository.Instance[StandardSuite.HEARTS, CardType.KING] //Rank = 17
        };

        [TestMethod]
        public void GenerateBuryKey_AtStartOfHand()
        {
            var generator = new BuryKeyGenerator();
            var deckMock = new Mock<IDeck>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(_cardsHeld);

            var expected = new BuryStatUniqueKey()
            {
                BuriedPoints = 11 + 4,
                AvgRankInHand = (int)Math.Round((4 + 8 + 3 + 14 + 16 + 15) / (double)6),
                PointsInHand = 3 + 2 + 3 + 0 + 10 + 11,
                SuitsInHand = 4
            };
            var actual = generator.GenerateKey(_cardsHeld, _buried);

            Assert.AreEqual(expected.BuriedPoints, actual.BuriedPoints);
            Assert.AreEqual(expected.AvgRankInHand, actual.AvgRankInHand);
            Assert.AreEqual(expected.PointsInHand, actual.PointsInHand);
            Assert.AreEqual(expected.SuitsInHand, actual.SuitsInHand);
        }

        [TestMethod]
        public void BuryStatGuessRepository_GetGuessStat()
        {
            var key1 = new BuryStatUniqueKey()
            {
                BuriedPoints = 20,
                AvgRankInHand = 5,
                PointsInHand = 30,
                SuitsInHand = 2
            };
            var key2 = new BuryStatUniqueKey()
            {
                BuriedPoints = 5,
                AvgRankInHand = 18,
                PointsInHand = 9,
                SuitsInHand = 2
            };
            var repository = new BuryStatGuesser();
            var stat1 = repository.GetRecordedResults(key1);
            var stat2 = repository.GetRecordedResults(key2);
            Assert.IsTrue(stat1.AvgPickPoints > stat2.AvgPickPoints, "key1 is more likely to render positive points than key2.");
        }
    }
}
