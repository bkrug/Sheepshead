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
            var deckMock = new Mock<IDeck>();
            var players = playerMocks.Select(m => m.Object).ToList();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[3]);

            var orderer = new PickPlayerOrdererInner(deckMock.Object);
            var actualList = orderer.PlayersInPickOrder;

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
            var deckMock = new Mock<IDeck>();
            var players = playerMocks.Select(m => m.Object).ToList();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);

            var orderer = new PickPlayerOrdererInner(deckMock.Object);
            var actualList = orderer.PlayersInPickOrder;

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
            var deckMock = new Mock<IDeck>();
            var players = playerMocks.Select(m => m.Object).ToList();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[4]);

            var orderer = new PickPlayerOrdererInner(deckMock.Object);
            var actualList = orderer.PlayersInPickOrder;

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
            var ordererMock = new Mock<IPickPlayerOrdererInner>();
            var players = playerMocks.Select(m => m.Object).ToList();
            ordererMock.Setup(m => m.PlayersInPickOrder).Returns(players);
            ordererMock.Setup(m => m.PlayersRefusingPick).Returns(players.Skip(5).ToList());

            var orderer = new PickPlayerOrderer(ordererMock.Object);
            var actualList = orderer.PlayersWithoutPickTurn;

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
            var ordererMock = new Mock<IPickPlayerOrdererInner>();
            var players = playerMocks.Select(m => m.Object).ToList();
            ordererMock.Setup(m => m.PlayersInPickOrder).Returns(players);
            ordererMock.Setup(m => m.PlayersRefusingPick).Returns(players.Take(2).ToList());

            var orderer = new PickPlayerOrderer(ordererMock.Object);
            var actualList = orderer.PlayersWithoutPickTurn;

            Assert.AreSame(players[2], actualList[0]);
            Assert.AreSame(players[3], actualList[1]);
            Assert.AreSame(players[4], actualList[2]);
        }
    }
}
