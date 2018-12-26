using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Model;
using Sheepshead.Model.Players;
using Sheepshead.Logic.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void SimplePlayer_GetMove()
        {
            var mainPlayer = new SimplePlayer(new Participant());
            var card1 = SheepCard.JACK_CLUBS;
            var card2 = SheepCard.N8_HEARTS;
            new List<SheepCard>() { card1, card2 }.ForEach(c => mainPlayer.AddCard(c));
            var playerList = new List<IPlayer>() { new MockPlayer(), new MockPlayer(), mainPlayer, new MockPlayer(), new MockPlayer() };
            {
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.IsLegalAddition(card1, mainPlayer)).Returns(true);
                trickMock.Setup(m => m.IsLegalAddition(card2, mainPlayer)).Returns(false);
                var cardPlayed = mainPlayer.GetMove(trickMock.Object);
                Assert.AreEqual(card1, cardPlayed, "Since card1 is legal, that is the card simple player will play");
            }
            {
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.IsLegalAddition(card1, mainPlayer)).Returns(false);
                trickMock.Setup(m => m.IsLegalAddition(card2, mainPlayer)).Returns(true);
                var cardPlayed = mainPlayer.GetMove(trickMock.Object);
                Assert.AreEqual(card2, cardPlayed, "Since card2 is legal, that is the card simple player will play");
            }
        }

        private Mock<ITrick> GenerateTrickMock(List<IPlayer> playerList)
        {
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            trickMock.Setup(m => m.Players).Returns(playerList);
            return trickMock;
        }

        private Mock<IHand> GenerateHandMock(List<IPlayer> playerList)
        {
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PlayerCount).Returns(5);
            handMock.Setup(m => m.Players).Returns(playerList);
            return handMock;
        }

        private IPlayer player1 = new MockPlayer();
        private IPlayer player2 = new MockPlayer();
        private IPlayer player3 = new MockPlayer();
        private IPlayer player4 = new MockPlayer();

        [TestMethod]
        public void SimplePlayer_WillPick()
        {
            var mainPlayer = new SimplePlayer(new Participant());
            {
                var playerList = new List<IPlayer>() { player1, player2, mainPlayer, player3, player4 };
                var handMock = GenerateHandMock(playerList);
                handMock.Setup(m => m.StartingPlayer).Returns(player2);
                Assert.IsFalse(mainPlayer.WillPick(handMock.Object), "Simple Player should not pick if he is not last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, mainPlayer, player3, player4 };
                var handMock = GenerateHandMock(playerList);
                handMock.Setup(m => m.StartingPlayer).Returns(player3);
                Assert.IsTrue(mainPlayer.WillPick(handMock.Object), "Simple Player should pick if he is last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, player3, player4, mainPlayer };
                var handMock = GenerateHandMock(playerList);
                handMock.Setup(m => m.StartingPlayer).Returns(player1);
                Assert.IsTrue(mainPlayer.WillPick(handMock.Object), "Simple Player should pick if he is last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, player3, player4, mainPlayer };
                var handMock = GenerateHandMock(playerList);
                handMock.Setup(m => m.StartingPlayer).Returns(player3);
                Assert.IsFalse(mainPlayer.WillPick(handMock.Object), "Simple Player should not pick if he is not last.");
            }
        }

        [TestMethod]
        public void Intermediate_DropCards()
        {
            var picker = new IntermediatePlayer(new Participant());
            picker.AddCardRange(new List<SheepCard>()
            {
                SheepCard.N7_DIAMONDS,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.JACK_CLUBS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_HEARTS,
                SheepCard.N8_HEARTS
            });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { 
                SheepCard.N8_SPADES,
                SheepCard.QUEEN_HEARTS
            });
            var cardsToDrop = picker.DropCardsForPick(handMock.Object);
            Assert.IsTrue(cardsToDrop.Contains(SheepCard.N7_CLUBS), "Drop 7 of Clubs since it is only club.");
            Assert.IsTrue(cardsToDrop.Contains(SheepCard.N8_SPADES), "Drop 8 of Spades since it is only club.");
        }

        [TestMethod]
        public void Intermediate_ChooseCalledAce()
        {
            var picker = new IntermediatePlayer(new Participant());
            picker.AddCardRange(new List<SheepCard>()
            {
                SheepCard.N7_DIAMONDS,
                SheepCard.QUEEN_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.N10_HEARTS,
                SheepCard.N7_HEARTS,
                SheepCard.N8_HEARTS
            });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() {
                SheepCard.N7_CLUBS,
                SheepCard.QUEEN_HEARTS
            });
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());

            var partnerCard = picker.ChooseCalledAce(handMock.Object);

            Assert.IsTrue(partnerCard == SheepCard.ACE_CLUBS || partnerCard == SheepCard.ACE_HEARTS, "Partner card cannot be ace of spaces since picker has no spade.");
        }

        [TestMethod]
        public void IntermediatePlayer_ChooseCalledAce_GetNothing()
        {
            var picker = new IntermediatePlayer(new Participant());
            picker.AddCardRange(new List<SheepCard>()
            {
                SheepCard.ACE_CLUBS,
                SheepCard.QUEEN_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_HEARTS,
                SheepCard.N8_HEARTS
            });
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() {
                SheepCard.ACE_HEARTS,
                SheepCard.QUEEN_HEARTS
            });
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());

            var partnerCard = picker.ChooseCalledAce(handMock.Object);

            Assert.IsNull(partnerCard, "No ace can be called as the partner card at this point.");
        }

        [TestMethod]
        public void Player_LegalCalledAces_TwoAcesAreLegal()
        {
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.N9_CLUBS, SheepCard.N9_SPADES });
            var player = new HumanPlayer(new Participant());
            player.AddCardRange(new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.N8_CLUBS, SheepCard.N8_SPADES, SheepCard.N8_HEARTS, SheepCard.ACE_HEARTS, SheepCard.QUEEN_HEARTS });

            var actual = player.LegalCalledAces(handMock.Object);

            CollectionAssert.AreEquivalent(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_SPADES }, actual);
        }

        [TestMethod]
        public void Player_LegalCalledAces_TwoTensAreLegal()
        {
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.N10_SPADES });
            var player = new HumanPlayer(new Participant());
            player.AddCardRange(new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.N8_CLUBS, SheepCard.N8_SPADES, SheepCard.N8_HEARTS, SheepCard.ACE_HEARTS, SheepCard.ACE_SPADES });

            var actual = player.LegalCalledAces(handMock.Object);

            CollectionAssert.AreEquivalent(new List<SheepCard>() { SheepCard.N10_CLUBS, SheepCard.N10_HEARTS }, actual);
        }

        [TestMethod]
        public void Player_LegalCalledAces_TwoNothingIsLegal()
        {
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            handMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.QUEEN_CLUBS, SheepCard.JACK_SPADES });
            var player = new HumanPlayer(new Participant());
            player.AddCardRange(new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.N8_CLUBS, SheepCard.ACE_CLUBS, SheepCard.N9_CLUBS, SheepCard.QUEEN_HEARTS, SheepCard.QUEEN_DIAMONDS });

            var actual = player.LegalCalledAces(handMock.Object);

            CollectionAssert.AreEquivalent(new List<SheepCard>(), actual);
        }

        [TestMethod]
        public void Player_QueueRankInHand()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant())
            };
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.Players).Returns(players);
            handMock.Setup(m => m.PlayerCount).Returns(players.Count);
            handMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            Assert.AreEqual(1, players[0].QueueRankInHand(handMock.Object));
            Assert.AreEqual(2, players[1].QueueRankInHand(handMock.Object));
            Assert.AreEqual(3, players[2].QueueRankInHand(handMock.Object));
            Assert.AreEqual(4, players[3].QueueRankInHand(handMock.Object));
            Assert.AreEqual(5, players[4].QueueRankInHand(handMock.Object));
        }

        [TestMethod]
        public void Player_QueueRankInTrick()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant()),
                new AdvancedPlayer(new Participant())
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.Players).Returns(players);
            trickMock.Setup(m => m.PlayerCount).Returns(players.Count);
            trickMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            Assert.AreEqual(1, players[0].QueueRankInTrick(trickMock.Object));
            Assert.AreEqual(2, players[1].QueueRankInTrick(trickMock.Object));
            Assert.AreEqual(3, players[2].QueueRankInTrick(trickMock.Object));
            Assert.AreEqual(4, players[3].QueueRankInTrick(trickMock.Object));
            Assert.AreEqual(5, players[4].QueueRankInTrick(trickMock.Object));
        }
    }
}
