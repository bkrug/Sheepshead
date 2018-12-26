using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Logic;
using Sheepshead.Logic.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PlayerOrdererTests
    {
        [TestMethod]
        public void PlayerOrderer_GetPickingPlayersInOrder1()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PlayerOrderer();
            var actualList = orderer.PlayersInTurnOrder(players, players[3]);

            Assert.AreSame(players[3], actualList[0]);
            Assert.AreSame(players[4], actualList[1]);
            Assert.AreSame(players[0], actualList[2]);
            Assert.AreSame(players[1], actualList[3]);
            Assert.AreSame(players[2], actualList[4]);
        }

        [TestMethod]
        public void PlayerOrderer_GetPickingPlayersInOrder2()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PlayerOrderer();
            var actualList = orderer.PlayersInTurnOrder(players, players[0]);

            Assert.AreSame(players[0], actualList[0]);
            Assert.AreSame(players[1], actualList[1]);
            Assert.AreSame(players[2], actualList[2]);
            Assert.AreSame(players[3], actualList[3]);
            Assert.AreSame(players[4], actualList[4]);
        }

        [TestMethod]
        public void PlayerOrderer_GetPickingPlayersInOrder3()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PlayerOrderer();
            var actualList = orderer.PlayersInTurnOrder(players, players[4]);

            Assert.AreSame(players[4], actualList[0]);
            Assert.AreSame(players[0], actualList[1]);
            Assert.AreSame(players[1], actualList[2]);
            Assert.AreSame(players[2], actualList[3]);
            Assert.AreSame(players[3], actualList[4]);
        }

        [TestMethod]
        public void PlayerOrderer_GetPlayersWhoHavntPicked_AllPlayers()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PlayerOrderer();
            var actualList = orderer.PlayersWithoutTurn(players, players.Skip(5).ToList());

            Assert.AreSame(players[0], actualList[0]);
            Assert.AreSame(players[1], actualList[1]);
            Assert.AreSame(players[2], actualList[2]);
            Assert.AreSame(players[3], actualList[3]);
            Assert.AreSame(players[4], actualList[4]);
        }

        [TestMethod]
        public void PlayerOrderer_GetPlayersWhoHavntPicked_SomePlayers()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PlayerOrderer();
            var actualList = orderer.PlayersWithoutTurn(players, players.Take(2).ToList());

            Assert.AreSame(players[2], actualList[0]);
            Assert.AreSame(players[3], actualList[1]);
            Assert.AreSame(players[4], actualList[2]);
        }
    }
}
