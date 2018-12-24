using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Model;
using Sheepshead.Model.Wrappers;
using Sheepshead.Model.Players;
using Sheepshead.Model.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameRepositoryTests
    {
        [TestMethod]
        public void GameRepository_CreationOfGameRetainsGame()
        {
            var dict = new Dictionary<Guid, IGame>();
            var repository = new OldGameRepository(dict);
            var game = repository.Create(1, 4, 0, 0, PartnerMethod.JackOfDiamonds, true);
            Assert.IsTrue(dict.ContainsKey(game.Id), "Game returned is in the dictionary.");
            Assert.AreSame(dict[game.Id], game, "Game returned is in the dictionary.");
            Assert.AreEqual(1, dict.Count, "Game Repository did not create extra tests.");
        }

        [TestMethod]
        public void GameRepository_GetGameReturnsCorrectGame()
        {
            var dict = new Dictionary<Guid, IGame>();
            var repository = new OldGameRepository(dict);
            var expected0 = AddGame(dict, new Game(new List<IPlayer>(), PartnerMethod.JackOfDiamonds, new RandomWrapper(), null));
            var expected1 = AddGame(dict, new Game(new List<IPlayer>(), PartnerMethod.JackOfDiamonds, new RandomWrapper(), null));
            var expected2 = AddGame(dict, new Game(new List<IPlayer>(), PartnerMethod.JackOfDiamonds, new RandomWrapper(), null));
            Assert.AreEqual(dict[expected1].Id, repository.GetGame(g => g.Id == expected1).Id, "GetGame() returned correct results when searching by id.");
            Assert.AreEqual(dict[expected2].Id, repository.GetGame(g => g.Id == expected2).Id, "GetGame() returned correct results when searching by id again.");
        }

        [TestMethod]
        public void GameRepository_GetGameReturnsCorrectGame2()
        {
            var dict = new Dictionary<Guid, IGame>();
            var repository = new BaseRepository<IGame>(dict);
            var expected = 
            AddGame(dict, new Game(new List<IPlayer>(), PartnerMethod.JackOfDiamonds, new RandomWrapper(), null));
            AddGame(dict, new Game(new List<IPlayer>(), PartnerMethod.JackOfDiamonds, new RandomWrapper(), null));
            AddGame(dict, new Game(new List<IPlayer>(), PartnerMethod.JackOfDiamonds, new RandomWrapper(), null));
            Assert.IsNull(repository.GetById(Guid.NewGuid()), "GetGame() returned null and not an error when there were no results.");
        }

        private Guid AddGame(Dictionary<Guid, IGame> dict, IGame game)
        {
            dict.Add(game.Id, game);
            return game.Id;
        }
    }
}
