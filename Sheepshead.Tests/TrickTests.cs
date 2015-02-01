using System;
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
            var gameMock = new Mock<IGame>();
            gameMock.Setup(m => m.PlayerCount).Returns(5);
            gameMock.Setup(m => m.Players).Returns(new List<IPlayer>());
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Game).Returns(gameMock.Object);
            var handMock = new Mock<IHand>();
            var trickList = new List<ITrick>();
            handMock.Setup(m => m.Tricks).Returns(trickList);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
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
                var trick = new Trick(GetHand(), null);
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
                var trick = new Trick(GetHand(), null);
                trick.Add(firstPlayer, CardRepository.Instance[StandardSuite.SPADES, CardType.N9]);
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], player), "There is no spades in the hand. Hearts is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], player), "There is no spades in the hand. Clubs is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], player), "There is no spades in the hand. Trump is fine.");
            }
            {
                var trick = new Trick(GetHand(), null);
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
                var trick = new Trick(GetHand(), null);
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
                var trick = new Trick(GetHand(), null);
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
                var trick = new Trick(GetHand(), null);
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
            var mockGame = new Mock<IGame>();
            mockGame.Setup(m => m.PlayerCount).Returns(5);
            mockGame.Setup(m => m.Players).Returns(new List<IPlayer>());
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Game).Returns(mockGame.Object);
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]);
            mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            var hand = mockHand.Object;
            var trick = new Trick(hand, null);
            var player = new Mock<IPlayer>();
            player.Setup(c => c.Name).Returns("DesiredPlayer");
            player.Setup(c => c.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]});
            mockHand.SetupProperty(f => f.Partner);
            trick.Add(player.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]);
            Assert.AreEqual(player.Object, hand.Partner, "When someone adds the partner card to the trick, the hand's partner get's specified.");
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
            var trick = new Trick(mockHand.Object, null);
            Assert.AreSame(trick, passedTrick, "When a trick is instantiated, it should be added to a given hand.");
        }

        [TestMethod]
        public void Trick_IsComplete()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            var mockHand = new Mock<IHand>();
            var mockDeck = new Mock<IDeck>();
            var mockGame = new Mock<IGame>();
            mockGame.Setup(m => m.Players).Returns(new List<IPlayer>());
            mockHand.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N10]);
            mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
            mockDeck.Setup(m => m.Game).Returns(mockGame.Object);
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            foreach (var playerCount in new[] { 3, 5 })
            {
                mockGame.Setup(m => m.PlayerCount).Returns(playerCount);
                var trick = new Trick(mockHand.Object, null);
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
            var mockGame = new Mock<IGame>();
            mockGame.Setup(m => m.Players).Returns(playerList);
            mockGame.Setup(m => m.PlayerCount).Returns(5);
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Game).Returns(mockGame.Object);
            mockDeck.Setup(m => m.StartingPlayer).Returns(player1.Object);
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
            var trickList = new List<ITrick>();
            mockHand.Setup(m => m.Tricks).Returns(trickList);
            mockHand.Setup(m => m.AddTrick(It.IsAny<ITrick>())).Callback((ITrick newTrick) => { trickList.Add(newTrick); });
            var mockCompleteTrick = new Mock<ITrick>();
            mockCompleteTrick.Setup(m => m.Winner()).Returns(new TrickWinner() { Player = player4.Object, Points = 94 });

            var trick1 = new Trick(mockHand.Object, null);
            Assert.AreEqual(player1.Object, trick1.StartingPlayer, "The starting player for first trick should be the same as the starting player for the deck.");

            trickList.Remove(trick1);
            trickList.Add(mockCompleteTrick.Object);
            var trick2 = new Trick(mockHand.Object, null);
            Assert.AreEqual(player4.Object, trick2.StartingPlayer, "The starting player for the second trick should be the winner of the previous trick.");
        }
    }
}
