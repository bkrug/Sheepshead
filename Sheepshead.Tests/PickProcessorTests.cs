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

        [TestMethod]
        public void PickProcessorOuter2_ContinueFromHumanPickTurn_HumanPicks()
        {
            var humanCards = new List<ICard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(humanCards);
            var pickProcessorMock = new Mock<IPickProcessorOuter>();
            pickProcessorMock
                .Setup(m => m.PlayNonHumanPickTurns(It.IsAny<IDeck>(), It.IsAny<IHandFactory>()))
                .Callback(() => Assert.Fail("Should not have let non humans pick."));
            var handMock = new Mock<IHand>();
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<ICard>>()))
                .Callback((IDeck deck, IPlayer player, List<ICard> cards) => handMock.Setup(m => m.Picker).Returns(humanMock.Object))
                .Returns(() => handMock.Object);
            var blinds = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(blinds);
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { humanMock.Object, new Mock<IComputerPlayer>().Object });

            var pickProcessor = new PickProcessorOuter2();
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
            var pickProcessorMock = new Mock<IPickProcessorOuter>();
            var handMock = new Mock<IHand>();
            pickProcessorMock
                .Setup(m => m.PlayNonHumanPickTurns(It.IsAny<IDeck>(), It.IsAny<IHandFactory>()))
                .Callback(() => handMock.Setup(m => m.Picker).Returns(expectedPicker))
                .Returns(expectedPicker);
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<ICard>>()))
                .Callback((IDeck deck, IPlayer player, List<ICard> cards) => Assert.Fail("Hand should not be created by ContinueFromHumanPick() method."));
            var refusingPlayers = new List<IPlayer>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Hand).Returns(handMock.Object);
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { humanMock.Object, expectedPicker });
            
            var pickProcessor = new PickProcessorOuter2();
            var hand = pickProcessor.ContinueFromHumanPickTurn(humanMock.Object, false, deckMock.Object, handFactoryMock.Object, pickProcessorMock.Object);

            Assert.IsTrue(hand.Picker is IComputerPlayer);
            Assert.AreSame(deckMock.Object.Hand, hand);
            Assert.IsTrue(refusingPlayers.Contains(humanMock.Object));
        }

        [TestMethod]
        public void PickProcessorOuter2_ContinueFromHumanPickTurn_WrongTurn()
        {
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(new List<ICard>());
            var computerPlayer = new Mock<IComputerPlayer>().Object;
            var pickProcessorMock = new Mock<IPickProcessorOuter>();
            var handFactoryMock = new Mock<IHandFactory>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(new List<ICard>());
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { computerPlayer, humanMock.Object });

            var pickProcessor = new PickProcessorOuter2();
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
            var toBury = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object };
            var playerCards = toBury.ToList();
            var buried = new List<ICard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(playerCards);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(buried);
            deckMock.Setup(m => m.Hand.Picker).Returns(humanMock.Object);

            new PickProcessorOuter().BuryCards(deckMock.Object, humanMock.Object, toBury);

            Assert.AreEqual(0, playerCards.Count(), "The buried cards were removed from the picker's hand.");
            Assert.IsTrue(buried.Contains(toBury[0]));
            Assert.IsTrue(buried.Contains(toBury[1]));
        }

        [TestMethod]
        public void PickProcessorOuter_BuryCards_NotPicker()
        {
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(new List<ICard>());
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(new List<ICard>());
            deckMock.Setup(m => m.Hand.Picker).Returns(new Mock<IComputerPlayer>().Object);

            var threwException = false;
            try {
                new PickProcessorOuter().BuryCards(deckMock.Object, humanMock.Object, new List<ICard>());
            }
            catch(NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException);
        }
    }
}
