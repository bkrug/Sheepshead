using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Model;
using Sheepshead.Model.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class LeasterStateAnalyzerTests
    {
        [TestMethod]
        public void LeasterStateAnalyzer_CanIWin_Yes()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.QUEEN_HEARTS,
                SheepCard.N7_DIAMONDS,
                SheepCard.KING_HEARTS
            };
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(cards);
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.KING_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.QUEEN_HEARTS, playerMock.Object)).Returns(true);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.N7_DIAMONDS, playerMock.Object)).Returns(true);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.CanIWin(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void LeasterStateAnalyzer_CanIWin_No()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.QUEEN_SPADES,
                SheepCard.N8_HEARTS,
                SheepCard.KING_SPADES
            };
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(cards);
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.N8_HEARTS, playerMock.Object)).Returns(true);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.CanIWin(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void LeasterStateAnalyzer_CanILoose_Yes()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.QUEEN_HEARTS,
                SheepCard.N7_DIAMONDS,
                SheepCard.KING_HEARTS
            };
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(cards);
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.KING_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.QUEEN_HEARTS, playerMock.Object)).Returns(true);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.N7_DIAMONDS, playerMock.Object)).Returns(true);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.CanILoose(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void LeasterStateAnalyzer_CanILoose_No()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.QUEEN_SPADES,
                SheepCard.JACK_HEARTS,
                SheepCard.KING_SPADES
            };
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(cards);
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.QUEEN_SPADES, playerMock.Object)).Returns(true);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.JACK_HEARTS, playerMock.Object)).Returns(true);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.CanILoose(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }


        [TestMethod]
        public void LeasterStateAnalyzer_EarlyInTrick_5Player_Yes()
        {
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.KING_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Game.PlayerCount).Returns(5);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.EarlyInTrick(trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void LeasterStateAnalyzer_EarlyInTrick_5Player_No()
        {
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Game.PlayerCount).Returns(5);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.EarlyInTrick(trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void LeasterStateAnalyzer_EarlyInTrick_3Player_Yes()
        {
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Game.PlayerCount).Returns(3);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.EarlyInTrick(trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void LeasterStateAnalyzer_EarlyInTrick_3Player_No()
        {
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_DIAMONDS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Game.PlayerCount).Returns(3);
            var analyzer = new LeasterStateAnalyzer();
            var actual = analyzer.EarlyInTrick(trickMock.Object);
            Assert.AreEqual(false, actual);
        }
    }
}
