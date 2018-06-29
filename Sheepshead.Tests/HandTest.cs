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
                mockPlayer.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.N7_SPADES });
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
            var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
            deckMock.Setup(m => m.Blinds).Returns(blinds);
            var hand = new Hand(deckMock.Object, picker, new List<SheepCard>());
            foreach (var mockTrick in trickMocks)
            {
                hand.AddTrick(mockTrick.Object);
            }
            trickMocks[2].Setup(m => m.QueueRankOfPartner).Returns(-1);
            hand.SetPartner(partner, trickMocks[2].Object);
            return hand;
        }

        [TestMethod]
        public void Hand_Scores_Leasters1()
        {
            var scoreTests = new List<int[,]>()
            {
                new int[,] { { 1, 14 }, { 2, 30 }, { 1, 17 }, { 4, 40 }, { 5, 19 } },
                new int[,] { { 1, 11 }, { 2, 30 }, { 2, 30 }, { 4, 40 }, { 5, 9  } },
                new int[,] { { 2, 14 }, { 2, 30 }, { 2, 17 }, { 4, 40 }, { 4, 19 } },
                new int[,] { { 1, 20 }, { 2, 20 }, { 3, 30 }, { 4, 0  }, { 5, 50 } },
                new int[,] { { 2, 14 }, { 2, 30 }, { 2, 17 }, { 2, 40 }, { 2, 19 } }
            };
            var expectedCoins = new List<int[]>() 
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
                var actualCoins = hand.Scores().Coins;
                //Assert.AreEqual(0, actualScores.Sum(kvp => kvp.Value), "Player's scores add to zero.  (This is really a test of the test, not the code.) Running Test " + a);
                for (var b = 0; b < 5; ++b)
                {
                    var player = players[b];
                    Assert.AreEqual(expectedCoins[a][b], actualCoins[player], "Matching player scores.  Running Test " + a + ", player " + (b + 1));
                }
            }
        }

        private void MockTrickWinners(IHand hand, Mock<IPlayer> player, int points)
        {
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.Winner()).Returns(new TrickWinner() { Player = player.Object, Points = points });
            hand.AddTrick(trickMock.Object);
        }

        [TestMethod]
        public void Hand_Scores_JackDiamondsPartner_DefenseLooseOneCoin()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.N10_SPADES });

            var hand = new Hand(deckMock.Object, pickerMock.Object, deckMock.Object.Buried);
            hand.SetPartner(partnerMock.Object, null);
            MockTrickWinners(hand, pickerMock, 21);
            MockTrickWinners(hand, partnerMock, 12);
            MockTrickWinners(hand, pickerMock, 14);
            MockTrickWinners(hand, player1Mock, 17);
            MockTrickWinners(hand, player2Mock, 7);
            MockTrickWinners(hand, player3Mock, 28);

            var scores = hand.Scores();

            Assert.AreEqual(21 + 14 + 21, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(12, scores.Points[partnerMock.Object]);
            Assert.AreEqual(17, scores.Points[player1Mock.Object]);
            Assert.AreEqual(7, scores.Points[player2Mock.Object]);
            Assert.AreEqual(28, scores.Points[player3Mock.Object]);

            Assert.AreEqual(2, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(1, scores.Coins[partnerMock.Object]);
            Assert.AreEqual(-1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_JackDiamondsPartner_DefenseLooseTwoCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.JACK_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_SPADES });

            var hand = new Hand(deckMock.Object, pickerMock.Object, deckMock.Object.Buried);
            hand.SetPartner(partnerMock.Object, null);
            MockTrickWinners(hand, pickerMock, 21);
            MockTrickWinners(hand, partnerMock, 23);
            MockTrickWinners(hand, pickerMock, 28);
            MockTrickWinners(hand, partnerMock, 14);
            MockTrickWinners(hand, player2Mock, 7);
            MockTrickWinners(hand, player3Mock, 12);

            var scores = hand.Scores();

            Assert.AreEqual(21 + 28 + 15, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(23 + 14, scores.Points[partnerMock.Object]);
            Assert.IsFalse(scores.Points.ContainsKey(player1Mock.Object));
            Assert.AreEqual(7, scores.Points[player2Mock.Object]);
            Assert.AreEqual(12, scores.Points[player3Mock.Object]);

            Assert.AreEqual(4, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(2, scores.Coins[partnerMock.Object]);
            Assert.AreEqual(-2, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-2, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-2, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_JackDiamondsPartner_DefenseLooseThreeCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.JACK_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.KING_SPADES, SheepCard.N10_HEARTS });

            var hand = new Hand(deckMock.Object, pickerMock.Object, deckMock.Object.Buried);
            hand.SetPartner(partnerMock.Object, null);
            MockTrickWinners(hand, pickerMock, 21);
            MockTrickWinners(hand, partnerMock, 23);
            MockTrickWinners(hand, pickerMock, 28);
            MockTrickWinners(hand, partnerMock, 14);
            MockTrickWinners(hand, pickerMock, 7);
            MockTrickWinners(hand, pickerMock, 12);

            var scores = hand.Scores();

            Assert.AreEqual(21 + 28 + 7 + 12 + 14, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(23 + 14, scores.Points[partnerMock.Object]);
            Assert.IsFalse(scores.Points.ContainsKey(player1Mock.Object));
            Assert.IsFalse(scores.Points.ContainsKey(player2Mock.Object));
            Assert.IsFalse(scores.Points.ContainsKey(player3Mock.Object));

            Assert.AreEqual(6, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(3, scores.Coins[partnerMock.Object]);
            Assert.AreEqual(-3, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-3, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-3, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_JackDiamondsPartner_DefenseWinsOneCoin()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_HEARTS });

            var hand = new Hand(deckMock.Object, pickerMock.Object, deckMock.Object.Buried);
            hand.SetPartner(partnerMock.Object, null);
            MockTrickWinners(hand, pickerMock, 20);
            MockTrickWinners(hand, pickerMock, 15);
            MockTrickWinners(hand, partnerMock, 10);
            MockTrickWinners(hand, player1Mock, 27);
            MockTrickWinners(hand, player2Mock, 18);
            MockTrickWinners(hand, player3Mock, 15);

            var scores = hand.Scores();

            Assert.AreEqual(20 + 15 + 15, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(10, scores.Points[partnerMock.Object]);
            Assert.AreEqual(27, scores.Points[player1Mock.Object]);
            Assert.AreEqual(18, scores.Points[player2Mock.Object]);
            Assert.AreEqual(15, scores.Points[player3Mock.Object]);

            Assert.AreEqual(-2, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(-1, scores.Coins[partnerMock.Object]);
            Assert.AreEqual(1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(1, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(1, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_JackDiamondsPartner_DefenseWinsTwoCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.QUEEN_HEARTS, SheepCard.JACK_DIAMONDS });

            var hand = new Hand(deckMock.Object, pickerMock.Object, deckMock.Object.Buried);
            hand.SetPartner(partnerMock.Object, null);
            MockTrickWinners(hand, pickerMock, 10);
            MockTrickWinners(hand, partnerMock, 15);
            MockTrickWinners(hand, player1Mock, 20);
            MockTrickWinners(hand, player1Mock, 28);
            MockTrickWinners(hand, player2Mock, 27);
            MockTrickWinners(hand, player3Mock, 15);

            var scores = hand.Scores();

            Assert.AreEqual(10 + 5, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(15, scores.Points[partnerMock.Object]);
            Assert.AreEqual(48, scores.Points[player1Mock.Object]);
            Assert.AreEqual(27, scores.Points[player2Mock.Object]);
            Assert.AreEqual(15, scores.Points[player3Mock.Object]);

            Assert.AreEqual(-4, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(-2, scores.Coins[partnerMock.Object]);
            Assert.AreEqual(2, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(2, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(2, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_JackDiamondsPartner_DefenseWinsThreeCoins()
        {

        }

        [TestMethod]
        public void Hand_Scores_CalledAcePartner()
        {

        }

        [TestMethod]
        public void Hand_Scores_NoPartner()
        {

        }

        [TestMethod]
        public void Hand_Scores_Leasters_WithoutBlind()
        {
            
        }

        [TestMethod]
        public void Hand_Scores_Leasters_WithBlind()
        {

        }

        [TestMethod]
        public void Hand_IsComplete()
        {
            var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.PlayerCount).Returns(5);
            var hand = new Hand(mockDeck.Object, new NewbiePlayer(), new List<SheepCard>());

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

            hand = new Hand(mockDeck.Object, new NewbiePlayer(), new List<SheepCard>());
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
            var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
            {
                var mockDeck = new Mock<IDeck>();
                mockDeck.Setup(m => m.Blinds).Returns(blinds);
                mockDeck.Setup(m => m.PlayerCount).Returns(5);
                var mockPicker = new Mock<IPlayer>();
                var originalPickerCards = new List<SheepCard>() { SheepCard.N7_SPADES, SheepCard.N8_SPADES, SheepCard.N9_SPADES, SheepCard.N10_SPADES };
                mockPicker.Setup(f => f.Cards).Returns(originalPickerCards);
                var droppedCards = new List<SheepCard>() { SheepCard.N7_SPADES, SheepCard.N8_SPADES};
                var hand = new Hand(mockDeck.Object, mockPicker.Object, droppedCards);
                Assert.AreEqual(SheepCard.JACK_DIAMONDS, hand.PartnerCard, "Jack of diamonds should be partner card right now");
                var expectedPickerCards = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS, SheepCard.N9_SPADES, SheepCard.N10_SPADES };
                Assert.IsTrue(SameContents(expectedPickerCards, mockPicker.Object.Cards), "Picker dropped some cards to pick the blinds.");
            }
            {
                var mockDeck = new Mock<IDeck>();
                mockDeck.Setup(m => m.Blinds).Returns(blinds);
                mockDeck.Setup(m => m.PlayerCount).Returns(5);
                var mockPicker = new Mock<IPlayer>();
                mockPicker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.JACK_DIAMONDS });
                var hand = new Hand(mockDeck.Object, mockPicker.Object, new List<SheepCard>());
                Assert.AreEqual(SheepCard.JACK_HEARTS, hand.PartnerCard, "Jack of diamonds should be partner card right now");
            }
        }

        [TestMethod]
        public void Hand_Constructor_NoPartner()
        {
            var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.PlayerCount).Returns(3);
            var mockPicker = new Mock<IPlayer>();
            mockPicker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.JACK_DIAMONDS });
            var hand = new Hand(mockDeck.Object, mockPicker.Object, new List<SheepCard>());
            Assert.AreEqual(null, hand.PartnerCard, "No partner card should be selected since it is a three player game.");
        }

        private bool SameContents(List<SheepCard> list1, List<SheepCard> list2)
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
            pickerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>());
            var hand2 = new Hand(deckMock.Object, pickerMock.Object, new List<SheepCard>());
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
            player2.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var playerList = new List<IPlayer>() { player1.Object, player2.Object, player3.Object, player4.Object, player5.Object };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.StartingPlayer).Returns(player5.Object);
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            deckMock.Setup(m => m.Players).Returns(playerList);
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.JACK_DIAMONDS });
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_SPADES, SheepCard.N10_CLUBS });
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

            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>() {  });
            var leastersHand = new Hand(deckMock.Object, null, null);
            GetTricks(playerList, leastersHand);
            var leastersSummary = "7HJD,,JDKC8H7SAH,AD9H8DQS8S,9CTHKSACJH,7HJSQDKH7C,KDTS7DJC9D,8CQH9SQCTD";
            Assert.AreEqual(leastersSummary, leastersHand.Summary(), "Test output for a leasters hand.");
        }

        private static void GetTricks(List<IPlayer> playerList, Hand hand)
        {
            GetTrick(hand, playerList, new List<SheepCard>()
            {
                SheepCard.KING_CLUBS,
                SheepCard.N8_HEARTS,
                SheepCard.N7_SPADES,
                SheepCard.ACE_HEARTS,
                SheepCard.JACK_DIAMONDS
            });
            GetTrick(hand, playerList, new List<SheepCard>()
            {
                SheepCard.N9_HEARTS,
                SheepCard.N8_DIAMONDS,
                SheepCard.QUEEN_SPADES,
                SheepCard.N8_SPADES,
                SheepCard.ACE_DIAMONDS
            });
            GetTrick(hand, playerList, new List<SheepCard>()
            {
                SheepCard.N10_HEARTS,
                SheepCard.KING_SPADES,
                SheepCard.ACE_CLUBS,
                SheepCard.JACK_HEARTS,
                SheepCard.N9_CLUBS
            });
            GetTrick(hand, playerList, new List<SheepCard>()
            {
                SheepCard.JACK_SPADES,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.KING_HEARTS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_HEARTS
            });
            GetTrick(hand, playerList, new List<SheepCard>()
            {
                SheepCard.N10_SPADES,
                SheepCard.N7_DIAMONDS,
                SheepCard.JACK_CLUBS,
                SheepCard.N9_DIAMONDS,
                SheepCard.KING_DIAMONDS
            });
            GetTrick(hand, playerList, new List<SheepCard>()
            {
                SheepCard.QUEEN_HEARTS,
                SheepCard.N9_SPADES,
                SheepCard.QUEEN_CLUBS,
                SheepCard.N10_DIAMONDS,
                SheepCard.N8_CLUBS
            });
        }

        private static void GetTrick(Hand hand, List<IPlayer> playerList, List<SheepCard> cards)
        {
            var trick = new Mock<ITrick>();
            var moves = new Dictionary<IPlayer, SheepCard>();
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
            player3.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var playerList = new List<IPlayer>() { player1.Object, player2.Object, player3.Object, player4.Object, player5.Object };
            var cards = new List<SheepCard>();
            for (var i = 0; i < 32; ++i) { cards.Add(0); }
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.StartingPlayer).Returns(player2.Object);
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            deckMock.Setup(m => m.Players).Returns(playerList);
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.N7_HEARTS, SheepCard.JACK_DIAMONDS });
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_SPADES, SheepCard.N10_CLUBS });

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
