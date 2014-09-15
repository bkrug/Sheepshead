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
        private IPlayer GetPlayer(List<Card> hand)
        {
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Hand).Returns(hand);
            return playerMock.Object;
        }

        [TestMethod]
        public void Trick_IsLegal()
        {
            var firstPlayer = new Mock<IPlayer>().Object;
            {
                var hand = new List<Card>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                var player = GetPlayer(hand);
                var trick = new Trick();
                trick.Add(firstPlayer, CardRepository.Instance[StandardSuite.HEARTS, CardType.N9]);
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], player), "A hearts is part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], player), "A clubs is not part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], player), "A trump is not part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N10], player), "A card outside of the hand is not legal.");
            }
            {
                var hand = new List<Card>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                var player = GetPlayer(hand);
                var trick = new Trick();
                trick.Add(firstPlayer, CardRepository.Instance[StandardSuite.SPADES, CardType.N9]);
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], player), "There is no spades in the hand. Hearts is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], player), "There is no spades in the hand. Clubs is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], player), "There is no spades in the hand. Trump is fine.");
            }
            {
                var trick = new Trick();
                var hand = new List<Card>() {
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
            var player1 = new Mock<IPlayer>().Object;
            var player2 = new Mock<IPlayer>().Object;
            var player3 = new Mock<IPlayer>().Object;
            var player4 = new Mock<IPlayer>().Object;
            var player5 = new Mock<IPlayer>().Object;
            {
                var trick = new Trick();
                trick.Add(player1, CardRepository.Instance[StandardSuite.HEARTS, CardType.N8]);
                trick.Add(player2, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE]);
                trick.Add(player3, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]);
                trick.Add(player4, CardRepository.Instance[StandardSuite.HEARTS, CardType.KING]);
                trick.Add(player5, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE]);
                Assert.AreEqual(player3, trick.Winner(), "Ten of hearts has the hights rank of correct suite.");
            }
            {
                var trick = new Trick();
                trick.Add(player1, CardRepository.Instance[StandardSuite.HEARTS, CardType.N8]);
                trick.Add(player2, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]);
                trick.Add(player3, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]);
                trick.Add(player4, CardRepository.Instance[StandardSuite.HEARTS, CardType.KING]);
                trick.Add(player5, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE]);
                Assert.AreEqual(player2, trick.Winner(), "Trump has the highest rank.");
            }
            {
                var trick = new Trick();
                trick.Add(player1, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]);
                trick.Add(player2, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE]);
                trick.Add(player3, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]);
                trick.Add(player4, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING]);
                trick.Add(player5, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE]);
                Assert.AreEqual(player4, trick.Winner(), "Trump has the highest rank when it is played first, too.");
            }
        }
    }
}
