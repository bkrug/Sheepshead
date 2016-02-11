using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameRepositoryTests
    {
        [TestMethod]
        public void GameRepository_CreationOfGameRetainsGame()
        {
            var dict = new Dictionary<long, IGame>();
            var repository = new GameRepository(dict);
            var expectedName = "Dwayne's Game";
            var learningHelperFactory = new Mock<ILearningHelperFactory>();
            var game = repository.CreateGame(expectedName, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object);
            game.Name = expectedName;
            repository.Save(game);
            Assert.AreEqual(expectedName, game.Name, "Name is correct.");
            Assert.IsTrue(dict.ContainsKey(game.Id), "Game returned is in the dictionary.");
            Assert.AreSame(dict[game.Id], game, "Game returned is in the dictionary.");
            Assert.AreEqual(1, dict.Count, "Game Repository did not create extra tests.");
        }

        [TestMethod]
        public void GameRepository_GetGameReturnsCorrectGame()
        {
            var dict = new Dictionary<long, IGame>();
            var repository = new GameRepository(dict);
            var learningHelperFactory = new Mock<ILearningHelperFactory>();
            dict.Add(101, new Game(101, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object) { Name = "Fred's Game" });
            dict.Add(102, new Game(102, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object) { Name = "Bill's Game" });
            dict.Add(103, new Game(103, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object) { Name = "Andy's Game" });
            Assert.AreEqual(dict[102].Name, repository.GetGame(g => g.Id == 102).Name, "GetGame() returned correct results when searching by id.");
            Assert.AreEqual(dict[103].Id, repository.GetGame(g => g.Name == "Andy's Game").Id, "GetGame() returned correct results when searching by Name.");
        }

        [TestMethod]
        public void GameRepository_GetGameReturnsCorrectGame2()
        {
            var dict = new Dictionary<long, IGame>();
            var repository = new BaseRepository<IGame>(dict);
            var learningHelperFactory = new Mock<ILearningHelperFactory>();
            dict.Add(101, new Game(101, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object) { Name = "Fred's Game" });
            dict.Add(102, new Game(102, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object) { Name = "Bill's Game" });
            dict.Add(103, new Game(103, new List<IPlayer>(), new RandomWrapper(), learningHelperFactory.Object) { Name = "Andy's Game" });
            Assert.AreEqual(dict[101].Name, repository.GetById(101).Name, "GetGame() returned correct results when searching by id.");
            Assert.AreEqual(null, repository.GetById(104), "GetGame() returned null and not an error when there were no results.");
        }

        //[TestMethod]
        //public void GameRepository_GetOpenGames()
        //{
        //    var dict = new Dictionary<long, IGame>();
        //    var repository = new GameRepository(dict);
        //    var game1 = new Mock<IGame>();
        //    game1.Setup(m => m.Id).Returns(1);
        //    game1.Setup(m => m.HumanPlayerCount).Returns(1);
        //    dict.Add(game1.Object.Id, game1.Object);
        //    var game2 = new Mock<IGame>();
        //    game2.Setup(m => m.Id).Returns(2);
        //    game2.Setup(m => m.HumanPlayerCount).Returns(2);
        //    dict.Add(game2.Object.Id, game2.Object);
        //    var openGames = repository.GetOpenGames().ToList();
        //    Assert.AreEqual(1, openGames.Count, "Correct number of games returned.");
        //    Assert.AreSame(game1.Object, openGames.ElementAt(0), "Correct game returned.");
        //}

        //[TestMethod]
        //public void GameRepository_GetOpenGames2()
        //{
        //    var dict = new Dictionary<long, IGame>();
        //    var repository = new GameRepository(dict);
        //    var game1 = new Mock<IGame>();
        //    game1.Setup(m => m.Id).Returns(1);
        //    game1.Setup(m => m.HumanPlayerCount).Returns(1);
        //    dict.Add(game1.Object.Id, game1.Object);
        //    var game2 = new Mock<IGame>();
        //    game2.Setup(m => m.Id).Returns(2);
        //    game2.Setup(m => m.HumanPlayerCount).Returns(2);
        //    dict.Add(game2.Object.Id, game2.Object);
        //    var game3 = new Mock<IGame>();
        //    game3.Setup(m => m.Id).Returns(3);
        //    game3.Setup(m => m.HumanPlayerCount).Returns(2);
        //    dict.Add(game3.Object.Id, game3.Object);
        //    var openGames = repository.GetOpenGames().ToList();
        //    Assert.IsTrue(openGames.Contains(game1.Object), "Game 1 returned.");
        //    Assert.IsTrue(openGames.Contains(game3.Object), "Game 3 returned.");
        //    Assert.AreEqual(2, openGames.Count, "Correct number of games returned.");
        //}
    }
}
