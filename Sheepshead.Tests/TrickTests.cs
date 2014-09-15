using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using System.Collections.Generic;

namespace Sheepshead.Tests
{
    [TestClass]
    public class TrickTests
    {
        [TestMethod]
        public void Trick_IsLegal()
        {
            {
                var trick = new Trick();
                trick.Add(CardRepository.Instance[StandardSuite.HEARTS, CardType.N9]);
                var hand = new List<Card>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], hand), "A hearts is part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], hand), "A clubs is not part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.QUEEN], hand), "A trump is not part of the same suite.");
                Assert.IsFalse(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N10], hand), "A card outside of the hand is not legal.");
            }
            {
                var trick = new Trick();
                trick.Add(CardRepository.Instance[StandardSuite.SPADES, CardType.N9]);
                var hand = new List<Card>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], hand), "There is no spades in the hand. Hearts is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.N8], hand), "There is no spades in the hand. Clubs is fine.");
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], hand), "There is no spades in the hand. Trump is fine.");
            }
            {
                var trick = new Trick();
                var hand = new List<Card>() {
                    CardRepository.Instance[StandardSuite.HEARTS, CardType.KING], CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], 
                    CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN], CardRepository.Instance[StandardSuite.CLUBS, CardType.N8]
                };
                Assert.IsTrue(trick.IsLegalAddition(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], hand), "Adding a card to an empty trick is always okay.");
            }
        }
    }
}
