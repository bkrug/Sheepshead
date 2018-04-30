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
            Assert.AreEqual("JD", CardUtil.ToAbbr(SheepCard.JACK_DIAMONDS));
            Assert.AreEqual("7H", CardUtil.ToAbbr(SheepCard.N7_HEARTS));
        }

        [TestMethod]
        public void CardUtil_GetFace()
        {
            Assert.AreEqual(CardType.QUEEN, CardUtil.GetFace(SheepCard.QUEEN_CLUBS));
            Assert.AreEqual(CardType.QUEEN, CardUtil.GetFace(SheepCard.QUEEN_DIAMONDS));
            Assert.AreEqual(CardType.N7, CardUtil.GetFace(SheepCard.N7_CLUBS));
            Assert.AreEqual(CardType.N7, CardUtil.GetFace(SheepCard.N7_DIAMONDS));
        }

        [TestMethod]
        public void CardUtil_GetSuit()
        {
            Assert.AreEqual(Suit.TRUMP, CardUtil.GetSuit(SheepCard.QUEEN_CLUBS));
            Assert.AreEqual(Suit.TRUMP, CardUtil.GetSuit(SheepCard.QUEEN_DIAMONDS));
            Assert.AreEqual(Suit.SPADES, CardUtil.GetSuit(SheepCard.ACE_SPADES));
            Assert.AreEqual(Suit.CLUBS, CardUtil.GetSuit(SheepCard.N7_CLUBS));
            Assert.AreEqual(Suit.TRUMP, CardUtil.GetSuit(SheepCard.N7_DIAMONDS));
        }

        [TestMethod]
        public void CardUtil_GetStandardSuit()
        {
            Assert.AreEqual(StandardSuite.CLUBS, CardUtil.GetStandardSuit(SheepCard.QUEEN_CLUBS));
            Assert.AreEqual(StandardSuite.DIAMONDS, CardUtil.GetStandardSuit(SheepCard.QUEEN_DIAMONDS));
            Assert.AreEqual(StandardSuite.SPADES, CardUtil.GetStandardSuit(SheepCard.ACE_SPADES));
            Assert.AreEqual(StandardSuite.CLUBS, CardUtil.GetStandardSuit(SheepCard.N7_CLUBS));
            Assert.AreEqual(StandardSuite.DIAMONDS, CardUtil.GetStandardSuit(SheepCard.N7_DIAMONDS));
        }
    }
}
