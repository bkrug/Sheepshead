using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

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
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            var blinds = new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] };
            deckMock.Setup(m => m.Blinds).Returns(blinds);
            var hand = new Hand(deckMock.Object, picker, new List<ICard>());
            foreach (var mockTrick in trickMocks)
            {
                hand.AddTrick(mockTrick.Object);
            }
            trickMocks[2].Setup(m => m.QueueRankOfPartner).Returns(-1);
            hand.SetPartner(partner, trickMocks[2].Object);
            return hand;
        }

        //Notice that we don't account for the blinds here.  We don't need to.
        //The scores method is able to opperate by only counting up the defensive players' points.
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
        public void Hand_Scores_Leasters()
        {
            var scoreTests = new List<int[,]>()
            {
                new int[,] { { 1, 14 }, { 2, 30 }, { 1, 17 }, { 4, 40 }, { 5, 19 } },
                new int[,] { { 1, 11 }, { 2, 30 }, { 2, 30 }, { 4, 40 }, { 5, 9  } },
                new int[,] { { 2, 14 }, { 2, 30 }, { 2, 17 }, { 4, 40 }, { 4, 19 } },
                new int[,] { { 1, 20 }, { 2, 20 }, { 3, 30 }, { 4, 0  }, { 5, 50 } },
                new int[,] { { 2, 14 }, { 2, 30 }, { 2, 17 }, { 2, 40 }, { 2, 19 } }
            };
            var playerScores = new List<int[]>() 
            {
                new int[] { -1, -1, -1, -1, 4 },
                new int[] { -1, -1, -1, -1, 4 },
                new int[] { -1, -1, -1, 4, -1 },
                new int[] { -1, -1, -1, 4, -1 },
                new int[] { -1, 4, -1, -1, -1 }
            };
            for (var a = 0; a < scoreTests.Count; ++a)
            {
                var trickMocks = GetTrickMocks();
                var players = GetPlayers();
                PopulateTricks(ref trickMocks, players, scoreTests[a]);
                var hand = GetHand(trickMocks, players, null, null);
                var actualScores = hand.Scores();
                //Assert.AreEqual(0, actualScores.Sum(kvp => kvp.Value), "Player's scores add to zero.  (This is really a test of the test, not the code.) Running Test " + a);
                for (var b = 0; b < 5; ++b)
                {
                    var player = players[b];
                    Assert.AreEqual(playerScores[a][b], actualScores[player], "Matching player scores.  Running Test " + a + ", player " + (b + 1));
                }
            }
        }

        [TestMethod]
        public void Hand_IsComplete()
        {
            var blinds = new List<ICard>() { CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE] };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.PlayerCount).Returns(5);
            var hand = new Hand(mockDeck.Object, new NewbiePlayer(), new List<ICard>());

            var mockCompleteTrick = new Mock<ITrick>();
            var mockIncompleteTrick = new Mock<ITrick>();
            mockCompleteTrick.Setup(m => m.IsComplete()).Returns(true);
            mockIncompleteTrick.Setup(m => m.IsComplete()).Returns(false);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            Assert.IsFalse(hand.IsComplete(), "Hand is not complete if there are too few tricks.");

            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockIncompleteTrick.Object);
            Assert.IsFalse(hand.IsComplete(), "Hand is not complete if the last trick is not complete.");

            hand = new Hand(mockDeck.Object, new NewbiePlayer(), new List<ICard>());
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            Assert.IsTrue(hand.IsComplete(), "Hand is complete if there are enough tricks and the last is complete.");
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

        [TestMethod]
        public void Hand_Leasters()
        {
            var deckMock = new Mock<IDeck>();
            var hand = new Hand(deckMock.Object, null, null);
            Assert.IsTrue(hand.Leasters, "When there is no picker, play leasters.");

            var pickerMock = new Mock<IPlayer>();
            pickerMock.Setup(m => m.Cards).Returns(new List<ICard>());
            deckMock.Setup(m => m.Blinds).Returns(new List<ICard>());
            var hand2 = new Hand(deckMock.Object, pickerMock.Object, new List<ICard>());
            Assert.IsFalse(hand2.Leasters, "When there is a picker, don't play leasters.");
        }

        [TestMethod]
        public void Hand_OnAddTrick()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            var hand = new Hand(deckMock.Object, null, null);
            var addEventCalled = false;
            var endEventCalled = false;
            hand.OnAddTrick += (Object sender, EventArgs e) => { 
                addEventCalled = true;
            };
            hand.OnHandEnd += (Object sender, EventArgs e) => {
                endEventCalled = true;
            };
            for (var i = 0; i < 6; ++i)
            {
                addEventCalled = false;
                var trickMock = new Mock<ITrick>();
                hand.AddTrick(trickMock.Object);
                trickMock.Raise(x => x.OnTrickEnd += null, new EventArgs());
                Assert.IsTrue(addEventCalled, "event should be raised whenever a trick is added.");
                if (i + 1 == 6)
                    Assert.IsTrue(endEventCalled, "When the last trick ended, so did the hand.");
                else
                    Assert.IsFalse(endEventCalled, "Hand End event should only be called when the last trick ended.");
            }
        }

        [TestMethod]
        public void Hand_Summary()
        {
            var player1 = new Mock<IPlayer>();
            var player2 = new Mock<IPlayer>();
            var player3 = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var player5 = new Mock<IPlayer>();
            player2.Setup(m => m.Cards).Returns(new List<ICard>());
            var playerList = new List<IPlayer>() { player1.Object, player2.Object, player3.Object, player4.Object, player5.Object };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.StartingPlayer).Returns(player5.Object);
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            deckMock.Setup(m => m.Players).Returns(playerList);
            deckMock.Setup(m => m.Blinds).Returns(new List<ICard>() { new Card(StandardSuite.HEARTS, CardType.N7, 0, 0), new Card(StandardSuite.DIAMONDS, CardType.JACK, 0, 0) });
            deckMock.Setup(m => m.Buried).Returns(new List<ICard>() { new Card(StandardSuite.SPADES, CardType.ACE, 0, 0), new Card(StandardSuite.CLUBS, CardType.N10, 0, 0) });
            var hand = new Hand(deckMock.Object, player2.Object, deckMock.Object.Buried);
            GetTricks(playerList, hand);
            //Format:
            //List the two blinds first
            //List the picker and buried card. 2 implies the player who had second turn to pick and second turn in first trick
            //List cards for first trick.
            //List cards for the second trick.
            //List cards for the third trick.
            //List cards for the fourth trick.
            //List cards for the fifth trick.
            //If we say that player A is the player who had the first turn during the first trick, 
            //then player A's card is shown first for each of the following trick, even if a different player lead those turns.
            var expectedSummary = "7HJD,3ASTC,JDKC8H7SAH,AD9H8DQS8S,9CTHKSACJH,7HJSQDKH7C,KDTS7DJC9D,8CQH9SQCTD";
            Assert.AreEqual(expectedSummary, hand.Summary(), "Test output for normal hand.");

            deckMock.Setup(m => m.Buried).Returns(new List<ICard>() {  });
            var leastersHand = new Hand(deckMock.Object, null, null);
            GetTricks(playerList, leastersHand);
            var leastersSummary = "7HJD,,JDKC8H7SAH,AD9H8DQS8S,9CTHKSACJH,7HJSQDKH7C,KDTS7DJC9D,8CQH9SQCTD";
            Assert.AreEqual(leastersSummary, leastersHand.Summary(), "Test output for a leasters hand.");
        }

        private static void GetTricks(List<IPlayer> playerList, Hand hand)
        {
            GetTrick(hand, playerList, new List<ICard>()
            {
                new Card(StandardSuite.CLUBS, CardType.KING, 0, 0),
                new Card(StandardSuite.HEARTS, CardType.N8, 0, 0),
                new Card(StandardSuite.SPADES, CardType.N7, 0, 0),
                new Card(StandardSuite.HEARTS, CardType.ACE, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.JACK, 0, 0)
            });
            GetTrick(hand, playerList, new List<ICard>()
            {
                new Card(StandardSuite.HEARTS, CardType.N9, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.N8, 0, 0),
                new Card(StandardSuite.SPADES, CardType.QUEEN, 0, 0),
                new Card(StandardSuite.SPADES, CardType.N8, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.ACE, 0, 0)
            });
            GetTrick(hand, playerList, new List<ICard>()
            {
                new Card(StandardSuite.HEARTS, CardType.N10, 0, 0),
                new Card(StandardSuite.SPADES, CardType.KING, 0, 0),
                new Card(StandardSuite.CLUBS, CardType.ACE, 0, 0),
                new Card(StandardSuite.HEARTS, CardType.JACK, 0, 0),
                new Card(StandardSuite.CLUBS, CardType.N9, 0, 0)
            });
            GetTrick(hand, playerList, new List<ICard>()
            {
                new Card(StandardSuite.SPADES, CardType.JACK, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.QUEEN, 0, 0),
                new Card(StandardSuite.HEARTS, CardType.KING, 0, 0),
                new Card(StandardSuite.CLUBS, CardType.N7, 0, 0),
                new Card(StandardSuite.HEARTS, CardType.N7, 0, 0)
            });
            GetTrick(hand, playerList, new List<ICard>()
            {
                new Card(StandardSuite.SPADES, CardType.N10, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.N7, 0, 0),
                new Card(StandardSuite.CLUBS, CardType.JACK, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.N9, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.KING, 0, 0)
            });
            GetTrick(hand, playerList, new List<ICard>()
            {
                new Card(StandardSuite.HEARTS, CardType.QUEEN, 0, 0),
                new Card(StandardSuite.SPADES, CardType.N9, 0, 0),
                new Card(StandardSuite.CLUBS, CardType.QUEEN, 0, 0),
                new Card(StandardSuite.DIAMONDS, CardType.N10, 0, 0),
                new Card(StandardSuite.CLUBS, CardType.N8, 0, 0)
            });
        }

        private static void GetTrick(Hand hand, List<IPlayer> playerList, List<ICard> cards)
        {
            var trick = new Mock<ITrick>();
            var moves = new Dictionary<IPlayer, ICard>();
            for (var i = 0; i < 5; ++i)
                moves.Add(playerList[i], cards[i]);
            trick.Setup(m => m.CardsPlayed).Returns(moves);
            hand.AddTrick(trick.Object);
        }

        [TestMethod]
        public void Hand_PartnerCardPlayed()
        {
            var player1 = new Mock<IPlayer>();
            var player2 = new Mock<IPlayer>();
            var player3 = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var player5 = new Mock<IPlayer>();
            player3.Setup(m => m.Cards).Returns(new List<ICard>());
            var playerList = new List<IPlayer>() { player1.Object, player2.Object, player3.Object, player4.Object, player5.Object };
            var cards = new List<ICard>();
            for (var i = 0; i < 32; ++i) { cards.Add(new Mock<ICard>().Object); }
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.StartingPlayer).Returns(player2.Object);
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            deckMock.Setup(m => m.Players).Returns(playerList);
            deckMock.Setup(m => m.Blinds).Returns(new List<ICard>() { new Card(StandardSuite.HEARTS, CardType.N7, 0, 0), new Card(StandardSuite.DIAMONDS, CardType.JACK, 0, 0) });
            deckMock.Setup(m => m.Buried).Returns(new List<ICard>() { new Card(StandardSuite.SPADES, CardType.ACE, 0, 0), new Card(StandardSuite.CLUBS, CardType.N10, 0, 0) });

            var hand = new Hand(deckMock.Object, player3.Object, deckMock.Object.Buried);
            var trickMocks = new List<Mock<ITrick>>();
            for (var i = 0; i < 6; ++i)
            {
                trickMocks.Add(new Mock<ITrick>());
                hand.AddTrick(trickMocks.Last().Object);
            }
            trickMocks[2].Setup(m => m.QueueRankOfPartner).Returns(3);
            hand.SetPartner(playerList[3], hand.Tricks[2]);
            Assert.AreEqual(hand.PartnerCardPlayed[0], 2);
            Assert.AreEqual(hand.PartnerCardPlayed[1], 3);
        }
    }
}
