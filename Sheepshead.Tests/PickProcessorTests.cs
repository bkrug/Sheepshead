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
    public class PickProcessorTests
    {
        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_EveryonePasses()
        {
            var playerMocks = new List<Mock>() { new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var computerPlayerMocks = playerMocks.OfType<Mock<IComputerPlayer>>().ToList();
            computerPlayerMocks.ForEach(m => m.Setup(p => p.WillPick(It.IsAny<IDeck>())).Returns(false));

            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var refusingPlayers = new List<IPlayer>() { new Mock<IComputerPlayer>().Object, new Mock<IHumanPlayer>().Object };
            var refusingPlayersOrig = refusingPlayers.ToList();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);
            var picker = pickProcessor.PlayNonHumanPickTurns();

            Assert.AreSame(null, picker);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayers[0], refusingPlayers[2]);
            Assert.AreSame(unplayedPlayers[1], refusingPlayers[3]);
            Assert.AreSame(unplayedPlayers[2], refusingPlayers[4]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_PlayTillHumanTurn()
        {
            var playerMocks = new List<Mock>() { new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IHumanPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var computerPlayerMocks = playerMocks.OfType<Mock<IComputerPlayer>>().ToList();
            computerPlayerMocks.ForEach(m => m.Setup(p => p.WillPick(It.IsAny<IDeck>())).Returns(false));

            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var refusingPlayers = new List<IPlayer>() { new Mock<IComputerPlayer>().Object, new Mock<IHumanPlayer>().Object };
            var refusingPlayersOrig = refusingPlayers.ToList();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);
            var picker = pickProcessor.PlayNonHumanPickTurns();

            Assert.AreSame(null, picker);
            Assert.AreEqual(4, refusingPlayers.Count);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayers[0], refusingPlayers[2]);
            Assert.AreSame(unplayedPlayers[1], refusingPlayers[3]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_FindPicker()
        {
            var playerMocks = new List<Mock>() { new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IHumanPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var computerPlayerMocks = playerMocks.OfType<Mock<IComputerPlayer>>().ToList();
            computerPlayerMocks[0].Setup(p => p.WillPick(It.IsAny<IDeck>())).Returns(false);
            computerPlayerMocks[1].Setup(p => p.WillPick(It.IsAny<IDeck>())).Returns(false);
            computerPlayerMocks[2].Setup(p => p.WillPick(It.IsAny<IDeck>())).Returns(true);

            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var refusingPlayers = new List<IPlayer>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);
            var picker = pickProcessor.PlayNonHumanPickTurns();

            Assert.AreSame(unplayedPlayers[2], picker);
            Assert.AreEqual(2, refusingPlayers.Count);
            Assert.AreSame(unplayedPlayers[0], refusingPlayers[0]);
            Assert.AreSame(unplayedPlayers[1], refusingPlayers[1]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_WrongPlayer()
        {
            var playerMocks = new List<Mock>() { new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IHumanPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();

            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var refusingPlayers = new List<IPlayer>() { new Mock<IHumanPlayer>().Object };
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);
            var threwException = false;
            try {
                var picker = pickProcessor.PlayNonHumanPickTurns();
            }
            catch(NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException, "Can't let computer pick unless computer has next turn.");
        }

        [TestMethod]
        public void PickProcessor_LetHumanPick_WillPick()
        {
            var playerMocks = new List<Mock>() { new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var humanCards = new List<ICard>();
            var humanMock = ((Mock<IHumanPlayer>)playerMocks[0]);
            humanMock.Setup(m => m.Cards).Returns(humanCards);

            var blinds = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            deckMock.Setup(m => m.Blinds).Returns(blinds);
            var refusingPlayers = new List<IPlayer>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);
            pickProcessor.LetHumanPick(humanMock.Object, true);

            Assert.IsTrue(humanCards.Contains(blinds[0]));
            Assert.IsTrue(humanCards.Contains(blinds[1]));
        }


        [TestMethod]
        public void PickProcessor_LetHumanPick_WillNotPick()
        {
            var playerMocks = new List<Mock>() { new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var humanCards = new List<ICard>();
            var humanMock = ((Mock<IHumanPlayer>)playerMocks[0]);

            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var refusingPlayers = new List<IPlayer>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);
            pickProcessor.LetHumanPick(humanMock.Object, false);

            Assert.AreSame(humanMock.Object, refusingPlayers.First());
            Assert.AreEqual(1, refusingPlayers.Count);
        }

        [TestMethod]
        public void PickProcessor_LetHumanPick_WrongPlayer()
        {
            var playerMocks = new List<Mock>() { new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IHumanPlayer>() };
            var unplayedPlayers = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var humanMock = ((Mock<IHumanPlayer>)playerMocks[2]);

            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var refusingPlayers = new List<IPlayer>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var pickProcessor = new PickProcessor(deckMock.Object, null);

            var threwException = false;
            try
            {
                pickProcessor.LetHumanPick(humanMock.Object, true);
            }
            catch (NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException, "Can't let computer pick unless computer has next turn.");
        }

        [TestMethod]
        public void PickProcessor_AcceptComputerPicker()
        {
            var deckMock = new Mock<IDeck>();
            var computerPlayerMock = new Mock<IComputerPlayer>();
            var buried = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object };
            computerPlayerMock.Setup(m => m.DropCardsForPick(deckMock.Object)).Returns(buried);
            var expectedHand = new Mock<IHand>().Object;
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(deckMock.Object, computerPlayerMock.Object, buried))
                .Returns(expectedHand);

            var pickProcessor = new PickProcessor(deckMock.Object, handFactoryMock.Object);
            var hand = pickProcessor.AcceptComputerPicker(computerPlayerMock.Object);

            Assert.AreSame(expectedHand, hand);
        }
    }
}
