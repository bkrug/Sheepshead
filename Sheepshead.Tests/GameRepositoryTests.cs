﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;


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
            var game = repository.CreateGame(expectedName, new List<IPlayer>(), new RandomWrapper());
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
            dict.Add(101, new Game(101, new List<IPlayer>(), new RandomWrapper(), null, null) { Name = "Fred's Game" });
            dict.Add(102, new Game(102, new List<IPlayer>(), new RandomWrapper(), null, null) { Name = "Bill's Game" });
            dict.Add(103, new Game(103, new List<IPlayer>(), new RandomWrapper(), null, null) { Name = "Andy's Game" });
            Assert.AreEqual(dict[102].Name, repository.GetGame(g => g.Id == 102).Name, "GetGame() returned correct results when searching by id.");
            Assert.AreEqual(dict[103].Id, repository.GetGame(g => g.Name == "Andy's Game").Id, "GetGame() returned correct results when searching by Name.");
        }

        [TestMethod]
        public void GameRepository_GetGameReturnsCorrectGame2()
        {
            var dict = new Dictionary<long, IGame>();
            var repository = new BaseRepository<IGame>(dict);
            dict.Add(101, new Game(101, new List<IPlayer>(), new RandomWrapper(), null, null) { Name = "Fred's Game" });
            dict.Add(102, new Game(102, new List<IPlayer>(), new RandomWrapper(), null, null) { Name = "Bill's Game" });
            dict.Add(103, new Game(103, new List<IPlayer>(), new RandomWrapper(), null, null) { Name = "Andy's Game" });
            Assert.AreEqual(dict[101].Name, repository.GetById(101).Name, "GetGame() returned correct results when searching by id.");
            Assert.AreEqual(null, repository.GetById(104), "GetGame() returned null and not an error when there were no results.");
        }
    }
}
