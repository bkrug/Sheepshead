using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PlayCreatorTests
    {
        [TestMethod]
        public void PlayCreator_PlayWeakestWin()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            var playerCards = new List<SheepCard>()
            {
                SheepCard.KING_HEARTS,
                SheepCard.JACK_DIAMONDS,
                SheepCard.N10_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            playerMock.Setup(m => m.Cards).Returns(playerCards);
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.ACE_SPADES },
                { new Mock<IPlayer>().Object, SheepCard.ACE_DIAMONDS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.JACK_DIAMONDS, playerMock.Object)).Returns(true);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.QUEEN_CLUBS, playerMock.Object)).Returns(true);
            var playCreator = new PlayCreator();
            var actual = playCreator.PlayWeakestWin(playerMock.Object, trickMock.Object);
            Assert.AreEqual(SheepCard.JACK_DIAMONDS, actual);
        }

        [TestMethod]
        public void PlayCreator_PlayStrongestWin()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            var playerCards = new List<SheepCard>()
            {
                SheepCard.KING_HEARTS,
                SheepCard.JACK_DIAMONDS,
                SheepCard.N10_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            playerMock.Setup(m => m.Cards).Returns(playerCards);
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.ACE_SPADES },
                { new Mock<IPlayer>().Object, SheepCard.ACE_DIAMONDS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.JACK_DIAMONDS, playerMock.Object)).Returns(true);
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.QUEEN_CLUBS, playerMock.Object)).Returns(true);
            var playCreator = new PlayCreator();
            var actual = playCreator.PlayStrongestWin(playerMock.Object, trickMock.Object);
            Assert.AreEqual(SheepCard.QUEEN_CLUBS, actual);
        }
    }
}
