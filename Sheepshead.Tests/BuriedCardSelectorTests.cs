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

        [TestMethod]
        public void BuriedCardSelector_BuryCardsByEasiestToRetireLowestRank_RetireOneButNotOther()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.N7_SPADES,
                SheepCard.JACK_HEARTS,
                SheepCard.N8_DIAMONDS,
                SheepCard.N8_SPADES,
                SheepCard.KING_HEARTS,
                SheepCard.KING_DIAMONDS,
                SheepCard.N9_SPADES,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.KING_HEARTS, SheepCard.KING_CLUBS };
            var actual = selector.CardsToBury;
            Assert.IsTrue(actual.Contains(SheepCard.KING_HEARTS));
            Assert.IsTrue(actual.Any(c => CardUtil.GetSuit(c) == Suit.SPADES));
        }

        [TestMethod]
        public void BuriedCardSelector_BuryCardsByEasiestToRetireLowestRank_RetireNothing()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.N9_SPADES,
                SheepCard.N8_SPADES,
                SheepCard.N7_SPADES,
                SheepCard.N7_CLUBS,
                SheepCard.KING_CLUBS,
                SheepCard.N8_CLUBS,
                SheepCard.KING_DIAMONDS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var actual = selector.CardsToBury;
            Assert.IsTrue(actual.All(c => CardUtil.GetSuit(c) == Suit.SPADES) || actual.All(c => CardUtil.GetSuit(c) == Suit.CLUBS));
        }

        [TestMethod]
        public void BuriedCardSelector_BuryCardsByLowestRank_OneFail()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.N8_DIAMONDS,
                SheepCard.N7_DIAMONDS,
                SheepCard.QUEEN_HEARTS,
                SheepCard.JACK_SPADES,
                SheepCard.N9_SPADES,
                SheepCard.ACE_DIAMONDS,
                SheepCard.KING_DIAMONDS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N9_SPADES, SheepCard.N7_DIAMONDS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void BuriedCardSelector_BuryCardsByLowestRank_AllTrump()
        {
            var cards = new List<SheepCard>()
            {
                SheepCard.N8_DIAMONDS,
                SheepCard.N7_DIAMONDS,
                SheepCard.QUEEN_HEARTS,
                SheepCard.JACK_SPADES,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.ACE_DIAMONDS,
                SheepCard.KING_DIAMONDS,
                SheepCard.QUEEN_CLUBS
            };
            var selector = new BuriedCardSelector(cards);
            var expected = new List<SheepCard>() { SheepCard.N8_DIAMONDS, SheepCard.N7_DIAMONDS };
            var actual = selector.CardsToBury;
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
