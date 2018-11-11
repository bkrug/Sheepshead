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
    public class AdvancedPlayerTests
    {
        #region Pick Tests
        //Tests:
        //* In five player game, you have many powerful trump. Pick.
        //* In five player game, you have many weak trump, and points you would like to bury. Pick.
        //* In five player game, you have many weak trump, but no points to bury, and are towards the front. Pass.
        //* In five player game, you have many weak trump, but no points to bury, but are towards the back. Pick.
        //* In five player game, you have few trump. Pass.
        //* In three player game, you have many trump and many aces or tens. Pick.
        //* In three player game, you have many trump but fewer aces and tens, and are towards the front. Pass.
        //* In three player game, you have not quite so many trump, aces, and tens, but are towards the back. Pick.
        //* In three player game, you just don't have much. Pass.

        [TestMethod]
        public void AdvancedPlayer_FivePlayerGame_ManyPowerfulTrump_ShouldPick()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.JACK_SPADES);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_FivePlayerGame_ManyWeakTrumpAndPointsToBury_ShouldPick()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.ACE_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.ACE_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_FivePlayerGame_ManyWeakTrumpFewPointsTowardsFront_ShouldPass()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N7_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N8_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_FivePlayerGame_ManyWeakTrumpFewPointsTowardsBack_ShouldPick()
        {
            var players = new List<IPlayer>() {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new AdvancedPlayer()
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N7_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N8_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_FivePlayerGame_FewTrump_ShouldPass()
        {
            var players = new List<IPlayer>() {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new AdvancedPlayer()
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.ACE_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.ACE_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N7_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.KING_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.KING_SPADES);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_ThreePlayerGame_ManyPowerfulTrump_ShouldPick()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.JACK_SPADES);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.ACE_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N10_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.ACE_SPADES);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_ThreePlayerGame_ManyWeakTrumpFewPointsTowardsFront_ShouldPass()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N7_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N8_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N7_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.KING_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N8_CLUBS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_ThreePlayerGame_ManyWeakTrumpFewPointsTowardsBack_ShouldPick()
        {
            var players = new List<IPlayer>() {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new AdvancedPlayer()
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N7_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N8_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N9_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N7_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.KING_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.ACE_DIAMONDS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_ThreePlayerGame_FewTrump_ShouldPass()
        {
            var players = new List<IPlayer>() {
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new AdvancedPlayer()
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.ACE_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.ACE_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N7_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.KING_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N8_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N7_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N9_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N8_CLUBS);
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Players).Returns(players);
            deckMock.Setup(m => m.PlayerCount).Returns(players.Count);
            deckMock.Setup(m => m.StartingPlayer).Returns(players[0]);
            var actual = advancedPlayer.WillPick(deckMock.Object);
            Assert.AreEqual(false, actual);
        }
        #endregion

        #region Leading a Trick Tests

        //* You lead the trick as an offensive player, but not all cards are Jacks or Queens, lead with weak trump.
        //* You lead the trick as an offensive player, almost all cards are Jacks or Queens, lead with a strong trump.
        //* You lead the trick as a defensive player, lead with the suit of called ace.
        //* You lead the trick as a defensive player, lead with non-trump that you have few of.

        [TestMethod]
        public void AdvancedPlayer_LeadTrick_OffensivePlayer_WeakCards_LeadWithWeakTrump1()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.JACK_SPADES);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.ACE_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N7_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.ACE_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N10_HEARTS);
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(t => t.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>());
            trickMock.Setup(t => t.StartingPlayer).Returns(advancedPlayer);
            trickMock.Setup(t => t.Hand.Leasters).Returns(false);
            trickMock.Setup(t => t.Hand.Picker).Returns(advancedPlayer);
            var actual = advancedPlayer.GetMove(trickMock.Object);
            Assert.IsTrue(CardUtil.GetSuit(actual) == Suit.TRUMP);
            Assert.AreNotEqual(SheepCard.JACK_SPADES, actual);
            Assert.AreNotEqual(SheepCard.QUEEN_CLUBS, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_LeadTrick_OffensivePlayer_WeakCards_LeadWithWeakTrump2()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.JACK_DIAMONDS); //This player is partner
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.ACE_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.N7_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.ACE_SPADES);
            advancedPlayer.Cards.Add(SheepCard.N10_HEARTS);
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(t => t.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>());
            trickMock.Setup(t => t.StartingPlayer).Returns(advancedPlayer);
            trickMock.Setup(t => t.Hand.Leasters).Returns(false);
            trickMock.Setup(t => t.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            var actual = advancedPlayer.GetMove(trickMock.Object);
            Assert.IsTrue(CardUtil.GetSuit(actual) == Suit.TRUMP);
            Assert.AreNotEqual(SheepCard.JACK_SPADES, actual);
            Assert.AreNotEqual(SheepCard.QUEEN_CLUBS, actual);
        }


        [TestMethod]
        public void AdvancedPlayer_LeadTrick_OffensivePlayer_StrongCards_LeadWithStrongTrump1()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.JACK_SPADES);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.ACE_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.ACE_SPADES);
            advancedPlayer.Cards.Add(SheepCard.QUEEN_SPADES);
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(t => t.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>());
            trickMock.Setup(t => t.StartingPlayer).Returns(advancedPlayer);
            trickMock.Setup(t => t.Hand.Leasters).Returns(false);
            trickMock.Setup(t => t.Hand.Picker).Returns(advancedPlayer);
            var actual = advancedPlayer.GetMove(trickMock.Object);
            Assert.IsTrue(new List<SheepCard>() { SheepCard.QUEEN_CLUBS, SheepCard.QUEEN_HEARTS, SheepCard.QUEEN_SPADES }.Contains(actual));
        }

        [TestMethod]
        public void AdvancedPlayer_LeadTrick_DefensivePlayer_LeadWithSuitOfCalledAce()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N7_SPADES);
            advancedPlayer.Cards.Add(SheepCard.ACE_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.KING_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.JACK_HEARTS);
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(t => t.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>());
            trickMock.Setup(t => t.StartingPlayer).Returns(advancedPlayer);
            trickMock.Setup(t => t.Hand.Leasters).Returns(false);
            trickMock.Setup(t => t.Hand.Deck.Game.PartnerMethod).Returns(PartnerMethod.CalledAce);
            trickMock.Setup(t => t.Hand.PartnerCard).Returns(SheepCard.ACE_SPADES);
            var actual = advancedPlayer.GetMove(trickMock.Object);
            Assert.AreEqual(SheepCard.N7_SPADES, actual);
        }

        [TestMethod]
        public void AdvancedPlayer_LeadTrick_DefensivePlayer_LeadWithNonTrumpThatYouHaveFewOf()
        {
            var players = new List<IPlayer>() {
                new AdvancedPlayer(),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var advancedPlayer = players.OfType<AdvancedPlayer>().Single();
            advancedPlayer.Cards.Add(SheepCard.N10_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.N7_SPADES);
            advancedPlayer.Cards.Add(SheepCard.ACE_CLUBS);
            advancedPlayer.Cards.Add(SheepCard.KING_DIAMONDS);
            advancedPlayer.Cards.Add(SheepCard.KING_HEARTS);
            advancedPlayer.Cards.Add(SheepCard.JACK_HEARTS);
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(t => t.CardsPlayed).Returns(new Dictionary<IPlayer, SheepCard>());
            trickMock.Setup(t => t.StartingPlayer).Returns(advancedPlayer);
            trickMock.Setup(t => t.Hand.Leasters).Returns(false);
            trickMock.Setup(t => t.Hand.Deck.Game.PartnerMethod).Returns(PartnerMethod.JackOfDiamonds);
            trickMock.Setup(t => t.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            var actual = advancedPlayer.GetMove(trickMock.Object);
            Assert.IsTrue(!new List<SheepCard>() { SheepCard.KING_DIAMONDS, SheepCard.JACK_HEARTS }.Contains(actual));
        }
        #endregion

        #region Mid-Trick Tests (Not all opponents have played)

        #endregion

        //* In leasters, you have a lot of high value cards, so give them up in hands that someone else one.
        //* In leasters, you have more limited high value cards, give them up in hands one by someone with many points.
    }
}
