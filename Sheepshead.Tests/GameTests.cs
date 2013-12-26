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
    }
}
