using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Player_HasSameNameAsUser()
        {
            var expectedName = "NameOfSomeoneImportant";
            var user = new Mock<IUser>();
            user.Setup(m => m.Name).Returns(expectedName);
            var player = new HumanPlayer(user.Object);
            Assert.AreEqual(expectedName, player.Name, "Name of player object and user object match.");
        }

        [TestMethod]
        public void NewbiePlayer_GetMove()
        {
            var mainPlayer = new NewbiePlayer();
            var card1 = CardRepository.Instance[StandardSuite.CLUBS, CardType.JACK];
            var card2 = CardRepository.Instance[StandardSuite.HEARTS, CardType.N8];
            mainPlayer.Cards.AddRange(new List<ICard>() { card1, card2 });
            var playerList = new List<IPlayer>() { new Player(), new Player(), mainPlayer, new Player(), new Player() };
            {
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.IsLegalAddition(card1, mainPlayer)).Returns(true);
                trickMock.Setup(m => m.IsLegalAddition(card2, mainPlayer)).Returns(false);
                var cardPlayed = mainPlayer.GetMove(trickMock.Object);
                Assert.AreEqual(card1, cardPlayed, "Since card1 is legal, that is the card newbie player will play");
            }
            {
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.IsLegalAddition(card1, mainPlayer)).Returns(false);
                trickMock.Setup(m => m.IsLegalAddition(card2, mainPlayer)).Returns(true);
                var cardPlayed = mainPlayer.GetMove(trickMock.Object);
                Assert.AreEqual(card2, cardPlayed, "Since card2 is legal, that is the card newbie player will play");
            }
        }

        private Mock<ITrick> GenerateTrickMock(List<IPlayer> playerList)
        {
            var gameMock = new Mock<IGame>();
            var deckMock = new Mock<IDeck>();
            var handMock = new Mock<IHand>();
            var trickMock = new Mock<ITrick>();
            gameMock.Setup(m => m.PlayerCount).Returns(5);
            gameMock.Setup(m => m.Players).Returns(playerList);
            deckMock.Setup(m => m.Game).Returns(gameMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            trickMock.Setup(m => m.Hand).Returns(handMock.Object);
            return trickMock;
        }

        private IPlayer player1 = new Player();
        private IPlayer player2 = new Player();
        private IPlayer player3 = new Player();
        private IPlayer player4 = new Player();

        [TestMethod]
        public void NewbiePlayer_WillPick()
        {
            var mainPlayer = new NewbiePlayer();
            {
                var playerList = new List<IPlayer>() { player1, player2, mainPlayer, player3, player4 };
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.StartingPlayer).Returns(player2);
                Assert.IsFalse(mainPlayer.WillPick(trickMock.Object), "Newbie Player should not pick if he is not last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, mainPlayer, player3, player4 };
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.StartingPlayer).Returns(player3);
                Assert.IsTrue(mainPlayer.WillPick(trickMock.Object), "Newbie Player should pick if he is last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, player3, player4, mainPlayer };
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.StartingPlayer).Returns(player1);
                Assert.IsTrue(mainPlayer.WillPick(trickMock.Object), "Newbie Player should pick if he is last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, player3, player4, mainPlayer };
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.StartingPlayer).Returns(player3);
                Assert.IsFalse(mainPlayer.WillPick(trickMock.Object), "Newbie Player should not pick if he is not last.");
            }
        }

        [TestMethod]
        public void NewbiePlayer_DropCards()
        {
            var picker = new BasicPlayer();
            picker.Cards.AddRange(new List<ICard>()
            {
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.JACK],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N7],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.N7],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.N8],
                CardRepository.Instance[StandardSuite.SPADES, CardType.N8],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN],
            });
            var cardsToDrop = picker.DropCardsForPick(new Mock<IHand>().Object, picker);
            Assert.IsTrue(cardsToDrop.Contains(CardRepository.Instance[StandardSuite.CLUBS, CardType.N7]), "Drop 7 of Clubs since it is only club.");
            Assert.IsTrue(cardsToDrop.Contains(CardRepository.Instance[StandardSuite.SPADES, CardType.N8]), "Drop 8 of Spades since it is only club.");
        }
    }
}
