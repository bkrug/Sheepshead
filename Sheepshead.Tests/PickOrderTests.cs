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
    public class PickOrderTests
    {
        [TestMethod]
        public void PickPlayerOrderer_GetPickingPlayersInOrder1()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PickPlayerOrderer();
            var actualList = orderer.PlayersInPickOrder(players, players[3]);

            Assert.AreSame(players[3], actualList[0]);
            Assert.AreSame(players[4], actualList[1]);
            Assert.AreSame(players[0], actualList[2]);
            Assert.AreSame(players[1], actualList[3]);
            Assert.AreSame(players[2], actualList[4]);
        }

        [TestMethod]
        public void PickPlayerOrderer_GetPickingPlayersInOrder2()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PickPlayerOrderer();
            var actualList = orderer.PlayersInPickOrder(players, players[0]);

            Assert.AreSame(players[0], actualList[0]);
            Assert.AreSame(players[1], actualList[1]);
            Assert.AreSame(players[2], actualList[2]);
            Assert.AreSame(players[3], actualList[3]);
            Assert.AreSame(players[4], actualList[4]);
        }

        [TestMethod]
        public void PickPlayerOrderer_GetPickingPlayersInOrder3()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PickPlayerOrderer();
            var actualList = orderer.PlayersInPickOrder(players, players[4]);

            Assert.AreSame(players[4], actualList[0]);
            Assert.AreSame(players[0], actualList[1]);
            Assert.AreSame(players[1], actualList[2]);
            Assert.AreSame(players[2], actualList[3]);
            Assert.AreSame(players[3], actualList[4]);
        }

        [TestMethod]
        public void PickPlayerOrderer_GetPlayersWhoHavntPicked_AllPlayers()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PickPlayerOrderer();
            var actualList = orderer.PlayersWithoutPickTurn(players, players.Skip(5).ToList());

            Assert.AreSame(players[0], actualList[0]);
            Assert.AreSame(players[1], actualList[1]);
            Assert.AreSame(players[2], actualList[2]);
            Assert.AreSame(players[3], actualList[3]);
            Assert.AreSame(players[4], actualList[4]);
        }

        [TestMethod]
        public void PickPlayerOrderer_GetPlayersWhoHavntPicked_SomePlayers()
        {
            var playerMocks = new List<Mock<IPlayer>>() { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
            var players = playerMocks.Select(m => m.Object).ToList();

            var orderer = new PickPlayerOrderer();
            var actualList = orderer.PlayersWithoutPickTurn(players, players.Take(2).ToList());

            Assert.AreSame(players[2], actualList[0]);
            Assert.AreSame(players[3], actualList[1]);
            Assert.AreSame(players[4], actualList[2]);
        }
    }
}
