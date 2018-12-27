using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Logic;
using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class TrickTests
    {
        private IHand GetHand()
        {
            var handMock = new Mock<IHand>();
            var trickList = new List<ITrick>();
            handMock.Setup(m => m.ITricks).Returns(trickList);
            handMock.Setup(m => m.PartnerCardEnum).Returns(SheepCard.KING_CLUBS);
            handMock.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            handMock.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.JackOfDiamonds);
            return handMock.Object;
        }

        private IPlayer GetPlayer(List<SheepCard> hand)
        {
            var participant = new Participant();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(hand);
            playerMock.Setup(m => m.Participant).Returns(participant);
            return playerMock.Object;
        }

        [TestMethod]
        public void Trick_IsLegal_MustMatchStartingCard()
        {
            var firstPlayer = new Participant() { Cards = CardUtil.GetAbbreviation(SheepCard.N9_HEARTS) }.Player;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(firstPlayer);
            var player = new Participant() { Cards = "K♥;7♥;Q♦;8♣" }.Player;
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.ITricks).Returns(new List<ITrick>());
            handMock.Setup(m => m.Players).Returns(new List<IPlayer>() { firstPlayer, player });
            handMock.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.JackOfDiamonds);
            handMock.Setup(m => m.PartnerCardEnum).Returns(SheepCard.JACK_DIAMONDS);
            var trick = new Trick(handMock.Object, startingPlayerCalcMock.Object);
            trick.Add(firstPlayer, SheepCard.N9_HEARTS);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N7_HEARTS, player), "A hearts is part of the same suite.");
            Assert.IsFalse(trick.IsLegalAddition(SheepCard.N8_CLUBS, player), "A clubs is not part of the same suite.");
            Assert.IsFalse(trick.IsLegalAddition(SheepCard.QUEEN_DIAMONDS, player), "A trump is not part of the same suite.");
            Assert.IsFalse(trick.IsLegalAddition(SheepCard.N10_CLUBS, player), "A card outside of the hand is not legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_PlayerDoesNotHaveSuit()
        {
            var firstPlayerMock = new Mock<IPlayer>();
            var firstPlayer = firstPlayerMock.Object;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(firstPlayer);
            firstPlayerMock.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N9_SPADES });
            firstPlayerMock.Setup(m => m.Participant).Returns(new Participant());
            var hand = new List<SheepCard>() {
                SheepCard.KING_HEARTS, SheepCard.N7_HEARTS, SheepCard.QUEEN_CLUBS, SheepCard.N8_CLUBS
            };
            var player = GetPlayer(hand);
            var trick = new Trick(GetHand(), startingPlayerCalcMock.Object);
            trick.Add(firstPlayer, SheepCard.N9_SPADES);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N7_HEARTS, player), "There is no spades in the hand. Hearts is fine.");
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N8_CLUBS, player), "There is no spades in the hand. Clubs is fine.");
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.QUEEN_CLUBS, player), "There is no spades in the hand. Trump is fine.");
        }

        [TestMethod]
        public void Trick_IsLegal_FirstCardInSuit()
        {
            var firstPlayer = new Participant().Player;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(firstPlayer);
            var handMock = new Mock<IHand>();
            handMock.Setup(h => h.Players).Returns(new List<IPlayer>() { firstPlayer });
            handMock.Setup(h => h.ITricks).Returns(new List<ITrick>());
            handMock.Setup(h => h.IGame.PartnerMethodEnum).Returns(PartnerMethod.JackOfDiamonds);
            handMock.Setup(h => h.PartnerCardEnum).Returns(SheepCard.JACK_DIAMONDS);
            var trick = new Trick(handMock.Object, startingPlayerCalcMock.Object);
            var hand = new List<SheepCard>() {
                SheepCard.KING_HEARTS, SheepCard.N7_HEARTS, SheepCard.QUEEN_CLUBS, SheepCard.N8_CLUBS
            };
            var player = GetPlayer(hand);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N7_HEARTS, player), "Adding a card to an empty trick is always okay.");
        }

        [TestMethod]
        public void Trick_IsLegal_PickerCannotLeadWithLastCardOfCalledSuit()
        {
            var picker = new Mock<IPlayer>();
            picker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N9_HEARTS, SheepCard.N9_DIAMONDS, SheepCard.QUEEN_HEARTS, SheepCard.N9_CLUBS, SheepCard.ACE_CLUBS, SheepCard.KING_CLUBS });
            var hand = new Mock<IHand>();
            hand.Setup(m => m.Buried).Returns(new List<SheepCard>() { SheepCard.N10_CLUBS, SheepCard.N10_SPADES });
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            hand.Setup(m => m.Picker).Returns(picker.Object);
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(picker.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsFalse(trick.IsLegalAddition(SheepCard.N9_HEARTS, picker.Object), "Picker has no other hearts, so this cannot be the lead card.");
        }

        [TestMethod]
        public void Trick_IsLegal_PickerCanLeadWithCardOfCalledSuit()
        {
            var hand = new Mock<IHand>();
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            var picker = new Mock<IPlayer>();
            picker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N9_DIAMONDS, SheepCard.QUEEN_HEARTS, SheepCard.N9_HEARTS, SheepCard.N9_CLUBS, SheepCard.ACE_CLUBS, SheepCard.KING_CLUBS });
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(picker.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N9_HEARTS, picker.Object), "Picker has a remaining heart, so this is legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_PickerCanLeadWithCardOfCalledSuit1()
        {
            var hand = new Mock<IHand>();
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            var picker = new Mock<IPlayer>();
            picker.Setup(m => m.Participant).Returns(new Participant());
            picker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N9_DIAMONDS, SheepCard.QUEEN_HEARTS, SheepCard.N8_HEARTS, SheepCard.ACE_CLUBS, SheepCard.KING_CLUBS, SheepCard.N9_HEARTS, });
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(picker.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N9_HEARTS, picker.Object), "Picker has a remaining heart, so this is legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_PickerCanFollowWithLastCardOfCalledSuit()
        {
            var startingPlayer = new Mock<IPlayer>();
            startingPlayer.Setup(m => m.Cards).Returns(new List<SheepCard>());
            startingPlayer.Setup(m => m.Participant).Returns(new Participant());
            var hand = new Mock<IHand>();
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            var picker = new Mock<IPlayer>();
            picker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N9_DIAMONDS, SheepCard.QUEEN_HEARTS, SheepCard.N9_HEARTS, SheepCard.ACE_CLUBS, SheepCard.KING_CLUBS, SheepCard.N7_SPADES });
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(startingPlayer.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            trick.Add(startingPlayer.Object, SheepCard.N10_HEARTS);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N9_HEARTS, picker.Object), "Picker has a remaining heart, so this is legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_PickerLeads_OtherTrickLedWithSameSuit()
        {
            var picker = new Mock<IPlayer>();
            picker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N9_HEARTS, SheepCard.N9_DIAMONDS, SheepCard.QUEEN_HEARTS, SheepCard.N9_CLUBS, SheepCard.ACE_CLUBS, SheepCard.KING_CLUBS });
            var previousTrick = new Mock<ITrick>();
            previousTrick.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>() { { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS } });
            var hand = new Mock<IHand>();
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            hand.Setup(m => m.Picker).Returns(picker.Object);
            hand.Setup(m => m.ITricks).Returns(new List<ITrick>() { previousTrick.Object });
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(picker.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N9_HEARTS, picker.Object), "Picker has no other hearts, but hearts were led in a previous trick.");
        }

        [TestMethod]
        public void Trick_IsLegal_PartnerCannotLeadWithPartnerCard()
        {
            var partner = new Mock<IPlayer>();
            partner.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_HEARTS, SheepCard.ACE_HEARTS, SheepCard.JACK_DIAMONDS, SheepCard.N7_SPADES, SheepCard.ACE_SPADES });
            var hand = new Mock<IHand>();
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(partner.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsFalse(trick.IsLegalAddition(SheepCard.ACE_HEARTS, partner.Object), "Picker has a remaining heart, so this is legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_PartnerCanLeadWithOtherCards()
        {
            var partner = new Mock<IPlayer>();
            partner.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_HEARTS, SheepCard.ACE_HEARTS, SheepCard.JACK_DIAMONDS, SheepCard.N7_SPADES, SheepCard.ACE_SPADES });
            var hand = new Mock<IHand>();
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(partner.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N10_HEARTS, partner.Object), "Picker has a remaining heart, so this is legal.");
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N7_HEARTS, partner.Object), "Picker has a remaining heart, so this is legal.");
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.JACK_DIAMONDS, partner.Object), "Picker has a remaining heart, so this is legal.");
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.ACE_SPADES, partner.Object), "Picker has a remaining heart, so this is legal.");
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.N7_SPADES, partner.Object), "Picker has a remaining heart, so this is legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_PartnerLeads_OtherTrickLeadWithSameSuit()
        {
            var partner = new Mock<IPlayer>();
            partner.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_HEARTS, SheepCard.ACE_HEARTS, SheepCard.JACK_DIAMONDS, SheepCard.N7_SPADES, SheepCard.ACE_SPADES });
            var previousTrick = new Mock<ITrick>();
            previousTrick.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>() { { new Mock<IPlayer>().Object, SheepCard.N7_DIAMONDS } });
            previousTrick.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>() { { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS } });
            var hand = new Mock<IHand>();
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_HEARTS);
            hand.Setup(m => m.ITricks).Returns(new List<ITrick>() { previousTrick.Object });
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(partner.Object);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.ACE_HEARTS, partner.Object), "Previous trick lead with heart, so this is legal.");
        }

        [TestMethod]
        public void Trick_IsLegal_LastCardLeft()
        {
            var partner = new Participant() { Cards = CardUtil.GetAbbreviation(SheepCard.ACE_SPADES) }.Player;
            var otherTrick = new Mock<ITrick>().Object;
            var hand = new Mock<IHand>();
            hand.Setup(m => m.ITricks).Returns(new List<ITrick>() { otherTrick, otherTrick, otherTrick, otherTrick });
            hand.Setup(m => m.IGame.PartnerMethodEnum).Returns(PartnerMethod.CalledAce);
            hand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.ACE_SPADES);
            var calculator = new Mock<IStartingPlayerCalculator>();
            calculator.Setup(m => m.GetStartingPlayer(hand.Object, It.IsAny<ITrick>())).Returns(partner);
            var trick = new Trick(hand.Object, calculator.Object);
            Assert.IsTrue(trick.IsLegalAddition(SheepCard.ACE_SPADES, partner), "Cannt normally lead with the partner card, but can do so for last trick.");
        }

        [TestMethod]
        public void Trick_Winner()
        {
            var player1 = new Participant().Player;
            var player2 = new Participant().Player;
            var player3 = new Participant().Player;
            var player4 = new Participant().Player;
            var player5 = new Participant().Player;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(player1);
            {
                var trick = new Trick(GetHand(), startingPlayerCalcMock.Object);
                trick.Add(player1, SheepCard.N8_HEARTS);
                trick.Add(player2, SheepCard.ACE_SPADES);
                trick.Add(player3, SheepCard.N10_HEARTS);
                trick.Add(player4, SheepCard.KING_HEARTS);
                trick.Add(player5, SheepCard.ACE_CLUBS);
                var winner = trick.Winner();
                Assert.AreEqual(player3, winner.Player, "Ten of hearts has the hightest rank of the correct suite.");
                Assert.AreEqual(36, winner.Points, "Expected points for 2 Aces, 1 King, 1 Ten.");
            }
            {
                var trick = new Trick(GetHand(), startingPlayerCalcMock.Object);
                trick.Add(player1, SheepCard.N8_HEARTS);
                trick.Add(player2, SheepCard.N8_DIAMONDS);
                trick.Add(player3, SheepCard.N10_HEARTS);
                trick.Add(player4, SheepCard.KING_HEARTS);
                trick.Add(player5, SheepCard.ACE_CLUBS);
                var winner = trick.Winner();
                Assert.AreEqual(player2, winner.Player, "Trump has the highest rank.");
                Assert.AreEqual(25, winner.Points, "Expected points for 1 Aces, 1 Ten, 1 King.");
            }
            {
                var trick = new Trick(GetHand(), startingPlayerCalcMock.Object);
                trick.Add(player1, SheepCard.N8_DIAMONDS);
                trick.Add(player2, SheepCard.ACE_SPADES);
                trick.Add(player3, SheepCard.N10_HEARTS);
                trick.Add(player4, SheepCard.KING_DIAMONDS);
                trick.Add(player5, SheepCard.ACE_CLUBS);
                var winner = trick.Winner();
                Assert.AreEqual(player4, winner.Player, "Trump has the highest rank when it is played first, too.");
                Assert.AreEqual(36, winner.Points, "Expected points for 2 Aces, 1 King, 1 Ten.");
            }
        }

        [TestMethod]
        public void Trick_SetPartner()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(firstPlayer);
            var trickList = new List<ITrick>();
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.QUEEN_DIAMONDS);
            mockHand.Setup(m => m.PlayerCount).Returns(5);
            mockHand.Setup(m => m.Players).Returns(new List<IPlayer>());
            mockHand.Setup(m => m.ITricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            var hand = mockHand.Object;
            var trick = new Trick(hand, startingPlayerCalcMock.Object);
            var player = new Mock<IPlayer>();
            player.Setup(c => c.Name).Returns("DesiredPlayer");
            player.Setup(c => c.Cards).Returns(new List<SheepCard>() { SheepCard.QUEEN_DIAMONDS});
            player.Setup(c => c.Participant).Returns(new Participant());
            IPlayer partner = player.Object;
            var _methodCalled = false;
            mockHand.Setup(f => f.SetPartner(It.IsAny<IPlayer>(), It.IsAny<ITrick>())).Callback((IPlayer pl, ITrick tr) => {
                _methodCalled = true;
                Assert.AreEqual(partner, pl);
                Assert.AreEqual(trick, tr);
            });
            trick.Add(player.Object, SheepCard.QUEEN_DIAMONDS);
            Assert.IsTrue(_methodCalled, "When someone adds the partner card to the trick, the hand's partner get's specified.");
        }

        [TestMethod]
        public void Trick_AddToHand()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(firstPlayer);
            var mockHand = new Mock<IHand>();
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.ITricks).Returns(trickList);
            ITrick passedTrick = null;
            var trickMock = new Mock<ITrick>();
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick givenTrick) =>
            {
                trickList.Add(givenTrick);
                passedTrick = givenTrick;
            });
            var trick = new Trick(mockHand.Object, startingPlayerCalcMock.Object);
            Assert.AreSame(trick, passedTrick, "When a trick is instantiated, it should be added to a given hand.");
        }

        [TestMethod]
        public void Trick_IsComplete()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var startingPlayerCalcMock = new Mock<IStartingPlayerCalculator>();
            startingPlayerCalcMock.Setup(m => m.GetStartingPlayer(It.IsAny<IHand>(), It.IsAny<ITrick>())).Returns(firstPlayer);
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.Players).Returns(new List<IPlayer>());
            mockHand.Setup(m => m.PartnerCardEnum).Returns(SheepCard.N10_DIAMONDS);
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.ITricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            foreach (var playerCount in new[] { 3, 5 })
            {
                mockHand.Setup(m => m.PlayerCount).Returns(playerCount);
                var trick = new Trick(mockHand.Object, startingPlayerCalcMock.Object);
                for (var cardsInTrick = 0; cardsInTrick < playerCount; ++cardsInTrick)
                {
                    Assert.IsFalse(trick.IsComplete(), "Trick should not be complete when there are " + cardsInTrick + " cards in the trick and " + playerCount + " players in the game.");
                    trick.Add(new MockPlayer(), 0);
                }
                Assert.IsTrue(trick.IsComplete(), "Trick should be complete when there are " + playerCount + " cards in the trick and " + playerCount + " players in the game.");
            }
        }

        [TestMethod]
        public void Trick_StartingPlayer_FirstTrick()
        {
            var player1 = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var mockNewTrick = new Mock<ITrick>();
            var trickList = new List<ITrick>() { mockNewTrick.Object };
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.StartingPlayer).Returns(player1.Object);
            mockHand.Setup(m => m.ITricks).Returns(trickList);
            var startingPlayer = new StartingPlayerCalculator().GetStartingPlayer(mockHand.Object, mockNewTrick.Object);
            Assert.AreEqual(player1.Object, startingPlayer, "The starting player for first trick should be the same as the starting player for the hand.");
        }

        [TestMethod]
        public void Trick_StartingPlayer_SecondTrick()
        {
            var player1 = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var mockNewTrick = new Mock<ITrick>();
            var mockCompleteTrick = new Mock<ITrick>();
            mockCompleteTrick.Setup(m => m.Winner()).Returns(new TrickWinner() { Player = player4.Object, Points = 94 });
            var trickList = new List<ITrick>() { mockCompleteTrick.Object, mockNewTrick.Object };
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.StartingPlayer).Returns(player1.Object);
            mockHand.Setup(m => m.ITricks).Returns(trickList);
            var startingPlayer = new StartingPlayerCalculator().GetStartingPlayer(mockHand.Object, mockNewTrick.Object);
            Assert.AreEqual(player4.Object, startingPlayer, "The starting player for the second trick should be the winner of the previous trick.");
        }

        private static List<IPlayer> GetPlayerList()
        {
            var player1 = new Mock<IPlayer>();
            var player2 = new Mock<IPlayer>();
            var player3 = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var player5 = new Mock<IPlayer>();
            var playerMockList = new List<Mock<IPlayer>>() { player1, player2, player3, player4, player5 };
            playerMockList.ForEach(m => m.Setup(mm => mm.Cards).Returns(new List<SheepCard>()));
            playerMockList.ForEach(m => m.Setup(mm => mm.Participant).Returns(new Participant()));
            var playerList = playerMockList.Select(m => m.Object).ToList();
            return playerList;
        }

        private static Mock<IHand> GetHand(IPlayer startingPlayer, List<IPlayer> playerList)
        {
            var trickList = new List<ITrick>();
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.StartingPlayer).Returns(startingPlayer);
            handMock.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick t) => { trickList.Add(t); });
            handMock.Setup(m => m.ITricks).Returns(trickList);
            handMock.Setup(m => m.Players).Returns(playerList);
            handMock.Setup(m => m.PlayerCount).Returns(playerList.Count);
            return handMock;
        }

        [TestMethod]
        public void Trick_OrderedMoves()
        {
            var participants = new List<Participant>() { new Participant(), new Participant(), new Participant(), new Participant(), new Participant() };
            var playerList = participants.Select(p => p.Player).ToList();
            var startingPlayer = playerList[3];
            var cardList = new List<SheepCard>() { 0, (SheepCard)1, (SheepCard)2, (SheepCard)3, (SheepCard)4, };
            var trickList = new List<ITrick>();
            var handMock = GetHand(startingPlayer, playerList);

            var trick = new Trick(handMock.Object);
            for (var i = 0; i < 5; ++i) {
                var index = i + 3 < 5 ? i + 3 : i + 3 - 5; //We want to start with player 4.
                trick.Add(playerList[index], cardList[index]);
            }

            for (var i = 0; i < 5; ++i)
            {
                var index = i + 3 < 5 ? i + 3 : i + 3 - 5; //We want to start with player 4.
                Assert.AreSame(playerList[index], trick.OrderedMoves[i].Key, "Expected players to match at move " + i);
                Assert.AreEqual(cardList[index], trick.OrderedMoves[i].Value, "Expected cards to match at move " + i);
            }
        }

        [TestMethod]
        public void Trick_OnTrickEnd()
        {
            var playerList = GetPlayerList();
            var startingPlayer = playerList[3];
            var cardList = new List<SheepCard>() { 0, (SheepCard)1, (SheepCard)2, (SheepCard)3, (SheepCard)4, };
            var handMock = GetHand(startingPlayer, playerList);
            var event1hit = false;
            var event2hit = false;

            var trick = new Trick(handMock.Object);
            trick.OnTrickEnd += (object sender, EventArgs e) =>
            {
                event1hit = true;
            };
            trick.OnTrickEnd += (object sender, EventArgs e) =>
            {
                event2hit = true;
            };
            for (var i = 0; i < 5; ++i)
            {
                Assert.IsFalse(event1hit);
                Assert.IsFalse(event2hit);
                var index = i + 3 < 5 ? i + 3 : i + 3 - 5; //We want to start with player 4.
                trick.Add(playerList[index], cardList[index]);
            }

            Assert.IsTrue(event1hit);
            Assert.IsTrue(event2hit);
        }

        [TestMethod]
        public void Trick_OnMove()
        {
            var playerList = GetPlayerList();
            var startingPlayer = playerList[3];
            var cardList = new List<SheepCard>() { 0, (SheepCard)1, (SheepCard)2, (SheepCard)3, (SheepCard)4, };
            var handMock = GetHand(startingPlayer, playerList);
            var event1hit = false;
            var event2hit = false;
            var expectedValues = new object[]{ null, null };

            var trick = new Trick(handMock.Object);
            trick.OnMove += (object sender, Trick.MoveEventArgs e) =>
            {
                event1hit = true;
                Assert.AreSame(expectedValues[0], e.Player);
                Assert.AreEqual((SheepCard)expectedValues[1], e.Card);
            };
            trick.OnMove += (object sender, Trick.MoveEventArgs e) =>
            {
                event2hit = true;
                Assert.AreSame(expectedValues[0], e.Player);
                Assert.AreEqual((SheepCard)expectedValues[1], e.Card);
            };
            for (var i = 0; i < 5; ++i)
            {
                event1hit = false;
                event2hit = false;
                var index = i + 3 < 5 ? i + 3 : i + 3 - 5; //We want to start with player 4.
                expectedValues[0] = playerList[index];
                expectedValues[1] = cardList[index];
                trick.Add(playerList[index], cardList[index]);
                Assert.IsTrue(event1hit);
                Assert.IsTrue(event2hit);
            }
        }
    }

    public class MockPlayer : IPlayer
    {
        public string Name { get; set; }

        private List<SheepCard> _cards = new List<SheepCard>();
        IReadOnlyList<SheepCard> IPlayer.Cards => _cards;
        public Participant Participant { get; } = new Participant();

        public MockPlayer()
        {
        }

        public int QueueRankInTrick(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public int QueueRankInHand(IHand hand)
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> LegalCalledAces(IHand hand)
        {
            throw new NotImplementedException();
        }

        public void AddCard(SheepCard card)
        {
            _cards.Add(card);
        }

        public void RemoveCard(SheepCard card)
        {
            _cards.Remove(card);
        }

        public void RemoveAllCards()
        {
            _cards.Clear();
        }

        public void AddCardRange(List<SheepCard> cards)
        {
            _cards.AddRange(cards);
        }
    }
}
