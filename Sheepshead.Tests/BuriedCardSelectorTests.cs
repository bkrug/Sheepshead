using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class BuriedCardSelectorTests
    {
        [TestMethod]
        public void BuriedCardSelector_Constructor()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.ACE_HEARTS,
                SheepCard.N10_SPADES,
                SheepCard.N10_HEARTS,
                SheepCard.N7_SPADES,
                SheepCard.KING_HEARTS,
                SheepCard.ACE_CLUBS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            Assert.AreEqual(3, selector.CardsPerSuit[Suit.HEARTS]);
            Assert.AreEqual(2, selector.CardsPerSuit[Suit.SPADES]);
            Assert.AreEqual(1, selector.CardsPerSuit[Suit.CLUBS]);
        }

        [TestMethod]
        public void BuriedCardSelector_RetireTwoSuitsWithAceAndTen()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.ACE_HEARTS,
                SheepCard.N10_SPADES,
                SheepCard.N10_CLUBS,
                SheepCard.N7_CLUBS,
                SheepCard.KING_DIAMONDS,
                SheepCard.ACE_CLUBS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.ACE_HEARTS, SheepCard.N10_SPADES };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void BuriedCardSelector_RetireOneSuitsWithAceAndTen()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.ACE_HEARTS,
                SheepCard.N10_SPADES,
                SheepCard.N10_CLUBS,
                SheepCard.ACE_SPADES,
                SheepCard.KING_DIAMONDS,
                SheepCard.ACE_CLUBS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N10_SPADES, SheepCard.ACE_SPADES };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void BuriedCardSelector_BuryAcesAndTensFromSmallestFails()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.ACE_HEARTS,
                SheepCard.N7_SPADES,
                SheepCard.N10_CLUBS,
                SheepCard.N8_SPADES,
                SheepCard.KING_DIAMONDS,
                SheepCard.N9_CLUBS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N10_CLUBS, SheepCard.ACE_HEARTS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void BuriedCardSelector_RetireTwoFailSuitsWithOneAceOrTen()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.N7_HEARTS,
                SheepCard.N7_SPADES,
                SheepCard.N10_CLUBS,
                SheepCard.N8_SPADES,
                SheepCard.KING_DIAMONDS,
                SheepCard.KING_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.N10_CLUBS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual, "We can find two suits to retire, and also bury points");
        }

        [TestMethod]
        public void BuriedCardSelector_RetireOneFailSuitsWithOneAceOrTen()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_SPADES,
                SheepCard.N10_CLUBS,
                SheepCard.N8_SPADES,
                SheepCard.KING_DIAMONDS,
                SheepCard.KING_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N7_CLUBS, SheepCard.N10_CLUBS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual, "We can find one suit to retire, and also bury points");
        }

        [TestMethod]
        public void BuriedCardSelector_RetireTwoFailSuits()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_SPADES,
                SheepCard.KING_HEARTS,
                SheepCard.N8_SPADES,
                SheepCard.KING_DIAMONDS,
                SheepCard.KING_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N7_CLUBS, SheepCard.KING_HEARTS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual, "There are two fail suits for which we have only one card.");
        }

        [TestMethod]
        public void BuriedCardSelector_RetireOneFailSuits()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.JACK_DIAMONDS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_SPADES,
                SheepCard.KING_CLUBS,
                SheepCard.N8_SPADES,
                SheepCard.KING_DIAMONDS,
                SheepCard.KING_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N7_CLUBS, SheepCard.KING_CLUBS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual, "There is one fail suits for which we have exactly two cards.");
        }


    }
}
