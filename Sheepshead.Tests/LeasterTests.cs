using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Tests
{
    [TestClass]
    public class LeasterTests
    {
        [TestMethod]
        public void LeasterKeyGenerator_GetKey_FirstTrick_MatchingSuit()
        {
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N10])
            });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { trickMock.Object });
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            var curPlayerMock = new Mock<IPlayer>();
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.SPADES, CardType.N9],
                CardRepository.Instance[StandardSuite.SPADES, CardType.N10],
                CardRepository.Instance[StandardSuite.SPADES, CardType.ACE],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.ACE],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N10]
            });
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(trickMock.Object.OrderedMoves.Count() + 1);
            var playedCard = CardRepository.Instance[StandardSuite.SPADES, CardType.N9];
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = false,
                LostOneTrick = false,
                CardMatchesSuit = true,
                MostPowerfulInTrick = false,
                OpponentPercentDone = 50,
                AvgVisibleCardPoints = (int)(Math.Round((10 + 4) / 3.0)),
                UnknownStrongerCards = 17 - 5, //There are 19 stronger cards.  3 are held, 2 have been played.
                HeldStrongerCards = 3 //One trump and two spaces
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void LeasterKeyGenerator_GetKey_FirstTrick_DifferentSuit()
        {
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10])
            });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { trickMock.Object });
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            var curPlayerMock = new Mock<IPlayer>();
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.HEARTS, CardType.N9],
                CardRepository.Instance[StandardSuite.SPADES, CardType.N10],
                CardRepository.Instance[StandardSuite.SPADES, CardType.ACE],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.ACE],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N10]
            });
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(trickMock.Object.OrderedMoves.Count() + 1);
            var playedCard = CardRepository.Instance[StandardSuite.HEARTS, CardType.N9];
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = false,
                LostOneTrick = false,
                CardMatchesSuit = false,
                MostPowerfulInTrick = false,
                OpponentPercentDone = 50,
                AvgVisibleCardPoints = (int)(Math.Round((10 + 4) / 3.0)),
                UnknownStrongerCards = 17 - 3, //There are 17 stronger cards.  2 are held, 1 have been played.
                HeldStrongerCards = 2 //One trump and two heart
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void LeasterKeyGenerator_GetKey_LostOneTrick_MatchingSuit()
        {
            var curPlayerMock = new Mock<IPlayer>();
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.SPADES, CardType.N10],
                CardRepository.Instance[StandardSuite.SPADES, CardType.ACE],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.ACE],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N10]
            });
            var prevTrickMock = new Mock<ITrick>();
            prevTrickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N9])
            });
            prevTrickMock.Setup(m => m.CardsPlayed).Returns(prevTrickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = null, //It just matters that it not be the same player making the move.
                Points = 23
            });
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.KING])
            });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { prevTrickMock.Object, trickMock.Object });
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(trickMock.Object.OrderedMoves.Count() + 1);
            var playedCard = CardRepository.Instance[StandardSuite.SPADES, CardType.N10];
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = false,
                LostOneTrick = true,
                CardMatchesSuit = true,
                MostPowerfulInTrick = true,
                OpponentPercentDone = 25,
                AvgVisibleCardPoints = (int)(Math.Round((10 + 4) / 2.0)),
                UnknownStrongerCards = 15 - 4, //There are 15 stronger cards.  2 held, 2 have been played.
                HeldStrongerCards = 2 //One trump and two spaces
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void LeasterKeyGenerator_GetKey_WonOneTrick_MatchingSuit()
        {
            var curPlayerMock = new Mock<IPlayer>();
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.SPADES, CardType.N10],
                CardRepository.Instance[StandardSuite.SPADES, CardType.ACE],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.ACE],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N10]
            }); 
            var prevTrickMock = new Mock<ITrick>();
            prevTrickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.CLUBS, CardType.QUEEN]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N9])
            });
            prevTrickMock.Setup(m => m.CardsPlayed).Returns(prevTrickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = curPlayerMock.Object,
                Points = 23
            });
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.KING])
            });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { prevTrickMock.Object, trickMock.Object });
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(trickMock.Object.OrderedMoves.Count() + 1);
            var playedCard = CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7];
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = true,
                LostOneTrick = false,
                CardMatchesSuit = true,
                MostPowerfulInTrick = false,
                OpponentPercentDone = 75,
                AvgVisibleCardPoints = (int)Math.Round((4 + 4 + 3) / (double)4),  //Card played plus previously played cards
                UnknownStrongerCards = 13 - 3, //There are 19 stronger cards.  3 have been played.
                HeldStrongerCards = 0 //This is the only trump
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void LeasterKeyGenerator_GetKey_WonOneTrick_LostOneTrick_FirstMoveInTrick()
        {
            var curPlayerMock = new Mock<IPlayer>();
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.KING],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N10]
            });
            var prevTrickMock = new Mock<ITrick>();
            prevTrickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N7]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N9]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE])
            });
            prevTrickMock.Setup(m => m.CardsPlayed).Returns(prevTrickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = null,
                Points = 23
            });
            var prevTrickMock1 = new Mock<ITrick>();
            prevTrickMock1.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N9]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.ACE])
            });
            prevTrickMock1.Setup(m => m.CardsPlayed).Returns(prevTrickMock1.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock1.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = curPlayerMock.Object,
                Points = 23
            });
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() );
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { prevTrickMock.Object, prevTrickMock1.Object, trickMock.Object });
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(trickMock.Object.OrderedMoves.Count() + 1);
            var playedCard = CardRepository.Instance[StandardSuite.HEARTS, CardType.KING];
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = true,
                LostOneTrick = true,
                CardMatchesSuit = true,
                MostPowerfulInTrick = true,
                OpponentPercentDone = 0,
                AvgVisibleCardPoints = 4,  //Card played plus previously played cards
                UnknownStrongerCards = 16 - 8, //There are 19 stronger cards. 1 held, 7 have been played.
                HeldStrongerCards = 1
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void LeasterKeyGenerator_GetKey_MowerPotential()
        {
            var curPlayerMock = new Mock<IPlayer>();
            var prevTrickMock = new Mock<ITrick>();
            prevTrickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N7]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N9]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE])
            });
            prevTrickMock.Setup(m => m.CardsPlayed).Returns(prevTrickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = curPlayerMock.Object,
                Points = 23
            });
            var prevTrickMock1 = new Mock<ITrick>();
            prevTrickMock1.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N9]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.ACE])
            });
            prevTrickMock1.Setup(m => m.CardsPlayed).Returns(prevTrickMock1.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock1.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = curPlayerMock.Object,
                Points = 23
            });
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>());
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { prevTrickMock.Object, prevTrickMock1.Object, trickMock.Object });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE],
                CardRepository.Instance[StandardSuite.HEARTS, CardType.KING],
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7],
                CardRepository.Instance[StandardSuite.CLUBS, CardType.N10]
            });
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(trickMock.Object.OrderedMoves.Count() + 1);
            var playedCard = CardRepository.Instance[StandardSuite.HEARTS, CardType.KING];
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = true,
                LostOneTrick = false,
                CardMatchesSuit = true,
                MostPowerfulInTrick = true,
                OpponentPercentDone = 0,
                AvgVisibleCardPoints = 4,  //Card played plus previously played cards
                UnknownStrongerCards = 16 - 8, //There are 19 stronger cards. 1 held, 7 have been played.
                HeldStrongerCards = 1
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void LeasterKeyGenerator_GetKey_FirstPlayer()
        {
            var playedCard = CardRepository.Instance[StandardSuite.DIAMONDS, CardType.ACE];
            var curPlayerMock = new Mock<IPlayer>();
            var prevTrickMock = new Mock<ITrick>();
            prevTrickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N7]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N9]),
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.SPADES, CardType.ACE])
            });
            prevTrickMock.Setup(m => m.CardsPlayed).Returns(prevTrickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            prevTrickMock.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = curPlayerMock.Object,
                Points = 23
            });
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, playedCard),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.N10]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N8]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N9]),
                new KeyValuePair<IPlayer, ICard>(new Mock<IPlayer>().Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N10])
            });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Winner()).Returns(new TrickWinner()
            {
                Player = curPlayerMock.Object,
                Points = 23
            });
            trickMock.Setup(m => m.CardsPlayed).Returns(trickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            var nextTrickMock = new Mock<ITrick>();
            nextTrickMock.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE])
            });
            nextTrickMock.Setup(m => m.CardsPlayed).Returns(nextTrickMock.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            var nextTrickMock1 = new Mock<ITrick>();
            nextTrickMock1.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.HEARTS, CardType.KING])
            });
            nextTrickMock1.Setup(m => m.CardsPlayed).Returns(nextTrickMock1.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            var nextTrickMock2 = new Mock<ITrick>();
            nextTrickMock2.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N7])
            });
            nextTrickMock2.Setup(m => m.CardsPlayed).Returns(nextTrickMock2.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            var nextTrickMock3 = new Mock<ITrick>();
            nextTrickMock3.Setup(m => m.OrderedMoves).Returns(new List<KeyValuePair<IPlayer, ICard>>() {
                new KeyValuePair<IPlayer, ICard>(curPlayerMock.Object, CardRepository.Instance[StandardSuite.CLUBS, CardType.N10])
            });
            nextTrickMock3.Setup(m => m.CardsPlayed).Returns(nextTrickMock3.Object.OrderedMoves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            trickMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { prevTrickMock.Object, trickMock.Object, nextTrickMock.Object, nextTrickMock1.Object, nextTrickMock2.Object, nextTrickMock3.Object });
            curPlayerMock.Setup(m => m.Cards).Returns(new List<ICard>() { });
            curPlayerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(1);
            var expectedKey = new LeasterStatUniqueKey()
            {
                WonOneTrick = true,
                LostOneTrick = false,
                CardMatchesSuit = true,
                MostPowerfulInTrick = true,
                OpponentPercentDone = 0,
                AvgVisibleCardPoints = 11,  //Card played plus previously played cards
                UnknownStrongerCards = 8 - 1, //There are 8 stronger cards. 1 has been played.
                HeldStrongerCards = 0
            };

            var generator = new LeasterKeyGenerator();
            var actualKey = generator.GenerateKey(trickMock.Object, curPlayerMock.Object, playedCard);

            Assert.AreEqual(expectedKey, actualKey);
        }
    }
}