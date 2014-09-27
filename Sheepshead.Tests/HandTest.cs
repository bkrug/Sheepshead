using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Models;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Tests
{
    [TestClass]
    public class HandTest
    {
        private List<IPlayer> GetPlayers()
        {
            var list = new List<IPlayer>();
            for (var a = 0; a < 5; ++a)
            {
                var mockPlayer = new Mock<IPlayer>();
                list.Add(mockPlayer.Object);
                mockPlayer.Setup(m => m.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.SPADES, CardType.N7] });
            }
            return list;
        }

        private List<Mock<ITrick>> GetTrickMocks()
        {
            var list = new List<Mock<ITrick>>();
            for (var a = 0; a < 5; ++a)
                list.Add(new Mock<ITrick>());
            return list;
        }

        private void PopulateTricks(ref List<Mock<ITrick>> trickMocks, List<IPlayer> players, int[,] scores)
        {
            for (var a = 0; a < 5; ++a)
                trickMocks[a].Setup(m => m.Winner()).Returns(new TrickWinner()
                {
                    Player = players[scores[a, 0] - 1],
                    Points = scores[a, 1]
                });
        }

        private IHand GetHand(List<Mock<ITrick>> trickMocks, List<IPlayer> players, IPlayer picker, IPlayer partner)
        {
            var gameMock = new Mock<IGame>();
            gameMock.Setup(m => m.Players).Returns(players);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Game).Returns(gameMock.Object);
            var blinds = new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] };
            deckMock.Setup(m => m.Blinds).Returns(blinds);
            var hand = new Hand(deckMock.Object, picker, new List<ICard>());
            foreach (var mockTrick in trickMocks)
            {
                hand.AddTrick(mockTrick.Object);
            }
            hand.Partner = partner;
            return hand;
        }

        [TestMethod]
        public void Hand_Scores()
        {
            var scoreTests = new List<int[,]>()
            {
                new int[,] { { 1, 14 }, { 2, 30 }, { 1, 17 }, { 4, 40 }, { 5, 19 } },
                new int[,] { { 1, 11 }, { 2, 30 }, { 2, 30 }, { 4, 40 }, { 5, 9  } },
                new int[,] { { 2, 14 }, { 2, 30 }, { 2, 17 }, { 4, 40 }, { 4, 19 } },
                new int[,] { { 1, 20 }, { 2, 20 }, { 3, 30 }, { 4, 0  }, { 5, 50 } }
            };
            var playerScores = new List<int []>() 
            {
                new int[] { -1, 2, -1, 1, -1 },
                new int[] { -2, 4, -2, 2, -2 },
                new int[] { -3, 6, -3, 3, -3 },
                new int[] { 2, -4, 2, -2, 2 }
            };
            for (var a = 0; a < scoreTests.Count; ++a)
            {
                var trickMocks = GetTrickMocks();
                var players = GetPlayers();
                PopulateTricks(ref trickMocks, players, scoreTests[a]);
                var hand = GetHand(trickMocks, players, players[1], players[3]);
                var actualScores = hand.Scores();
                //Assert.AreEqual(0, actualScores.Sum(kvp => kvp.Value), "Player's scores add to zero.  (This is really a test of the test, not the code.) Running Test " + a);
                for (var b = 0; b < 5; ++b)
                {
                    var player = players[b];
                    Assert.AreEqual(playerScores[a][b], actualScores[player], "Matching player scores.  Running Test " + a + ", player " + (b+1));
                }
            }
        }

        [TestMethod]
        public void Hand_IsComplete()
        {
            var mockDeck = new Mock<IDeck>();
            var blinds = new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] };
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            var mockGame = new Mock<IGame>();
            var mockPlayerList = new List<Mock<IPlayer>>();
            for (var i = 0; i < 5; ++i)
            {
                var mockPlayer = new Mock<IPlayer>();
                mockPlayerList.Add(mockPlayer);
                mockPlayer.Setup(m => m.Cards).Returns(new List<ICard>() { new Card(), new Card(), new Card(), new Card(), new Card() });
            }
            mockDeck.Setup(m => m.Game).Returns(mockGame.Object);
            mockGame.Setup(m => m.Players).Returns(mockPlayerList.Select(m => m.Object).ToList());
            var hand = new Hand(mockDeck.Object, mockPlayerList.First().Object, new List<ICard>());
            Assert.IsFalse(hand.IsComplete(), "Hand is not complete if players have cards in their hands");
            for (var i = 0; i < 5; ++i)
                mockPlayerList.ElementAt(i).Setup(m => m.Cards).Returns(new List<ICard>() { new Card() });
            Assert.IsFalse(hand.IsComplete(), "Hand is not complete if players have cards in their hands");
            mockPlayerList.ElementAt(0).Setup(m => m.Cards).Returns(new List<ICard>());
            mockPlayerList.ElementAt(1).Setup(m => m.Cards).Returns(new List<ICard>());
            Assert.IsFalse(hand.IsComplete(), "Hand is not complete if players have cards in their hands");
            for (var i = 0; i < 5; ++i)
                mockPlayerList.ElementAt(i).Setup(m => m.Cards).Returns(new List<ICard>());
            Assert.IsTrue(hand.IsComplete(), "Hand is complete when no players have cards in their hands.");
        }

        [TestMethod]
        public void Hand_Constructor()
        {
            var blinds = new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] };
            {
                var mockDeck = new Mock<IDeck>();
                mockDeck.Setup(m => m.Blinds).Returns(blinds);
                var mockPicker = new Mock<IPlayer>();
                var originalPickerCards = new List<ICard>() { CardRepository.Instance[StandardSuite.SPADES, CardType.N7], CardRepository.Instance[StandardSuite.SPADES, CardType.N8], CardRepository.Instance[StandardSuite.SPADES, CardType.N9], CardRepository.Instance[StandardSuite.SPADES, CardType.N10] };
                mockPicker.Setup(f => f.Cards).Returns(originalPickerCards);
                var droppedCards = new List<ICard>() { CardRepository.Instance[StandardSuite.SPADES, CardType.N7], CardRepository.Instance[StandardSuite.SPADES, CardType.N8]};
                var hand = new Hand(mockDeck.Object, mockPicker.Object, droppedCards);
                Assert.AreEqual(hand.PartnerCard, CardRepository.Instance[StandardSuite.DIAMONDS, CardType.JACK], "Jack of diamonds should be partner card right now");
                var expectedPickerCards = new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE], CardRepository.Instance[StandardSuite.SPADES, CardType.N9], CardRepository.Instance[StandardSuite.SPADES, CardType.N10] };
                Assert.IsTrue(SameContents(expectedPickerCards, mockPicker.Object.Cards), "Picker dropped some cards to pick the blinds.");
            }
            {
                var mockDeck = new Mock<IDeck>();
                mockDeck.Setup(m => m.Blinds).Returns(blinds);
                var mockPicker = new Mock<IPlayer>();
                mockPicker.Setup(m => m.Cards).Returns(new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.JACK] });
                var hand = new Hand(mockDeck.Object, mockPicker.Object, new List<ICard>());
                Assert.AreEqual(hand.PartnerCard, CardRepository.Instance[StandardSuite.HEARTS, CardType.JACK], "Jack of diamonds should be partner card right now");
            }
        }

        private bool SameContents(List<ICard> list1, List<ICard> list2)
        {
            var tempList = list1.ToList();
            var match = true;
            foreach (var item in list1)
            {
                if (tempList.Contains(item))
                    tempList.Remove(item);
                else
                    match = false;
            }
            return !tempList.Any() && match;
        }
    }
}
