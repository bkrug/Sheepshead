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
    public class BuryKeyGeneratorTests
    {
        private List<ICard> _cardsHeld = new List<ICard> { 
            CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], //Rank = 4
            CardRepository.Instance[StandardSuite.DIAMONDS, CardType.JACK], //Rank = 8
            CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN], //Rank = 3
            CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7], //Rank = 14
            CardRepository.Instance[StandardSuite.SPADES, CardType.N10], //Rank = 22
            CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] //Rank = 15
        };
        private List<ICard> _buried = new List<ICard>
        {
            CardRepository.Instance[StandardSuite.SPADES, CardType.ACE], //Rank = 21
            CardRepository.Instance[StandardSuite.HEARTS, CardType.KING] //Rank = 29
        };

        [TestMethod]
        public void GenerateKey_AtStartOfHand()
        {
            var generator = new BuryKeyGenerator();
            var deckMock = new Mock<IDeck>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(_cardsHeld);

            var expected = new BuryStatUniqueKey()
            {
                AvgBuriedRank = (21 + 29) / 2,
                BuriedPoints = 11 + 4,
                AvgRankInHand = (4 + 8 + 3 + 14 + 22 + 15) / 6,
                PointsInHand = 3 + 2 + 3 + 0 + 10 + 11
            };
            var actual = generator.GenerateKey(_cardsHeld, _buried);

            Assert.AreEqual(expected.AvgBuriedRank, actual.AvgBuriedRank);
            Assert.AreEqual(expected.BuriedPoints, actual.BuriedPoints);
            Assert.AreEqual(expected.AvgRankInHand, actual.AvgRankInHand);
            Assert.AreEqual(expected.PointsInHand, actual.PointsInHand);
        }

        //[TestMethod]
        //public void GenerateKey_AtEndOfHand()
        //{
        //    var generator = new PickKeyGenerator();
        //    var deckMock = new Mock<IDeck>();
        //    var handMock = new Mock<IHand>();
        //    var trickList = new List<Mock<ITrick>>();
        //    var playerMock = new Mock<IPlayer>();
        //    playerMock.Setup(m => m.Cards).Returns(new List<ICard> ());
        //    var blinds = new List<ICard> {
        //        CardRepository.Instance[StandardSuite.HEARTS, CardType.N8],
        //        CardRepository.Instance[StandardSuite.CLUBS, CardType.KING]
        //    };
        //    for (var i = 0; i < _cardsDealt.Count; ++i)
        //        trickList.Add(new Mock<ITrick>());
        //    trickList[0].Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>() { { playerMock.Object, blinds.First() } });
        //    for (var i = 1; i < _cardsDealt.Count; ++i)
        //        trickList[i].Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>() { { playerMock.Object, _cardsDealt[i] } });
        //    deckMock.Setup(m => m.Buried).Returns(blinds.Skip(1).Union(_cardsDealt.Take(1)).ToList());
        //    deckMock.Setup(m => m.Blinds).Returns(blinds);
        //    handMock.Setup(m => m.Picker).Returns(playerMock.Object);
        //    handMock.Setup(m => m.Tricks).Returns(trickList.Select(m => m.Object).ToList());
        //    handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            
        //    var expected = GetExpected();
        //    var actual = generator.GenerateKey(handMock.Object, playerMock.Object);

        //    Assert.AreEqual(expected.TrumpCount, actual.TrumpCount);
        //    Assert.AreEqual(expected.AvgTrumpRank, actual.AvgTrumpRank);
        //    //Assert.AreEqual(expected.TrumpStdDeviation, actual.TrumpStdDeviation);
        //    Assert.AreEqual(expected.PointsInHand, actual.PointsInHand);
        //    Assert.AreEqual(expected.TotalCardsWithPoints, actual.TotalCardsWithPoints);
        //}

        private static BuryStatUniqueKey GetExpected()
        {
            var expected = new BuryStatUniqueKey()
            {
                AvgBuriedRank = 0,
                BuriedPoints = 0,
                AvgRankInHand = 0,
                PointsInHand = 0
            };
            return expected;
        }
    }
}
