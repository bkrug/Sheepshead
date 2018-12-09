using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Model;
using Sheepshead.Model.Players;

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

        private ITrick MockTrickWinners(Mock<IPlayer> player, int points)
        {
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.Winner()).Returns(new TrickWinner() { Player = player.Object, Points = points });
            return trickMock.Object;
        }

        [TestMethod]
        public void Hand_Scores_5Player_DefenseLooseOneCoin()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.N10_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Partner).Returns(partnerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 21),
                MockTrickWinners(partnerMock, 12),
                MockTrickWinners(pickerMock, 14),
                MockTrickWinners(player1Mock, 17),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player3Mock, 28)
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

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
        public void Hand_Scores_5Player_DefenseLooseTwoCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.JACK_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Partner).Returns(partnerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 21),
                MockTrickWinners(partnerMock, 23),
                MockTrickWinners(pickerMock, 28),
                MockTrickWinners(partnerMock, 14),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player3Mock, 12)
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

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
        public void Hand_Scores_5Player_DefenseLooseThreeCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.JACK_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.KING_SPADES, SheepCard.N10_HEARTS });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Partner).Returns(partnerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 21),
                MockTrickWinners(partnerMock, 23),
                MockTrickWinners(pickerMock, 28),
                MockTrickWinners(partnerMock, 14),
                MockTrickWinners(pickerMock, 7),
                MockTrickWinners(pickerMock, 12),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

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
        public void Hand_Scores_5Player_DefenseWinsOneCoin()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_HEARTS });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Partner).Returns(partnerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 20),
                MockTrickWinners(pickerMock, 15),
                MockTrickWinners(partnerMock, 10),
                MockTrickWinners(player1Mock, 27),
                MockTrickWinners(player2Mock, 18),
                MockTrickWinners(player3Mock, 15),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

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
        public void Hand_Scores_5Player_DefenseWinsTwoCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.KING_CLUBS, SheepCard.ACE_HEARTS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.QUEEN_HEARTS, SheepCard.JACK_DIAMONDS });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Partner).Returns(partnerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 10),
                MockTrickWinners(partnerMock, 15),
                MockTrickWinners(player1Mock, 20),
                MockTrickWinners(player1Mock, 28),
                MockTrickWinners(player2Mock, 27),
                MockTrickWinners(player3Mock, 15),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

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
        public void Hand_Scores_5Player_DefenseWinsThreeCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_HEARTS, SheepCard.JACK_HEARTS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { partnerMock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_SPADES, SheepCard.ACE_HEARTS });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Partner).Returns(partnerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(player1Mock, 10),
                MockTrickWinners(player1Mock, 6),
                MockTrickWinners(player2Mock, 20),
                MockTrickWinners(player2Mock, 28),
                MockTrickWinners(player3Mock, 19),
                MockTrickWinners(player3Mock, 15),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(22, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.IsFalse(scores.Points.ContainsKey(partnerMock.Object));
            Assert.AreEqual(16, scores.Points[player1Mock.Object]);
            Assert.AreEqual(48, scores.Points[player2Mock.Object]);
            Assert.AreEqual(34, scores.Points[player3Mock.Object]);

            Assert.AreEqual(-9, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(0, scores.Coins[partnerMock.Object]);
            Assert.AreEqual(3, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(3, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(3, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_5Player_NoPartner()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            var player4Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player4Mock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.N10_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 21),
                MockTrickWinners(pickerMock, 12),
                MockTrickWinners(pickerMock, 14),
                MockTrickWinners(player1Mock, 17),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player3Mock, 28),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(21 + 12 + 14 + 21, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(17, scores.Points[player1Mock.Object]);
            Assert.AreEqual(7, scores.Points[player2Mock.Object]);
            Assert.AreEqual(28, scores.Points[player3Mock.Object]);
            Assert.IsFalse(scores.Points.ContainsKey(player4Mock.Object));

            Assert.AreEqual(4, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(-1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player3Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player4Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_5Player_NoPartner_PickerWinsNoTricks()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            var player4Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player4Mock.Object, player1Mock.Object, pickerMock.Object, player2Mock.Object, player3Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.N10_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(player1Mock, 12),
                MockTrickWinners(player1Mock, 17),
                MockTrickWinners(player2Mock, 21),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player3Mock, 28),
                MockTrickWinners(player4Mock, 14),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(21, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(29, scores.Points[player1Mock.Object]);
            Assert.AreEqual(28, scores.Points[player2Mock.Object]);
            Assert.AreEqual(28, scores.Points[player3Mock.Object]);
            Assert.AreEqual(14, scores.Points[player4Mock.Object]);

            Assert.AreEqual(-12, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(3, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(3, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(3, scores.Coins[player3Mock.Object]);
            Assert.AreEqual(3, scores.Coins[player4Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_3Player_DefenseWinsThreeCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(3);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player1Mock.Object, pickerMock.Object, player2Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_DIAMONDS });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(player1Mock, 15),
                MockTrickWinners(player1Mock, 16),
                MockTrickWinners(player1Mock, 7),
                MockTrickWinners(player1Mock, 4),
                MockTrickWinners(player1Mock, 13),
                MockTrickWinners(player2Mock, 4),
                MockTrickWinners(player2Mock, 14),
                MockTrickWinners(player2Mock, 4),
                MockTrickWinners(player2Mock, 11),
                MockTrickWinners(player2Mock, 10),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(22, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(55, scores.Points[player1Mock.Object]);
            Assert.AreEqual(43, scores.Points[player2Mock.Object]);

            Assert.AreEqual(-6, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(3, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(3, scores.Coins[player2Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_3Player_DefenseWinsOneCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(3);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player1Mock.Object, pickerMock.Object, player2Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.KING_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 11),
                MockTrickWinners(pickerMock, 22),
                MockTrickWinners(pickerMock, 10),
                MockTrickWinners(player1Mock, 4),
                MockTrickWinners(player1Mock, 4),
                MockTrickWinners(player1Mock, 16),
                MockTrickWinners(player1Mock, 7),
                MockTrickWinners(player2Mock, 14),
                MockTrickWinners(player2Mock, 13),
                MockTrickWinners(player2Mock, 4),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(58, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(31, scores.Points[player1Mock.Object]);
            Assert.AreEqual(31, scores.Points[player2Mock.Object]);

            Assert.AreEqual(-2, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(1, scores.Coins[player2Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_3Player_DefenseLoosesOneCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(3);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player1Mock.Object, pickerMock.Object, player2Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.KING_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 11),
                MockTrickWinners(pickerMock, 22),
                MockTrickWinners(pickerMock, 10),
                MockTrickWinners(pickerMock, 7),
                MockTrickWinners(player1Mock, 4),
                MockTrickWinners(player1Mock, 4),
                MockTrickWinners(player1Mock, 16),
                MockTrickWinners(player2Mock, 14),
                MockTrickWinners(player2Mock, 13),
                MockTrickWinners(player2Mock, 4),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(65, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.AreEqual(24, scores.Points[player1Mock.Object]);
            Assert.AreEqual(31, scores.Points[player2Mock.Object]);

            Assert.AreEqual(2, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(-1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player2Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_3Player_DefenseLoosesThreeCoins()
        {
            var deckMock = new Mock<IDeck>();
            var pickerMock = new Mock<IPlayer>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(3);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player1Mock.Object, pickerMock.Object, player2Mock.Object });
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.KING_SPADES });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Picker).Returns(pickerMock.Object);
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(pickerMock, 11),
                MockTrickWinners(pickerMock, 22),
                MockTrickWinners(pickerMock, 10),
                MockTrickWinners(pickerMock, 7),
                MockTrickWinners(pickerMock, 4),
                MockTrickWinners(pickerMock, 4),
                MockTrickWinners(pickerMock, 16),
                MockTrickWinners(pickerMock, 14),
                MockTrickWinners(pickerMock, 13),
                MockTrickWinners(pickerMock, 4),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(120, scores.Points[pickerMock.Object], "Picker recieves the blind");
            Assert.IsFalse(scores.Points.ContainsKey(player1Mock.Object));
            Assert.IsFalse(scores.Points.ContainsKey(player2Mock.Object));

            Assert.AreEqual(6, scores.Coins[pickerMock.Object]);
            Assert.AreEqual(-3, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-3, scores.Coins[player2Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_Leasters_5Player_WithoutBlind()
        {
            var deckMock = new Mock<IDeck>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            var player4Mock = new Mock<IPlayer>();
            var player5Mock = new Mock<IPlayer>();
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player4Mock.Object, player1Mock.Object, player5Mock.Object, player2Mock.Object, player3Mock.Object });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Leasters).Returns(true);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(player1Mock, 12),
                MockTrickWinners(player1Mock, 17),
                MockTrickWinners(player2Mock, 21),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player3Mock, 28),
                MockTrickWinners(player4Mock, 14),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(29, scores.Points[player1Mock.Object]);
            Assert.AreEqual(28, scores.Points[player2Mock.Object]);
            Assert.AreEqual(28, scores.Points[player3Mock.Object]);
            Assert.AreEqual(14, scores.Points[player4Mock.Object]);
            Assert.IsFalse(scores.Points.ContainsKey(player5Mock.Object));

            Assert.AreEqual(-1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player3Mock.Object]);
            Assert.AreEqual(4, scores.Coins[player4Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player5Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_Leasters_5Player_AllTricksToOnePlayer_WithoutBlind()
        {
            var deckMock = new Mock<IDeck>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            var player4Mock = new Mock<IPlayer>();
            var player5Mock = new Mock<IPlayer>();
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N8_CLUBS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player4Mock.Object, player1Mock.Object, player5Mock.Object, player2Mock.Object, player3Mock.Object });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Leasters).Returns(true);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(player2Mock, 12),
                MockTrickWinners(player2Mock, 17),
                MockTrickWinners(player2Mock, 21),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player2Mock, 28),
                MockTrickWinners(player2Mock, 14),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.IsFalse(scores.Points.ContainsKey(player1Mock.Object));
            Assert.AreEqual(12 + 17 + 21 + 7 + 28 + 14, scores.Points[player2Mock.Object]);
            Assert.IsFalse(scores.Points.ContainsKey(player3Mock.Object));
            Assert.IsFalse(scores.Points.ContainsKey(player4Mock.Object));
            Assert.IsFalse(scores.Points.ContainsKey(player5Mock.Object));

            Assert.AreEqual(-1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(4, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player3Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player4Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player5Mock.Object]);
        }

        [TestMethod]
        public void Hand_Scores_Leasters_3Player_WithoutBlind()
        {
            var deckMock = new Mock<IDeck>();
            var player1Mock = new Mock<IPlayer>();
            var player2Mock = new Mock<IPlayer>();
            var player3Mock = new Mock<IPlayer>();
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>() { SheepCard.N10_HEARTS, SheepCard.N7_CLUBS });
            deckMock.Setup(d => d.PlayerCount).Returns(3);
            deckMock.Setup(d => d.Players).Returns(new List<IPlayer>() { player1Mock.Object, player3Mock.Object, player2Mock.Object });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Deck).Returns(deckMock.Object);
            handMock.Setup(m => m.Leasters).Returns(true);
            handMock.Setup(m => m.Tricks).Returns(new List<ITrick>()
            {
                MockTrickWinners(player1Mock, 11),
                MockTrickWinners(player1Mock, 22),
                MockTrickWinners(player1Mock, 10),
                MockTrickWinners(player2Mock, 7),
                MockTrickWinners(player2Mock, 4),
                MockTrickWinners(player3Mock, 4),
                MockTrickWinners(player3Mock, 16),
                MockTrickWinners(player3Mock, 14),
                MockTrickWinners(player3Mock, 13),
                MockTrickWinners(player3Mock, 4),
            });

            var calculator = new ScoreCalculator(handMock.Object);
            var scores = calculator.InternalScores();

            Assert.AreEqual(11 + 22 + 10, scores.Points[player1Mock.Object]);
            Assert.AreEqual(7 + 4, scores.Points[player2Mock.Object]);
            Assert.AreEqual(4 + 16 + 14 + 13 + 4, scores.Points[player3Mock.Object]);

            Assert.AreEqual(-1, scores.Coins[player1Mock.Object]);
            Assert.AreEqual(2, scores.Coins[player2Mock.Object]);
            Assert.AreEqual(-1, scores.Coins[player3Mock.Object]);
        }

        [TestMethod]
        public void Hand_IsComplete()
        {
            var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            mockDeck.Setup(m => m.PlayerCount).Returns(5);
            var hand = new Hand(mockDeck.Object);

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

            hand = new Hand(mockDeck.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            hand.AddTrick(mockCompleteTrick.Object);
            Assert.IsTrue(hand.IsComplete(), "Hand is complete if there are enough tricks and the last is complete.");
        }

        //TODO: Split into two tests
        [TestMethod]
        public void Hand_Constructor_PartnerCard_PickerWithoutJackDiamonds()
        {
            {
                var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
                var mockDeck = new Mock<IDeck>();
                mockDeck.Setup(m => m.Blinds).Returns(blinds);
                mockDeck.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
                mockDeck.Setup(m => m.PlayerCount).Returns(5);
                var mockPicker = new Mock<IPlayer>();
                var originalPickerCards = new List<SheepCard>() { SheepCard.N7_SPADES, SheepCard.N8_SPADES, SheepCard.N9_SPADES, SheepCard.N10_SPADES };
                mockPicker.Setup(f => f.Cards).Returns(originalPickerCards);
                var droppedCards = new List<SheepCard>() { SheepCard.N7_SPADES, SheepCard.N8_SPADES };
                var mockHand = new Mock<IHand>();
                mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
                mockDeck.Setup(m => m.Buried).Returns(droppedCards);
                var partnerCard = HandUtils.ChoosePartnerCard(mockHand.Object, mockPicker.Object);
                HandUtils.BuryCards(mockHand.Object, mockPicker.Object, droppedCards);
                Assert.AreEqual(SheepCard.JACK_DIAMONDS, partnerCard, "Jack of diamonds should be partner card right now");
                var expectedPickerCards = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS, SheepCard.N9_SPADES, SheepCard.N10_SPADES };
                CollectionAssert.AreEquivalent(expectedPickerCards, mockPicker.Object.Cards, "Picker dropped some cards to pick the blinds.");
            }
        }

        [TestMethod]
        public void Hand_Constructor_PartnerCard_PickerHasJackDiamonds()
        {
            var blinds = new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.JACK_HEARTS };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            mockDeck.Setup(m => m.PlayerCount).Returns(5);
            var pickerCards = new List<SheepCard>() {
                SheepCard.JACK_SPADES, SheepCard.JACK_CLUBS, SheepCard.QUEEN_DIAMONDS, SheepCard.N7_CLUBS, SheepCard.QUEEN_SPADES, SheepCard.QUEEN_CLUBS
            };
            var droppedCards = new List<SheepCard>() { SheepCard.JACK_CLUBS, SheepCard.JACK_HEARTS };
            var mockPicker = new Mock<IPlayer>();
            mockPicker.Setup(m => m.Cards).Returns(pickerCards);
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
            mockDeck.Setup(m => m.Buried).Returns(droppedCards);
            var partnerCard = HandUtils.ChoosePartnerCard(mockHand.Object, mockPicker.Object);
            Assert.AreEqual(SheepCard.QUEEN_HEARTS, partnerCard, "Queen of hearts should be partner card right now");
        }

        [TestMethod]
        public void Hand_Constructor_PartnerCard_PickerHasAllQueensJacks()
        {
            var blinds = new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.JACK_HEARTS };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            mockDeck.Setup(m => m.PlayerCount).Returns(5);
            var pickerCards = new List<SheepCard>() {
                SheepCard.JACK_SPADES, SheepCard.JACK_CLUBS, SheepCard.QUEEN_DIAMONDS, SheepCard.QUEEN_HEARTS, SheepCard.QUEEN_SPADES, SheepCard.QUEEN_CLUBS
            };
            var buriedCards = new List<SheepCard>() { SheepCard.JACK_HEARTS, SheepCard.JACK_DIAMONDS };
            var mockPicker = new Mock<IPlayer>();
            mockPicker.Setup(m => m.Cards).Returns(pickerCards);
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
            mockDeck.Setup(m => m.Buried).Returns(buriedCards);
            var partnerCard = HandUtils.ChoosePartnerCard(mockHand.Object, mockPicker.Object);
            Assert.IsNull(partnerCard, "There should be no partner card.");
        }

        [TestMethod]
        public void Hand_Constructor_NoPartner_3Player()
        {
            var blinds = new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.ACE_CLUBS };
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Blinds).Returns(blinds);
            mockDeck.Setup(m => m.PlayerCount).Returns(3);
            var mockPicker = new Mock<IPlayer>();
            mockPicker.Setup(m => m.Cards).Returns(new List<SheepCard>() { SheepCard.JACK_DIAMONDS });
            var mockHand = new Mock<IHand>();
            mockHand.Setup(m => m.Deck).Returns(mockDeck.Object);
            mockDeck.Setup(m => m.Buried).Returns(new List<SheepCard>());
            var partnerCard = HandUtils.ChoosePartnerCard(mockHand.Object, mockPicker.Object);
            Assert.AreEqual(null, partnerCard, "No partner card should be selected since it is a three player game.");
        }

        [TestMethod]
        public void Hand_Leasters()
        {
            var deckMock = new Mock<IDeck>();
            var hand = new Hand(deckMock.Object);
            hand.SetPicker(null, new List<SheepCard>());
            Assert.IsTrue(hand.Leasters, "When there is no picker, play leasters.");

            var pickerMock = new Mock<IPlayer>();
            pickerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.CalledAce);
            var hand2 = new Hand(deckMock.Object);
            hand2.SetPicker(pickerMock.Object, new List<SheepCard>());
            Assert.IsFalse(hand2.Leasters, "When there is a picker, don't play leasters.");
        }

        [TestMethod]
        public void Hand_OnEndHand()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            var hand = new Hand(deckMock.Object);
            var endEventCalled = false;
            hand.OnHandEnd += (Object sender, EventArgs e) => {
                endEventCalled = true;
            };
            for (var i = 0; i < 6; ++i)
            {
                var trickMock = new Mock<ITrick>();
                hand.AddTrick(trickMock.Object);
                trickMock.Raise(x => x.OnTrickEnd += null, new EventArgs());
                if (i + 1 == 6)
                    Assert.IsTrue(endEventCalled, "When the last trick ended, so did the hand.");
                else
                    Assert.IsFalse(endEventCalled, "Hand End event should only be called when the last trick ended.");
            }
        }

        [TestMethod]
        public void Hand_PresumedPartner_BasedOnLead()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            var player1 = new Mock<IPlayer>();
            var player2 = new Mock<IPlayer>();
            var pickerMock = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var player5 = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            var hand = new Hand(deckMock.Object);
            hand.SetPicker(pickerMock.Object, new List<SheepCard>());
            var cardsPlayed1 = new Dictionary<IPlayer, SheepCard>()
            {
                { player4.Object, SheepCard.ACE_DIAMONDS  }
            };
            var cardsPlayed2 = new Dictionary<IPlayer, SheepCard>()
            {
                { player2.Object, SheepCard.N7_DIAMONDS  }
            };
            var cardsPlayed3 = new Dictionary<IPlayer, SheepCard>()
            {
                { player4.Object, SheepCard.N10_DIAMONDS }
            };
            var cardsPlayed4 = new Dictionary<IPlayer, SheepCard>()
            {
                { player5.Object, SheepCard.KING_SPADES }
            };
            var trick1Mock = new Mock<ITrick>();
            var trick2Mock = new Mock<ITrick>();
            var trick3Mock = new Mock<ITrick>();
            var trick4Mock = new Mock<ITrick>();
            trick1Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed1);
            trick2Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed2);
            trick3Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed3);
            trick4Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed4);
            hand.AddTrick(trick1Mock.Object);
            hand.AddTrick(trick2Mock.Object);
            hand.AddTrick(trick3Mock.Object);
            hand.AddTrick(trick4Mock.Object);
            Assert.AreEqual(player4.Object, hand.PresumedParnter, "Player4 led with trump more than any other player.");
        }

        [TestMethod]
        public void Hand_PresumedPartner_2PlayersTie()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(d => d.Blinds).Returns(new List<SheepCard>());
            deckMock.Setup(d => d.Buried).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            deckMock.Setup(d => d.PlayerCount).Returns(5);
            var player1 = new Mock<IPlayer>();
            var player2 = new Mock<IPlayer>();
            var pickerMock = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var player5 = new Mock<IPlayer>();
            pickerMock.Setup(p => p.Cards).Returns(new List<SheepCard>());
            var hand = new Hand(deckMock.Object);
            hand.SetPicker(pickerMock.Object, new List<SheepCard>());
            var cardsPlayed1 = new Dictionary<IPlayer, SheepCard>()
            {
                { player4.Object, SheepCard.ACE_DIAMONDS  }
            };
            var cardsPlayed2 = new Dictionary<IPlayer, SheepCard>()
            {
                { player2.Object, SheepCard.N7_DIAMONDS  }
            };
            var cardsPlayed3 = new Dictionary<IPlayer, SheepCard>()
            {
                { player5.Object, SheepCard.KING_HEARTS }
            };
            var cardsPlayed4 = new Dictionary<IPlayer, SheepCard>()
            {
                { player5.Object, SheepCard.KING_SPADES }
            };
            var trick1Mock = new Mock<ITrick>();
            var trick2Mock = new Mock<ITrick>();
            var trick3Mock = new Mock<ITrick>();
            var trick4Mock = new Mock<ITrick>();
            trick1Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed1);
            trick2Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed2);
            trick3Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed3);
            trick4Mock.Setup(m => m.CardsPlayed).Returns(cardsPlayed4);
            hand.AddTrick(trick1Mock.Object);
            hand.AddTrick(trick2Mock.Object);
            hand.AddTrick(trick3Mock.Object);
            hand.AddTrick(trick4Mock.Object);
            Assert.IsNull(hand.PresumedParnter, "Cannot guess at who the partner is.");
        }
    }
}
