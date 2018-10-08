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

        //* You lead the trick as an offensive player, but not all cards are Jacks or Queens, lead with weak trump.
        //* You lead the trick as an offensive player, almost all cards are Jacks or Queens, lead with a strong trump.
        //* You lead the trick as a defensive player, lead with the suit of called ace.

        //* Not all your opponents have played, your team is winning with the most powerful remaining card. Give away points.
        //* Not all your opponents have played, your team is winning. Give away points.
        //* Not all your opponents have played, your team is loosing. There are points to be had. You have the most powerful remaining card. Play that card.
        //* Not all your opponents have played, your team is loosing. There are points to be had. You have not the most powerful remaining card. Give away (fewest possible) points.
        //* Not all your opponents have played. Your team is loosing. There are not many points. Give away (fewest possible) points.

        //* All your opponents have played, your team is winning the trick. Give away points if you can.
        //* All your opponents have played, your team is not winning. There are points to be won. Play a card that wins the trick.
        //* All your opponents have played, your team is not winning. You can't make them win. Give away (fewest possible) points.
        //* All your opponents have played, your team is not winning. You can't make them win. Give away something worthless.

        //* You think all your opponents have played, your team is winning the trick. Give away points if you can.
        //* You think all your opponents have played, your team is not winning. There are points to be won. Play a card that wins the trick.
        //* You think all your opponents have played, your team is not winning. You can't make them win. Give away (fewest possible) points.
        //* You think all your opponents have played, your team is not winning. You can't make them win. Give away something worthless.

        //* In leasters, you have a lot of high value cards, so give them up in hands that someone else one.
        //* In leasters, you have more limited high value cards, give them up in hands one by someone with many points.
    }
}
