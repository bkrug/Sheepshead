﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class TrickTests
    {
        private IHand GetHand()
        {
            var handMock = new Mock<IHand>();
            var trickList = new List<ITrick>();
            handMock.Setup(m => m.Tricks).Returns(trickList);
            handMock.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.CLUBS, CardType.KING]);
            handMock.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            return handMock.Object;
        }

        private IPlayer GetPlayer(List<ICard> hand)
        {
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(hand);
            return playerMock.Object;
        }

        [TestMethod]
        public void Trick_IsLegal()
        {
            var firstPlayerMock = new Mock<IPlayer>();
            var firstPlayer = firstPlayerMock.Object;
            {
                firstPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.HEARTS, CardType.N9]});
                var hand = new List<ICard>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                var player = GetPlayer(hand);
                var trick = new Trick(GetHand());
                trick.Add(firstPlayer, CardRepository.Instance[StandardSuite.HEARTS, CardType.N9]);
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], player), "A hearts is part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], player), "A clubs is not part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], player), "A trump is not part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N10], player), "A card outside of the hand is not legal.");
            }
            {
                firstPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.SPADES, CardType.N9] });
                var hand = new List<ICard>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                var player = GetPlayer(hand);
                var trick = new Trick(GetHand());
                trick.Add(firstPlayer, CardRepository.Instance[StandardSuite.SPADES, CardType.N9]);
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], player), "There is no spades in the hand. Hearts is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], player), "There is no spades in the hand. Clubs is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], player), "There is no spades in the hand. Trump is fine.");
            }
            {
                var trick = new Trick(GetHand());
                var hand = new List<ICard>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                var player = GetPlayer(hand);
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], player), "Adding a card to an empty trick is always okay.");
            }
        }

        [TestMethod]
        public void Trick_Winner()
        {
            var player1 = new MockPlayer();
            var player2 = new MockPlayer();
            var player3 = new MockPlayer();
            var player4 = new MockPlayer();
            var player5 = new MockPlayer();
            {
                var trick = new Trick(GetHand());
                trick.Add(player1, CardRepository.Instance[StandardSuite.HEARTS, CardType.N8]);
                trick.Add(player2, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE]);
                trick.Add(player3, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]);
                trick.Add(player4, CardRepository.Instance[StandardSuite.HEARTS, CardType.KING]);
                trick.Add(player5, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE]);
                var winner = trick.Winner();
                Assert.AreEqual(player3, winner.Player, "Ten of hearts has the hightest rank of the correct suite.");
                Assert.AreEqual(36, winner.Points, "Expected points for 2 Aces, 1 King, 1 Ten.");
            }
            {
                var trick = new Trick(GetHand());
                trick.Add(player1, CardRepository.Instance[StandardSuite.HEARTS, CardType.N8]);
                trick.Add(player2, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]);
                trick.Add(player3, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]);
                trick.Add(player4, CardRepository.Instance[StandardSuite.HEARTS, CardType.KING]);
                trick.Add(player5, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE]);
                var winner = trick.Winner();
                Assert.AreEqual(player2, winner.Player, "Trump has the highest rank.");
                Assert.AreEqual(25, winner.Points, "Expected points for 1 Aces, 1 Ten, 1 King.");
            }
            {
                var trick = new Trick(GetHand());
                trick.Add(player1, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]);
                trick.Add(player2, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE]);
                trick.Add(player3, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]);
                trick.Add(player4, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING]);
                trick.Add(player5, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE]);
                var winner = trick.Winner();
                Assert.AreEqual(player4, winner.Player, "Trump has the highest rank when it is played first, too.");
                Assert.AreEqual(36, winner.Points, "Expected points for 2 Aces, 1 King, 1 Ten.");
            }
        }

        public class MockPlayer : IPlayer
        {
            public List<ICard> Cards { get; set; }
            public string Name { get; set; }

            public MockPlayer()
            {
                Cards = new List<ICard>();
            }

            public int QueueRankInTrick(ITrick trick)
            {
                throw new NotImplementedException();
            }

            public int QueueRankInDeck(IDeck deck)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void Trick_SetPartner()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var trickList = new List<ITrick>();
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]);
            mockHand.Setup(m => m.PlayerCount).Returns(5);
            mockHand.Setup(m => m.Players).Returns(new List<IPlayer>());
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            var hand = mockHand.Object;
            var trick = new Trick(hand);
            var player = new Mock<IPlayer>();
            player.Setup(c => c.Name).Returns("DesiredPlayer");
            player.Setup(c => c.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]});
            IPlayer partner = player.Object;
            var _methodCalled = false;
            mockHand.Setup(f => f.SetPartner(It.IsAny<IPlayer>(), It.IsAny<ITrick>())).Callback((IPlayer pl, ITrick tr) => {
                _methodCalled = true;
                Assert.AreEqual(partner, pl);
                Assert.AreEqual(trick, tr);
            });
            trick.Add(player.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]);
            Assert.IsTrue(_methodCalled, "When someone adds the partner card to the trick, the hand's partner get's specified.");
        }

        [TestMethod]
        public void Trick_AddToHand()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var mockHand = new Mock<IHand>();
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.Deck).Returns(new Mock<IDeck>().Object);
            ITrick passedTrick = null;
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick givenTrick) =>
            {
                trickList.Add(givenTrick);
                passedTrick = givenTrick;
            });
            var trick = new Trick(mockHand.Object);
            Assert.AreSame(trick, passedTrick, "When a trick is instantiated, it should be added to a given hand.");
        }

        [TestMethod]
        public void Trick_IsComplete()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.Players).Returns(new List<IPlayer>());
            mockHand.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N10]);
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            foreach (var playerCount in new[] { 3, 5 })
            {
                mockHand.Setup(m => m.PlayerCount).Returns(playerCount);
                var trick = new Trick(mockHand.Object);
                for (var cardsInTrick = 0; cardsInTrick < playerCount; ++cardsInTrick)
                {
                    Assert.IsFalse(trick.IsComplete(), "Trick should not be complete when there are " + cardsInTrick + " cards in the trick and " + playerCount + " players in the game.");
                    trick.Add(new Player(), new Card());
                }
                Assert.IsTrue(trick.IsComplete(), "Trick should be complete when there are " + playerCount + " cards in the trick and " + playerCount + " players in the game.");
            }
        }

        [TestMethod]
        public void Trick_StartingPlayer()
        {
            var player1 = new Mock<BasicPlayer>();
            var player2 = new Mock<NewbiePlayer>();
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<BasicPlayer>();
            var player5 = new Mock<BasicPlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.StartingPlayer).Returns(player1.Object);
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            var mockCompleteTrick = new Mock<ITrick>();
            mockCompleteTrick.Setup(m => m.Winner()).Returns(new TrickWinner() { Player = player4.Object, Points = 94 });

            var trick1 = new Trick(mockHand.Object);
            Assert.AreEqual(player1.Object, trick1.StartingPlayer, "The starting player for first trick should be the same as the starting player for the deck.");

            trickList.Remove(trick1);
            trickList.Add(mockCompleteTrick.Object);
            var trick2 = new Trick(mockHand.Object);
            Assert.AreEqual(player4.Object, trick2.StartingPlayer, "The starting player for the second trick should be the winner of the previous trick.");
        }

        private static List<IPlayer> GetPlayerList()
        {
            var player1 = new Mock<BasicPlayer>();
            var player2 = new Mock<NewbiePlayer>();
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<BasicPlayer>();
            var player5 = new Mock<BasicPlayer>();
            var playerList = new List<IPlayer>() { player1.Object, player2.Object, player3.Object, player4.Object, player5.Object };
            return playerList;
        }

        private static Mock<IHand> GetHand(IPlayer startingPlayer, List<IPlayer> playerList)
        {
            var trickList = new List<ITrick>();
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.StartingPlayer).Returns(startingPlayer);
            handMock.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick t) => { trickList.Add(t); });
            handMock.Setup(m => m.Tricks).Returns(trickList);
            handMock.Setup(m => m.Players).Returns(playerList);
            handMock.Setup(m => m.PlayerCount).Returns(playerList.Count);
            return handMock;
        }

        [TestMethod]
        public void Trick_OrderedMoves()
        {
            var playerList = GetPlayerList();
            var startingPlayer = playerList[3];
            var cardList = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, };
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
                Assert.AreSame(cardList[index], trick.OrderedMoves[i].Value, "Expected cards to match at move " + i);
            }
        }

        [TestMethod]
        public void Trick_OnTrickEnd()
        {
            var playerList = GetPlayerList();
            var startingPlayer = playerList[3];
            var cardList = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, };
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
            var cardList = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object, };
            var handMock = GetHand(startingPlayer, playerList);
            var event1hit = false;
            var event2hit = false;
            var expectedValues = new object[]{ null, null };

            var trick = new Trick(handMock.Object);
            trick.OnMove += (object sender, Trick.MoveEventArgs e) =>
            {
                event1hit = true;
                Assert.AreSame(expectedValues[0], e.Player);
                Assert.AreSame(expectedValues[1], e.Card);
            };
            trick.OnMove += (object sender, Trick.MoveEventArgs e) =>
            {
                event2hit = true;
                Assert.AreSame(expectedValues[0], e.Player);
                Assert.AreSame(expectedValues[1], e.Card);
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
}
