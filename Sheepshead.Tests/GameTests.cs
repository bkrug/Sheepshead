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
            public ExposeGame() : base (0)
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
        public void Game_CanAddPlayerToGame()
        {
            var exposedGame = new ExposeGame() { MaxPlayers = 5, MaxHumanPlayers = 1 };
            var game = (Game)exposedGame;
            var mockPlayer1 = new Mock<IPlayer>();
            var mockPlayer2 = new Mock<IPlayer>();
            game.AddPlayer(mockPlayer1.Object);
            Assert.IsTrue(exposedGame.Players.Contains(mockPlayer1.Object), "Player was added");
            game.AddPlayer(mockPlayer2.Object);
            Assert.IsTrue(exposedGame.Players.Contains(mockPlayer2.Object), "Second Player was added");
            Assert.IsTrue(exposedGame.Players.Contains(mockPlayer1.Object), "First Player was not removed.");
            try
            {
                game.AddPlayer(mockPlayer1.Object);
                Assert.Fail("Should not have been able to re-add the first player.");
            }
            catch (ObjectInListException ex)
            {
                Assert.IsTrue(true, "Error occurred on attempt to re-add a player already in list.");
            }
            catch (Exception ex) 
            {
                Assert.Fail("Incorrect error thrown when attempting to re-add a player already in list: " + ex.Message);
            }
        }

        [TestMethod]
        public void Game_CannotAddMoreThanMaximumPlayersToGame()
        {
            var exposedGame = new ExposeGame() { MaxPlayers = 3, MaxHumanPlayers = 1 };
            var game = (Game)exposedGame;
            var mockPlayer1 = new Mock<IPlayer>();
            var mockPlayer2 = new Mock<IPlayer>();
            var mockPlayer3 = new Mock<IPlayer>();
            var mockPlayer4 = new Mock<IPlayer>();
            game.AddPlayer(mockPlayer1.Object);
            game.AddPlayer(mockPlayer2.Object);
            game.AddPlayer(mockPlayer3.Object);
            try
            {
                game.AddPlayer(mockPlayer4.Object);
                Assert.Fail("An error should have occrred when attempting to add too many players.");
            }
            catch (TooManyPlayersException ex)
            {
                Assert.IsTrue(true, "Error thrown when attempting to add more players than the game allows.");
            }
            catch (Exception ex) {
                Assert.Fail("Incorrect error thrown when attempting to add too many players: " + ex.Message);
            }
        }

        [TestMethod]
        public void Game_CannotCountHumanPlayersInGame()
        {
            var exposedGame = new ExposeGame();
            var game = (Game)exposedGame;
            var mockPlayer1 = new Mock<IPlayer>();
            var mockPlayer2 = new Mock<IHumanPlayer>();
            var mockPlayer3 = new Mock<IHumanPlayer>();
            game.MaxPlayers = 3;
            game.MaxHumanPlayers = 3;
            exposedGame.Players = new List<IPlayer>() { mockPlayer1.Object };
            Assert.AreEqual(0, game.HumanPlayerCount, "Correct number of human players counted.");
            exposedGame.Players = new List<IPlayer>() { mockPlayer1.Object, mockPlayer2.Object };
            Assert.AreEqual(1, game.HumanPlayerCount, "Correct number of human players counted.");
            exposedGame.Players = new List<IPlayer>() { mockPlayer1.Object, mockPlayer2.Object, mockPlayer3.Object };
            Assert.AreEqual(2, game.HumanPlayerCount, "Correct number of human players counted.");
            exposedGame.Players = new List<IPlayer>() { mockPlayer2.Object, mockPlayer3.Object };
            Assert.AreEqual(2, game.HumanPlayerCount, "Correct number of human players counted.");
        }

        [TestMethod]
        public void Game_CannotAddMorethanMaximumHumanPlayersToGame()
        {
            {
                var exposedGame = new ExposeGame();
                var game = (Game)exposedGame;
                var mockPlayer1 = new Mock<IPlayer>();
                var mockPlayer2 = new Mock<IHumanPlayer>();
                var mockPlayer3 = new Mock<IHumanPlayer>();
                game.MaxPlayers = 3;
                game.MaxHumanPlayers = 1;
                game.AddPlayer(mockPlayer1.Object);
                game.AddPlayer(mockPlayer2.Object);
                try
                {
                    game.AddPlayer(mockPlayer3.Object);
                    Assert.Fail("An error should have occrred when attempting to add too many players.");
                }
                catch (TooManyHumanPlayersException ex)
                {
                    Assert.IsTrue(true, "Error thrown when attempting to add more players than the game allows.");
                }
                catch (Exception ex)
                {
                    Assert.Fail("Incorrect error thrown when attempting to add too many players: " + ex.Message);
                }
            }

            {
                var exposedGame = new ExposeGame();
                var game = (Game)exposedGame;
                var mockPlayer1 = new Mock<IHumanPlayer>();
                var mockPlayer2 = new Mock<IHumanPlayer>();
                game.MaxPlayers = 3;
                game.MaxHumanPlayers = 1;
                game.AddPlayer(mockPlayer1.Object);
                try
                {
                    game.AddPlayer(mockPlayer2.Object);
                    Assert.Fail("An error should have occrred when attempting to add too many players.");
                }
                catch (TooManyHumanPlayersException ex)
                {
                    Assert.IsTrue(true, "Error thrown when attempting to add more players than the game allows.");
                }
                catch (Exception ex)
                {
                    Assert.Fail("Incorrect error thrown when attempting to add too many players: " + ex.Message);
                }
            }
        }

        [TestMethod]
        public void Deck_MakeDeck()
        {
            var game = new Game(4982) { MaxPlayers = 5, MaxHumanPlayers = 5 };
            for (var i = 0; i < 5; ++i)
                game.AddPlayer(new Player());
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
            var game = new Game(42340) { MaxPlayers = 5, MaxHumanPlayers = 2 };
            deckMock.Setup(m => m.Game).Returns(game);
            foreach (var player in playerList)
                game.AddPlayer(player);
            trickMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            bool player1Moved = false;
            bool player3Moved = false;
            bool player4Moved = false;
            player1.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player1Moved = true);
            player3.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player3Moved = true);
            player4.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player4Moved = true);
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>());
            game.PlayNonHumans(trickMock.Object);
            Assert.IsTrue(player1Moved);
            Assert.IsFalse(player3Moved);
            Assert.IsFalse(player4Moved);
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>() { { player1.Object, new Card() }, { player2, new Card() } });
            game.PlayNonHumans(trickMock.Object);
            Assert.IsTrue(player3Moved);
            Assert.IsTrue(player4Moved);
        }
    }
}
