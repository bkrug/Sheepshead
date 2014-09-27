using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using Moq;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameCreationTests
    {
        //[TestMethod]
        //public void GameService_CreatesAGame()
        //{
        //    var expectedName = "Game C";
        //    var humanPlayers = 2;
        //    var playerCount = 5;
        //    var gameRepositoryMock = new Mock<IGameRepository>();
        //    gameRepositoryMock
        //        .Setup(pr => pr.CreateGame(It.IsAny<string>()))
        //        .Returns((string s) => new Game(13) { Name = s });
        //    var service = new GameService(gameRepositoryMock.Object);
        //    var actual = service.CreateGame(expectedName, playerCount, humanPlayers);
        //    Assert.IsTrue(actual is IGame, "Game recieved.");
        //    Assert.AreEqual(actual.Name, expectedName, "Correct Name used.");
        //    Assert.AreEqual(actual.MaxPlayers, playerCount, "Number of players is correct.");
        //    Assert.AreEqual(actual.MaxHumanPlayers, humanPlayers, "Number of human players is correct.");
        //}

        //[TestMethod]
        //public void GameService_RetreivesAGame()
        //{
        //    var gameId = (long)45;
        //    var expectedName = "Game 45";
        //    var gameList = new List<IGame>() { new Game(gameId) { Name = expectedName } };
        //    var gameRepositoryMock = new Mock<IGameRepository>();
        //    gameRepositoryMock.Setup(pr => pr
        //        .GetById(It.IsAny<long>()))
        //        .Returns((long num) => gameList.FirstOrDefault(l => l.Id == num));
        //    var service = new GameService(gameRepositoryMock.Object);
        //    var actualGame = service.GetGame(gameId);
        //    Assert.AreEqual(actualGame.Id, gameId, "Ids match");
        //    Assert.AreEqual(actualGame.Name, expectedName, "Names match");
        //}

        //[TestMethod]
        //public void AfterJoiningGame_RepositorySavesGame()
        //{
        //    var userID = 12;
        //    var numPlayes = 5;
        //    var numHumanPlayers = 3;
        //    GameRepository.Instance.Refresh();
        //    var curGame = GameRepository.Instance.JoinGame(userID, numPlayes, numHumanPlayers);
        //    Assert.AreEqual(curGame.Id, Game.GetGames().First().Id, "The created game was retained.");
        //}

        //[TestMethod]
        //public void WeHaveCanEraseAllData()
        //{
        //    var userID = 12;
        //    var numPlayes = 5;
        //    var numHumanPlayers = 3;
        //    GameRepository.Instance.Refresh();
        //    var curGame = GameRepository.Instance.JoinGame(userID, numPlayes, numHumanPlayers);
        //    Assert.IsTrue(Game.GetGames().Any(), "There are some games initially");

        //    GameRepository.Instance.Refresh();
        //    Assert.IsTrue(!Game.GetGames().Any(), "There are no games after refresh");
        //}

        //[TestMethod]
        //public void FirstUserToJoinAGameGetsNewGame()
        //{
        //    var userID = 12;
        //    var numPlayes = 5;
        //    var numHumanPlayers = 3;
        //    GameRepository.Instance.Refresh();
        //    var curGame = GameRepository.Instance.JoinGame(userID, numPlayes, numHumanPlayers);
        //    Assert.AreEqual(1, curGame.HumanPlayers.Count, "The current user is the only player.");
        //    Assert.AreEqual(userID, curGame.HumanPlayers.First().UserId, "The current user is the only player.");
        //}

        //[TestMethod]
        //public void SecondUserToJoinAGameGetsExistingGame()
        //{
        //    var userId1 = 12;
        //    var userId2 = 13;
        //    var numPlayes = 5;
        //    var numHumanPlayers = 3;
        //    GameRepository.Instance.Refresh();
        //    var curGame1 = GameRepository.Instance.JoinGame(userId1, numPlayes, numHumanPlayers);
        //    var curGame2 = GameRepository.Instance.JoinGame(userId2, numPlayes, numHumanPlayers);
        //    Assert.AreEqual(curGame1.Id, curGame2.Id, "Both players joined the same game.");
        //}

        //[TestMethod]
        //public void UsersJoiningDifferentTypesOfGamesGetDifferentGames()
        //{
        //    var userId1 = 12;
        //    var userId2 = 13;
        //    var numPlayers1 = 5;
        //    var numPlayers2 = 3;
        //    var numHumanPlayers = 3;
        //    GameRepository.Instance.Refresh();
        //    var curGame1 = GameRepository.Instance.JoinGame(userId1, numPlayers1, numHumanPlayers);
        //    var curGame2 = GameRepository.Instance.JoinGame(userId2, numPlayers2, numHumanPlayers);
        //    Assert.AreNotEqual(curGame1.Id, curGame2.Id, "Both players joined the same game.");
        //}

        //[TestMethod]
        //public void WhenGameFull_NewUserJoinsADifferentGame()
        //{
        //    var userId1 = 12;
        //    var userId2 = 13;
        //    var userId3 = 14;
        //    var numPlayers = 5;
        //    var numHumanPlayers = 2;
        //    GameRepository.Instance.Refresh();
        //    var curGame1 = GameRepository.Instance.JoinGame(userId1, numPlayers, numHumanPlayers);
        //    var curGame2 = GameRepository.Instance.JoinGame(userId2, numPlayers, numHumanPlayers);
        //    var curGame3 = GameRepository.Instance.JoinGame(userId3, numPlayers, numHumanPlayers);
        //    Assert.AreEqual(curGame1.Id, curGame2.Id, "First two players are in the same game.");
        //    Assert.AreNotEqual(curGame1.Id, curGame3.Id, "Third player is in a different game.");
        //}

        //[TestMethod]
        //public void WhenCallerRequestsListOfPlayers_HeCannotChangeList()
        //{
        //    var userId1 = 12;
        //    var userId2 = 13;
        //    var userId3 = 14;
        //    var numPlayers = 5;
        //    var numHumanPlayers = 3;
        //    GameRepository.Instance.Refresh();
        //    var curGame1 = GameRepository.Instance.JoinGame(userId1, numPlayers, numHumanPlayers);
        //    var curGame2 = GameRepository.Instance.JoinGame(userId2, numPlayers, numHumanPlayers);
        //    var curGame3 = GameRepository.Instance.JoinGame(userId3, numPlayers, numHumanPlayers);
        //    Assert.IsTrue(curGame1.HumanPlayers.Any(), "The list has some entrys.");
        //    curGame1.HumanPlayers.RemoveAll(p => p.Id > 0);
        //    Assert.IsTrue(curGame1.HumanPlayers.Any(), "There are still some entrys in the list.");
        //}
    }
}
