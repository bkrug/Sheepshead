using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_ToAbbr()
        {
            var card1 = new Card(StandardSuite.DIAMONDS, CardType.JACK, 0, 0);
            Assert.AreEqual("JD", card1.ToAbbr());
            var card2 = new Card(StandardSuite.HEARTS, CardType.N7, 0, 0);
            Assert.AreEqual("7H", card2.ToAbbr());
        }

        [TestMethod]
        public void CardRepository_GetFace()
        {
            Assert.AreEqual(CardType.QUEEN, CardRepository.GetFace(SheepCard.QUEEN_CLUBS));
            Assert.AreEqual(CardType.QUEEN, CardRepository.GetFace(SheepCard.QUEEN_DIAMONDS));
            Assert.AreEqual(CardType.N7, CardRepository.GetFace(SheepCard.N7_CLUBS));
            Assert.AreEqual(CardType.N7, CardRepository.GetFace(SheepCard.N7_DIAMONDS));
        }

        [TestMethod]
        public void CardRepository_GetSuit()
        {
            Assert.AreEqual(Suit.TRUMP, CardRepository.GetSuit(SheepCard.QUEEN_CLUBS));
            Assert.AreEqual(Suit.TRUMP, CardRepository.GetSuit(SheepCard.QUEEN_DIAMONDS));
            Assert.AreEqual(Suit.SPADES, CardRepository.GetSuit(SheepCard.ACE_SPADES));
            Assert.AreEqual(Suit.CLUBS, CardRepository.GetSuit(SheepCard.N7_CLUBS));
            Assert.AreEqual(Suit.TRUMP, CardRepository.GetSuit(SheepCard.N7_DIAMONDS));
        }

        [TestMethod]
        public void CardRepository_GetStandardSuit()
        {
            Assert.AreEqual(StandardSuite.CLUBS, CardRepository.GetStandardSuit(SheepCard.QUEEN_CLUBS));
            Assert.AreEqual(StandardSuite.DIAMONDS, CardRepository.GetStandardSuit(SheepCard.QUEEN_DIAMONDS));
            Assert.AreEqual(StandardSuite.SPADES, CardRepository.GetStandardSuit(SheepCard.ACE_SPADES));
            Assert.AreEqual(StandardSuite.CLUBS, CardRepository.GetStandardSuit(SheepCard.N7_CLUBS));
            Assert.AreEqual(StandardSuite.DIAMONDS, CardRepository.GetStandardSuit(SheepCard.N7_DIAMONDS));
        }
    }
}
