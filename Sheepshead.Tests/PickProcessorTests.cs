using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Model;
using Sheepshead.Model.Models;
using Sheepshead.Model.Players;
using Sheepshead.Tests.PlayerMocks;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PickProcessorTests
    {
        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_EveryonePasses()
        {
            var handMock = new Mock<IHand>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            handMock.Setup(m => m.PlayerWontPick(It.IsAny<IPlayer>())).Callback((IPlayer p) => refusingPlayers.Add(p));
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            handMock.SetupGet(m => m.IGame.LeastersEnabled).Returns(true);

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();            
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(handMock.Object);

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
            var handMock = new Mock<IHand>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            handMock.Setup(m => m.PlayerWontPick(It.IsAny<IPlayer>())).Callback((IPlayer p) => refusingPlayers.Add(p));

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new Mock<IHumanPlayer>().Object
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(handMock.Object);

            Assert.IsNull(picker);
            Assert.AreEqual(4, refusingPlayers.Count);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayersOrig[0], refusingPlayers[2]);
            Assert.AreSame(unplayedPlayersOrig[1], refusingPlayers[3]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_NextPlayerIsHuman()
        {
            var handMock = new Mock<IHand>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(handMock.Object);

            Assert.IsNull(picker);
            Assert.AreEqual(2, refusingPlayers.Count);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_FindPicker()
        {
            var handMock = new Mock<IHand>();

            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            var refusingPlayersOrig = refusingPlayers.ToList();
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            handMock.Setup(m => m.PlayerWontPick(It.IsAny<IPlayer>())).Callback((IPlayer p) => refusingPlayers.Add(p));
            handMock.Setup(m => m.IGame.PartnerMethodEnum);

            var unplayedPlayersOrig = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(true),
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = unplayedPlayersOrig.ToList();
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(handMock.Object);

            Assert.AreSame(unplayedPlayersOrig[1], picker);
            Assert.AreEqual(3, refusingPlayers.Count);
            Assert.AreSame(refusingPlayersOrig[0], refusingPlayers[0]);
            Assert.AreSame(refusingPlayersOrig[1], refusingPlayers[1]);
            Assert.AreSame(unplayedPlayersOrig[0], refusingPlayers[2]);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_CalledAce_FindPicker_PickPartner()
        {
            SheepCard? _actualPartnerCard = null;
            var handMock = new Mock<IHand>();
            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            handMock.SetupGet(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            var unplayedPlayers = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(true, SheepCard.ACE_HEARTS),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            handMock.Setup(m => m.SetPartnerCard(It.IsAny<SheepCard?>())).Callback((SheepCard? c) => _actualPartnerCard = c);
            var picker = new PickProcessor().PlayNonHumanPickTurns(handMock.Object);

            Assert.AreEqual(SheepCard.ACE_HEARTS, _actualPartnerCard.Value);
        }

        [TestMethod]
        public void PickProcessor_PlayNonHumanPickTurns_CalledAce_FindPicker_NoPartner()
        {
            SheepCard? _actualPartnerCard = SheepCard.N7_HEARTS;
            var handMock = new Mock<IHand>();
            var refusingPlayers = new List<IPlayer>() {
                new Mock<IComputerPlayer>().Object,
                new Mock<IHumanPlayer>().Object
            };
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            handMock.SetupGet(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            var unplayedPlayers = new List<IPlayer>()
            {
                new ComputerPlayerPickingMock(true, null),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);

            handMock.Setup(m => m.SetPartnerCard(It.IsAny<SheepCard?>())).Callback((SheepCard? c) => _actualPartnerCard = c);
            var picker = new PickProcessor().PlayNonHumanPickTurns(handMock.Object);

            Assert.IsNull(_actualPartnerCard);
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
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Players).Returns(players);
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(playersWithoutTurn);
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(new List<IPlayer>());
            handMock.Setup(m => m.IGame.PartnerMethodEnum);

            var pickProcessor = new PickProcessor();
            var picker = pickProcessor.PlayNonHumanPickTurns(handMock.Object);

            Assert.AreSame(players[2], picker);
        }

        [TestMethod]
        public void PickProcessor_ContinueFromHumanPickTurn_HumanPicks()
        {
            var humanCards = new List<SheepCard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(humanCards);
            humanMock.Setup(f => f.AddCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => humanCards.Add(c));
            humanMock.Setup(f => f.RemoveCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => humanCards.Remove(c));
            var pickProcessorMock = new Mock<IPickProcessor>();
            pickProcessorMock
                .Setup(m => m.PlayNonHumanPickTurns(It.IsAny<IHand>()))
                .Callback(() => Assert.Fail("Should not have let non humans pick."));
            var blinds = new List<SheepCard>() { 0, (SheepCard)1 };
            IPlayer actualPicker = null;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.SetPicker(It.IsAny<IPlayer>(), It.IsAny<List<SheepCard>>())).Callback<IPlayer, List<SheepCard>>((p, c) => actualPicker = p);
            handMock.Setup(m => m.Blinds).Returns(blinds);
            handMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { humanMock.Object, new Mock<IComputerPlayer>().Object });

            var pickProcessor = new PickProcessor();
            var hand = pickProcessor.ContinueFromHumanPickTurn(humanMock.Object, true, handMock.Object, pickProcessorMock.Object);

            Assert.AreSame(humanMock.Object, actualPicker);
            Assert.IsTrue(humanCards.Contains(blinds[0]));
            Assert.IsTrue(humanCards.Contains(blinds[1]));
        }

        [TestMethod]
        public void PickProcessor_ContinueFromHumanPickTurn_HumanDeclinesButCompterPicks()
        {
            var humanMock = (new Participant() { Type = "H" }).Player as IHumanPlayer;
            var expectedPicker = (new Participant() { Type = "S" }).Player as IComputerPlayer;
            var refusingPlayers = new List<IPlayer>();
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPlayers);
            handMock.Setup(m => m.PlayerWontPick(It.IsAny<IPlayer>())).Callback((IPlayer p) => refusingPlayers.Add(p));
            handMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { humanMock, expectedPicker });
            var pickProcessorMock = new Mock<IPickProcessor>();
            var nonHumansGotToPlay = false;
            pickProcessorMock
                .Setup(m => m.PlayNonHumanPickTurns(It.IsAny<IHand>()))
                .Callback(() => {
                    handMock.Setup(m => m.Picker).Returns(expectedPicker);
                    nonHumansGotToPlay = true;
                })
                .Returns(expectedPicker);
            
            var pickProcessor = new PickProcessor();
            pickProcessor.ContinueFromHumanPickTurn(humanMock, false, handMock.Object, pickProcessorMock.Object);

            Assert.IsTrue(refusingPlayers.Contains(humanMock));
            Assert.IsTrue(nonHumansGotToPlay);
        }

        [TestMethod]
        public void PickProcessor_ContinueFromHumanPickTurn_WrongTurn()
        {
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var computerPlayer = new Mock<IComputerPlayer>().Object;
            var pickProcessorMock = new Mock<IPickProcessor>();
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>());
            handMock.Setup(m => m.PlayersWithoutPickTurn).Returns(new List<IPlayer>() { computerPlayer, humanMock.Object });

            var pickProcessor = new PickProcessor();
            var threwException = false;
            try
            {
                var hand = pickProcessor.ContinueFromHumanPickTurn(humanMock.Object, true, handMock.Object, pickProcessorMock.Object);
            }
            catch (NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void PickProcessor_BuryCards()
        {
            var toBury = new List<SheepCard>() { 0, (SheepCard)1 };
            var playerCards = toBury.ToList();
            var buried = new List<SheepCard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(playerCards);
            humanMock.Setup(f => f.AddCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => playerCards.Add(c));
            humanMock.Setup(f => f.RemoveCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => playerCards.Remove(c));
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.AddBuried(It.IsAny<SheepCard>())).Callback((SheepCard sc) => buried.Add(sc));
            handMock.Setup(m => m.Buried).Returns(buried);
            handMock.Setup(m => m.Picker).Returns(humanMock.Object);
            handMock.Setup(m => m.GoItAlone()).Callback(() => Assert.Fail("Should not have tried to go it alone"));

            new PickProcessor().BuryCards(handMock.Object, humanMock.Object, toBury, false);

            Assert.AreEqual(0, playerCards.Count(), "The buried cards were removed from the picker's hand.");
            Assert.IsTrue(buried.Contains(toBury[0]));
            Assert.IsTrue(buried.Contains(toBury[1]));
        }

        [TestMethod]
        public void PickProcessor_BuryCards_GoItAlone()
        {
            var toBury = new List<SheepCard>() { 0, (SheepCard)1 };
            var playerCards = toBury.ToList();
            var buried = new List<SheepCard>();
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(playerCards);
            humanMock.Setup(f => f.AddCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => playerCards.Add(c));
            humanMock.Setup(f => f.RemoveCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => playerCards.Remove(c));
            var goItAloneSet = false;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.AddBuried(It.IsAny<SheepCard>())).Callback((SheepCard sc) => buried.Add(sc));
            handMock.Setup(m => m.Buried).Returns(buried);
            handMock.Setup(m => m.Picker).Returns(humanMock.Object);
            handMock.Setup(m => m.GoItAlone()).Callback(() => goItAloneSet = true);

            new PickProcessor().BuryCards(handMock.Object, humanMock.Object, toBury, true);

            Assert.AreEqual(0, playerCards.Count(), "The buried cards were removed from the picker's hand.");
            Assert.IsTrue(buried.Contains(toBury[0]));
            Assert.IsTrue(buried.Contains(toBury[1]));
            Assert.IsTrue(goItAloneSet);
        }

        [TestMethod]
        public void PickProcessor_BuryCards_NotPicker()
        {
            var humanMock = new Mock<IHumanPlayer>();
            humanMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            handMock.Setup(m => m.Picker).Returns(new Mock<IComputerPlayer>().Object);

            var threwException = false;
            try {
                new PickProcessor().BuryCards(handMock.Object, humanMock.Object, new List<SheepCard>(), false);
            }
            catch(NotPlayersTurnException)
            {
                threwException = true;
            }
            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void PickProcessor_BuryCards_CalledAce_WithinRules()
        {
            var pickerMock = new Mock<IHumanPlayer>();
            SheepCard? innerPartnerCard = SheepCard.ACE_CLUBS;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PartnerCard).Returns(() => innerPartnerCard);
            handMock.Setup(m => m.SetPartnerCard(It.IsAny<SheepCard?>())).Callback((SheepCard? sc) => innerPartnerCard = sc);
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            var pickerCards = new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.KING_DIAMONDS, SheepCard.QUEEN_CLUBS, SheepCard.N7_HEARTS, SheepCard.N10_SPADES, SheepCard.QUEEN_HEARTS, SheepCard.JACK_HEARTS, SheepCard.N9_DIAMONDS };
            pickerMock.Setup(m => m.Cards).Returns(pickerCards);
            pickerMock.Setup(f => f.AddCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => pickerCards.Add(c));
            pickerMock.Setup(f => f.RemoveCard(It.IsAny<SheepCard>())).Callback((SheepCard c) => pickerCards.Remove(c));
            var cardsToBury = new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.N10_SPADES };
            var partnerCard = SheepCard.ACE_HEARTS;

            new PickProcessor().BuryCards(handMock.Object, pickerMock.Object, cardsToBury, false, partnerCard);

            var expectedCards = pickerCards.Except(cardsToBury).ToList();
            CollectionAssert.AreEquivalent(expectedCards, pickerCards);
            Assert.AreEqual(SheepCard.ACE_HEARTS, handMock.Object.PartnerCard);
        }

        [TestMethod]
        public void PickProcessor_BuryCards_CalledAce_PickerHasCard()
        {
            var pickerMock = new Mock<IHumanPlayer>();
            SheepCard? innerPartnerCard = SheepCard.ACE_CLUBS;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PartnerCard).Returns(() => innerPartnerCard);
            handMock.Setup(m => m.SetPartnerCard(It.IsAny<SheepCard?>())).Callback((SheepCard? sc) => innerPartnerCard = sc);
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            var pickerCards = new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.KING_DIAMONDS, SheepCard.QUEEN_CLUBS, SheepCard.N7_HEARTS, SheepCard.N10_SPADES, SheepCard.QUEEN_HEARTS, SheepCard.JACK_HEARTS, SheepCard.N9_DIAMONDS };
            pickerMock.Setup(m => m.Cards).Returns(pickerCards);
            var cardsToBury = new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.N10_SPADES };
            var partnerCard = SheepCard.ACE_CLUBS;

            try
            {
                new PickProcessor().BuryCards(handMock.Object, pickerMock.Object, cardsToBury, false, partnerCard);
                Assert.Fail("Should have thrown an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Picker has the parner card"));
            }
        }

        [TestMethod]
        public void PickProcessor_BuryCards_CalledAce_PickerDoesNotHaveSuit()
        {
            var pickerMock = new Mock<IHumanPlayer>();
            SheepCard? innerPartnerCard = SheepCard.ACE_CLUBS;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PartnerCard).Returns(() => innerPartnerCard);
            handMock.Setup(m => m.SetPartnerCard(It.IsAny<SheepCard?>())).Callback((SheepCard? sc) => innerPartnerCard = sc);
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            var pickerCards = new List<SheepCard>() { SheepCard.KING_SPADES, SheepCard.KING_DIAMONDS, SheepCard.QUEEN_CLUBS, SheepCard.N7_HEARTS, SheepCard.N10_SPADES, SheepCard.QUEEN_HEARTS, SheepCard.JACK_HEARTS, SheepCard.N9_DIAMONDS };
            pickerMock.Setup(m => m.Cards).Returns(pickerCards);
            var cardsToBury = new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.N10_SPADES };
            var partnerCard = SheepCard.ACE_CLUBS;

            try
            {
                new PickProcessor().BuryCards(handMock.Object, pickerMock.Object, cardsToBury, false, partnerCard);
                Assert.Fail("Should have thrown an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Picker does not have a card in the CLUBS"));
            }
        }

        [TestMethod]
        public void PickProcessor_BuryCards_CalledAce_InvalidCard()
        {
            var pickerMock = new Mock<IHumanPlayer>();
            SheepCard? innerPartnerCard = SheepCard.ACE_CLUBS;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PartnerCard).Returns(() => innerPartnerCard);
            handMock.Setup(m => m.SetPartnerCard(It.IsAny<SheepCard?>())).Callback((SheepCard? sc) => innerPartnerCard = sc);
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            var pickerCards = new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.KING_DIAMONDS, SheepCard.QUEEN_CLUBS, SheepCard.N7_HEARTS, SheepCard.N10_SPADES, SheepCard.QUEEN_HEARTS, SheepCard.JACK_HEARTS, SheepCard.N9_DIAMONDS };
            pickerMock.Setup(m => m.Cards).Returns(pickerCards);
            var cardsToBury = new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.N10_SPADES };
            var partnerCard = SheepCard.N8_CLUBS;

            try
            {
                new PickProcessor().BuryCards(handMock.Object, pickerMock.Object, cardsToBury, false, partnerCard);
                Assert.Fail("Should have thrown an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("is not a valid partner card"));
            }
        }
    }
}
