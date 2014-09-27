using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameTests
    {
        private class ExposeGame : Game {
            public ExposeGame() : base (0, new List<IPlayer>())
            {

            }

            public List<IPlayer> Players
            {
                get { return _players; }
                set { _players = value; }
            }
        }

        [TestMethod]
        public void Game_CanCountAllPlayersInGame()
        {
            var exposedGame = new ExposeGame();
            var game = (Game)exposedGame;
            var mockPlayer1 = new Mock<IPlayer>();
            var mockPlayer2 = new Mock<IPlayer>();
            var mockPlayer3 = new Mock<IPlayer>();
            exposedGame.Players = new List<IPlayer>() { mockPlayer1.Object, mockPlayer2.Object, mockPlayer3.Object };
            Assert.AreEqual(exposedGame.Players.Count, game.PlayerCount, "Returned correct number of players");
        }

        [TestMethod]
        public void Deck_MakeDeck()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new Player());
            var game = new Game(4982, playerList);
            var deck = new Deck(game);
            Assert.AreEqual(2, deck.Blinds.Count(), "There should be two blinds after dealing");
            Assert.AreEqual(5, game.Players.Count(), "There should be five doctores");
            foreach (var player in deck.Game.Players)
                Assert.AreEqual(6, player.Cards.Count(), "There are 6 cards in each players hand.");
        }

        [TestMethod]
        public void Game_PlayNonHuman()
        {
            var player1 = new Mock<BasicPlayer>();
            var player2 = new HumanPlayer(new User());
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<BasicPlayer>();
            var player5 = new HumanPlayer(new User());
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5, player1.Object, player2 };
            var deckMock = new Mock<IDeck>();
            var handMock = new Mock<IHand>();
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.Hand).Returns(handMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            var game = new Game(42340, playerList);
            deckMock.Setup(m => m.Game).Returns(game);
            trickMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            bool player1Moved = false;
            bool player3Moved = false;
            bool player4Moved = false;
            player1.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player1Moved = true);
            player3.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player3Moved = true);
            player4.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player4Moved = true);
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>());
            game.PlayNonHumans(trickMock.Object);
            Assert.IsTrue(player1Moved, "All players from the starting player to the first human should have been played.");
            Assert.IsFalse(player3Moved, "Plyaer 3 should not have been played yet.");
            Assert.IsFalse(player4Moved, "Player 4 should not have been played yet.");
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>() { { player1.Object, new Card() }, { player2, new Card() } });
            game.PlayNonHumans(trickMock.Object);
            player1Moved = false;
            Assert.IsFalse(player1Moved, "PLayer 1 should not have been played a second time.");
            Assert.IsTrue(player3Moved, "Player 3 should have been played when a second call was made to PlayNonHumans()");
            Assert.IsTrue(player4Moved, "Player 4 should have been played when a second call was made to PlayNonHumans()");
        }
    }
}
