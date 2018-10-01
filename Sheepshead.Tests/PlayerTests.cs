using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Tests.PlayerMocks;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void NewbiePlayer_GetMove()
        {
            var mainPlayer = new NewbiePlayer();
            var card1 = SheepCard.JACK_CLUBS;
            var card2 = SheepCard.N8_HEARTS;
            mainPlayer.Cards.AddRange(new List<SheepCard>() { card1, card2 });
            var playerList = new List<IPlayer>() { new Player(), new Player(), mainPlayer, new Player(), new Player() };
            {
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.IsLegalAddition(card1, mainPlayer)).Returns(true);
                trickMock.Setup(m => m.IsLegalAddition(card2, mainPlayer)).Returns(false);
                var cardPlayed = mainPlayer.GetMove(trickMock.Object);
                Assert.AreEqual(card1, cardPlayed, "Since card1 is legal, that is the card newbie player will play");
            }
            {
                var trickMock = GenerateTrickMock(playerList);
                trickMock.Setup(m => m.IsLegalAddition(card1, mainPlayer)).Returns(false);
                trickMock.Setup(m => m.IsLegalAddition(card2, mainPlayer)).Returns(true);
                var cardPlayed = mainPlayer.GetMove(trickMock.Object);
                Assert.AreEqual(card2, cardPlayed, "Since card2 is legal, that is the card newbie player will play");
            }
        }

        private Mock<ITrick> GenerateTrickMock(List<IPlayer> playerList)
        {
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            trickMock.Setup(m => m.Players).Returns(playerList);
            return trickMock;
        }

        private Mock<IDeck> GenerateDeckMock(List<IPlayer> playerList)
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayerCount).Returns(5);
            deckMock.Setup(m => m.Players).Returns(playerList);
            return deckMock;
        }

        private IPlayer player1 = new Player();
        private IPlayer player2 = new Player();
        private IPlayer player3 = new Player();
        private IPlayer player4 = new Player();

        [TestMethod]
        public void NewbiePlayer_WillPick()
        {
            var mainPlayer = new NewbiePlayer();
            {
                var playerList = new List<IPlayer>() { player1, player2, mainPlayer, player3, player4 };
                var deckMock = GenerateDeckMock(playerList);
                deckMock.Setup(m => m.StartingPlayer).Returns(player2);
                Assert.IsFalse(mainPlayer.WillPick(deckMock.Object), "Newbie Player should not pick if he is not last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, mainPlayer, player3, player4 };
                var deckMock = GenerateDeckMock(playerList);
                deckMock.Setup(m => m.StartingPlayer).Returns(player3);
                Assert.IsTrue(mainPlayer.WillPick(deckMock.Object), "Newbie Player should pick if he is last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, player3, player4, mainPlayer };
                var deckMock = GenerateDeckMock(playerList);
                deckMock.Setup(m => m.StartingPlayer).Returns(player1);
                Assert.IsTrue(mainPlayer.WillPick(deckMock.Object), "Newbie Player should pick if he is last.");
            }
            {
                var playerList = new List<IPlayer>() { player1, player2, player3, player4, mainPlayer };
                var deckMock = GenerateDeckMock(playerList);
                deckMock.Setup(m => m.StartingPlayer).Returns(player3);
                Assert.IsFalse(mainPlayer.WillPick(deckMock.Object), "Newbie Player should not pick if he is not last.");
            }
        }

        [TestMethod]
        public void NewbiePlayer_DropCards()
        {
            var picker = new BasicPlayer();
            picker.Cards.AddRange(new List<SheepCard>()
            {
                SheepCard.N7_DIAMONDS,
                SheepCard.QUEEN_DIAMONDS,
                SheepCard.JACK_CLUBS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_HEARTS,
                SheepCard.N8_HEARTS
            });
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { 
                SheepCard.N8_SPADES,
                SheepCard.QUEEN_HEARTS
            });
            var cardsToDrop = picker.DropCardsForPick(deckMock.Object);
            Assert.IsTrue(cardsToDrop.Contains(SheepCard.N7_CLUBS), "Drop 7 of Clubs since it is only club.");
            Assert.IsTrue(cardsToDrop.Contains(SheepCard.N8_SPADES), "Drop 8 of Spades since it is only club.");
        }

        [TestMethod]
        public void NewbiePlayer_ChooseCalledAce()
        {
            var picker = new BasicPlayer();
            picker.Cards.AddRange(new List<SheepCard>()
            {
                SheepCard.N7_DIAMONDS,
                SheepCard.QUEEN_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.N10_HEARTS,
                SheepCard.N7_HEARTS,
                SheepCard.N8_HEARTS
            });
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() {
                SheepCard.N7_CLUBS,
                SheepCard.QUEEN_HEARTS
            });
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>());

            var partnerCard = picker.ChooseCalledAce(deckMock.Object);

            Assert.IsTrue(partnerCard == SheepCard.ACE_CLUBS || partnerCard == SheepCard.ACE_HEARTS, "Partner card cannot be ace of spaces since picker has no spade.");
        }

        [TestMethod]
        public void NewbiePlayer_ChooseCalledAce_GetNothing()
        {
            var picker = new BasicPlayer();
            picker.Cards.AddRange(new List<SheepCard>()
            {
                SheepCard.ACE_CLUBS,
                SheepCard.QUEEN_SPADES,
                SheepCard.JACK_CLUBS,
                SheepCard.N7_CLUBS,
                SheepCard.N7_HEARTS,
                SheepCard.N8_HEARTS
            });
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() {
                SheepCard.ACE_HEARTS,
                SheepCard.QUEEN_HEARTS
            });
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>());

            var partnerCard = picker.ChooseCalledAce(deckMock.Object);

            Assert.IsNull(partnerCard, "No ace can be called as the partner card at this point.");
        }

        [TestMethod]
        public void Player_LegalCalledAces_TwoAcesAreLegal()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.N9_CLUBS, SheepCard.N9_SPADES });
            var player = new HumanPlayer();
            player.Cards.AddRange(new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.N8_CLUBS, SheepCard.N8_SPADES, SheepCard.N8_HEARTS, SheepCard.ACE_HEARTS, SheepCard.QUEEN_HEARTS });

            var actual = player.LegalCalledAces(deckMock.Object);

            CollectionAssert.AreEquivalent(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.ACE_SPADES }, actual);
        }

        [TestMethod]
        public void Player_LegalCalledAces_TwoTensAreLegal()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.ACE_CLUBS, SheepCard.N10_SPADES });
            var player = new HumanPlayer();
            player.Cards.AddRange(new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.N8_CLUBS, SheepCard.N8_SPADES, SheepCard.N8_HEARTS, SheepCard.ACE_HEARTS, SheepCard.ACE_SPADES });

            var actual = player.LegalCalledAces(deckMock.Object);

            CollectionAssert.AreEquivalent(new List<SheepCard>() { SheepCard.N10_CLUBS, SheepCard.N10_HEARTS }, actual);
        }

        [TestMethod]
        public void Player_LegalCalledAces_TwoNothingIsLegal()
        {
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Buried).Returns(new List<SheepCard>());
            deckMock.Setup(m => m.Blinds).Returns(new List<SheepCard>() { SheepCard.QUEEN_CLUBS, SheepCard.JACK_SPADES });
            var player = new HumanPlayer();
            player.Cards.AddRange(new List<SheepCard>() { SheepCard.JACK_DIAMONDS, SheepCard.N8_CLUBS, SheepCard.ACE_CLUBS, SheepCard.N9_CLUBS, SheepCard.QUEEN_HEARTS, SheepCard.QUEEN_DIAMONDS });

            var actual = player.LegalCalledAces(deckMock.Object);

            CollectionAssert.AreEquivalent(new List<SheepCard>(), actual);
        }

        [TestMethod]
        public void Player_QueueRankInDeck()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new AdvancedPlayer(),
                new AdvancedPlayer(),
                new AdvancedPlayer(),
                new AdvancedPlayer()
            };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            Assert.AreEqual(1, players[0].QueueRankInDeck(deckMock.Object));
            Assert.AreEqual(2, players[1].QueueRankInDeck(deckMock.Object));
            Assert.AreEqual(3, players[2].QueueRankInDeck(deckMock.Object));
            Assert.AreEqual(4, players[3].QueueRankInDeck(deckMock.Object));
            Assert.AreEqual(5, players[4].QueueRankInDeck(deckMock.Object));
        }

        [TestMethod]
        public void Player_QueueRankInTrick()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new AdvancedPlayer(),
                new AdvancedPlayer(),
                new AdvancedPlayer(),
                new AdvancedPlayer()
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
