using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Tests.PlayerMocks;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PickProcessorTests
    {
        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_EveryonePasses()
        {
            var deckMock = new Mock<IDeck>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            deckMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();            
            deckMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(deckMock.Object, new Mock<IHandFactory>().Object);

            Assert.IsNull(picker);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayersOrig[0], refusingPlayers[2]);
            Assert.AreSame(unplayedPlayersOrig[1], refusingPlayers[3]);
            Assert.AreSame(unplayedPlayersOrig[2], refusingPlayers[4]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_PlayTillHumanTurn()
        {
            var deckMock = new Mock<IDeck>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            deckMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new Mock<IHumanPlayer>().Object
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();
            deckMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(deckMock.Object, null);

            Assert.IsNull(picker);
            Assert.AreEqual(4, refusingPlayers.Count);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayersOrig[0], refusingPlayers[2]);
            Assert.AreSame(unplayedPlayersOrig[1], refusingPlayers[3]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_FindPicker()
        {
            var deckMock = new Mock<IDeck>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            deckMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(true),
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();
            deckMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(deckMock.Object, new Mock<IHandFactory>().Object);

            Assert.AreSame(unplayedPlayersOrig[1], picker);
            Assert.AreEqual(3, refusingPlayers.Count);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayersOrig[0], refusingPlayers[2]);
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

            var pickProcessor = new PickProcessor();
            var threwException = false;
            try {
                var picker = pickProcessor.PlayNonHumanPickTurns(deckMock.Object, null);
            }
            catch(NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException, "Can't let computer pick unless computer has next turn.");
        }

        [TestMethod]
        public void PickProcessor_AcceptComputerPicker()
        {
            var players = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(true),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
            };
            var playersWithoutTurn = players.ToList();
            var expectedPicker = players[2];
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(playersWithoutTurn);
            deckMock.SetupGet(m => m.PlayersRefusingPick).Returns(new List<IPlayer>());

            var handCreated = false;
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(deckMock.Object, expectedPicker, It.IsAny<List<SheepCard>>()))
                .Callback(() => { handCreated = true; });

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(deckMock.Object, handFactoryMock.Object);

            Assert.IsTrue(handCreated, "Hand was created for the expected picker.");
        }

        [TestMethod]
        public void PickProcessorOuter2_ContinueFromHumanPickTurn_HumanPicks()
        {
            var humanCards = new List<SheepCard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(humanCards);
            var pickProcessorMock = new Mock<IPickProcessor>();
            pickProcessorMock
                .Setup(m => m.PlayNonHumanPickTurns(It.IsAny<IDeck>(), It.IsAny<IHandFactory>()))
                .Callback(() => Assert.Fail("Should not have let non humans pick."));
            var handMock = new Mock<IHand>();
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<SheepCard>>()))
                .Callback((IDeck deck, IPlayer player, List<SheepCard> cards) => handMock.Setup(m => m.Picker).Returns(humanMock.Object))
                .Returns(() => handMock.Object);
            var blinds = new List<SheepCard>() { 0, (SheepCard)1 };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(blinds);
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { humanMock.Object, new Mock<IComputerPlayer>().Object });

            var pickProcessor = new PickProcessor();
            var hand = pickProcessor.ContinueFromHumanPickTurn(humanMock.Object, true, deckMock.Object, handFactoryMock.Object, pickProcessorMock.Object);

            Assert.AreSame(humanMock.Object, hand.Picker);
            Assert.IsTrue(humanCards.Contains(blinds[0]));
            Assert.IsTrue(humanCards.Contains(blinds[1]));
        }

        [TestMethod]
        public void PickProcessorOuter2_ContinueFromHumanPickTurn_HumanDeclinesButCompterPicks()
        {
            var humanMock = new Mock<IHumanPlayer>();
            var expectedPicker = new Mock<IComputerPlayer>().Object;
            var pickProcessorMock = new Mock<IPickProcessor>();
            var handMock = new Mock<IHand>();
            pickProcessorMock
                .Setup(m => m.PlayNonHumanPickTurns(It.IsAny<IDeck>(), It.IsAny<IHandFactory>()))
                .Callback(() => handMock.Setup(m => m.Picker).Returns(expectedPicker))
                .Returns(expectedPicker);
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<SheepCard>>()))
                .Callback((IDeck deck, IPlayer player, List<SheepCard> cards) => Assert.Fail("Hand should not be created by ContinueFromHumanPick() method."));
            var refusingPlayers = new List<IPlayer>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Hand).Returns(handMock.Object);
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { humanMock.Object, expectedPicker });
            
            var pickProcessor = new PickProcessor();
            var hand = pickProcessor.ContinueFromHumanPickTurn(humanMock.Object, false, deckMock.Object, handFactoryMock.Object, pickProcessorMock.Object);

            Assert.IsTrue(hand.Picker is IComputerPlayer);
            Assert.AreSame(deckMock.Object.Hand, hand);
            Assert.IsTrue(refusingPlayers.Contains(humanMock.Object));
        }

        [TestMethod]
        public void PickProcessorOuter2_ContinueFromHumanPickTurn_WrongTurn()
        {
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var computerPlayer = new Mock<IComputerPlayer>().Object;
            var pickProcessorMock = new Mock<IPickProcessor>();
            var handFactoryMock = new Mock<IHandFactory>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { computerPlayer, humanMock.Object });

            var pickProcessor = new PickProcessor();
            var threwException = false;
            try
            {
                var hand = pickProcessor.ContinueFromHumanPickTurn(humanMock.Object, true, deckMock.Object, handFactoryMock.Object, pickProcessorMock.Object);
            }
            catch (NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void PickProcessorOuter_BuryCards()
        {
            var toBury = new List<SheepCard>() { 0, (SheepCard)1 };
            var playerCards = toBury.ToList();
            var buried = new List<SheepCard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(playerCards);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(buried);
            deckMock.Setup(m => m.Hand.Picker).Returns(humanMock.Object);

            new PickProcessor().BuryCards(deckMock.Object, humanMock.Object, toBury);

            Assert.AreEqual(0, playerCards.Count(), "The buried cards were removed from the picker's hand.");
            Assert.IsTrue(buried.Contains(toBury[0]));
            Assert.IsTrue(buried.Contains(toBury[1]));
        }

        [TestMethod]
        public void PickProcessorOuter_BuryCards_NotPicker()
        {
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Hand.Picker).Returns(new Mock<IComputerPlayer>().Object);

            var threwException = false;
            try {
                new PickProcessor().BuryCards(deckMock.Object, humanMock.Object, new List<SheepCard>());
            }
            catch(NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException);
        }
    }
}
