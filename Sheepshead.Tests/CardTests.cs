using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_ToAbbr()
        {
            Assert.AreEqual("JD", CardRepository.ToAbbr(SheepCard.JACK_DIAMONDS));
            Assert.AreEqual("7H", CardRepository.ToAbbr(SheepCard.N7_HEARTS));
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
