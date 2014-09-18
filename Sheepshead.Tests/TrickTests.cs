using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using System.Collections.Generic;
using Moq;

namespace Sheepshead.Tests
{
    [TestClass]
    public class TrickTests
    {
        private IHand GetHand()
        {
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.CLUBS, CardType.KING]);
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
                Assert.AreEqual(player3, winner.Player, "Ten of hearts has the hights rank of correct suite.");
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
        }

        [TestMethod]
        public void Trick_SetPartner()
        {
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.PartnerCard).Returns(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]);
            var hand = mockHand.Object;
            var trick = new Trick(hand);
            var player = new Mock<IPlayer>();
            player.Setup(c => c.Name).Returns("DesiredPlayer");
            player.Setup(c => c.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]});
            mockHand.SetupProperty(f => f.Partner);
            trick.Add(player.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN]);
            Assert.AreEqual(player.Object, hand.Partner, "When someone adds the partner card to the trick, the hand's partner get's specified.");
        }
    }
}
