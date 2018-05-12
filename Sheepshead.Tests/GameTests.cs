using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;
using Sheepshead.Tests.PlayerMocks;
using Sheepshead.Model;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameTests
    {
        private class ExposeGame : Game {
            public ExposeGame() : base (new List<IPlayer>(), new RandomWrapper(), null, null)
            {

            }

            public new List<IPlayer> Players
            {
                get { return _players; }
                set { _players = value; }
            }
        }

        [TestMethod]
        public void Game_CanCountAllPlayersInGame()
        {
            var exposedGame = new ExposeGame();
            var game = (Game)exposedGame;
            var mockPlayer1 = new Mock<IPlayer>();
            var mockPlayer2 = new Mock<IPlayer>();
            var mockPlayer3 = new Mock<IPlayer>();
            exposedGame.Players = new List<IPlayer>() { mockPlayer1.Object, mockPlayer2.Object, mockPlayer3.Object };
            Assert.AreEqual(exposedGame.Players.Count, game.PlayerCount, "Returned correct number of players");
        }

        [TestMethod]
        public void Game_PlayNonHuman_PlayCard()
        {
            var playerList = new List<IPlayer>() {
                new ComputerPlayerReportingPlays(SheepCard.JACK_DIAMONDS),
                new ComputerPlayerReportingPlays(SheepCard.N7_CLUBS),
                new ComputerPlayerReportingPlays(SheepCard.N9_HEARTS),
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerReportingPlays(SheepCard.QUEEN_SPADES)
            };
            var trickMock = new Mock<ITrick>();
            var moveList = new List<SheepCard>();
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(playerList);
            trickMock
                .Setup(m => m.Add(It.IsAny<IPlayer>(), It.IsAny<SheepCard>()))
                .Callback((IPlayer player, SheepCard card) => { moveList.Add(card); });
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);
            var playersDifferentOrder = playerList.Skip(2).Union(playerList.Take(2)).ToList();

            var game = new Game(playersDifferentOrder, null, null, gameStateDescriberMock.Object);
            game.PlayNonHumansInTrick();

            Assert.IsTrue(((ComputerPlayerReportingPlays)playerList[0]).MadeMove);
            Assert.IsTrue(((ComputerPlayerReportingPlays)playerList[1]).MadeMove);
            Assert.IsTrue(((ComputerPlayerReportingPlays)playerList[2]).MadeMove);
            Assert.IsFalse(((ComputerPlayerReportingPlays)playerList[4]).MadeMove);
            Assert.AreEqual(3, moveList.Count);
            Assert.AreEqual(moveList[0], SheepCard.JACK_DIAMONDS);
            Assert.AreEqual(moveList[1], SheepCard.N7_CLUBS);
            Assert.AreEqual(moveList[2], SheepCard.N9_HEARTS);
        }

        [TestMethod]
        public void Game_PlayNonHuman_LastPlayerIsNotHuman()
        {
            var players = new List<IPlayer>() {
                new ComputerPlayerReportingPlays(SheepCard.JACK_HEARTS),
                new ComputerPlayerReportingPlays(SheepCard.KING_HEARTS),
                new ComputerPlayerReportingPlays(SheepCard.N10_CLUBS),
                new ComputerPlayerReportingPlays(SheepCard.N7_HEARTS)
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players);
            var playersDifferentOrder = players.Skip(2).Union(players.Take(2)).ToList();
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);

            var game = new Game(playersDifferentOrder, null, null, gameStateDescriberMock.Object);
            game.PlayNonHumansInTrick();

            Assert.IsTrue(players.OfType<ComputerPlayerReportingPlays>().All(p => p.MadeMove), "All players have played.");
        }

        [TestMethod]
        public void Game_PlayNonHuman_HumanHasNextTurn()
        {
            var players = new List<IPlayer>() {
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerReportingPlays(SheepCard.JACK_HEARTS),
                new ComputerPlayerReportingPlays(SheepCard.KING_HEARTS),
                new ComputerPlayerReportingPlays(SheepCard.N10_CLUBS),
                new ComputerPlayerReportingPlays(SheepCard.N7_HEARTS)
            };
            var playersDifferentOrder = players.Skip(2).Union(players.Take(2)).ToList();
            var trickMock = new Mock<ITrick>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { trickMock.Object });
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);

            var game = new Game(playersDifferentOrder, null, null, gameStateDescriberMock.Object);
            game.PlayNonHumansInTrick();

            Assert.IsTrue(players.OfType<ComputerPlayerReportingPlays>().All(p => !p.MadeMove), "Got this far without playing a computer player's turn.");
        }

        [TestMethod]
        public void Game_PlayNonHumanPickTurns_StopAtHuman()
        {
            var players = new List<IPlayer>() {
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerPickingMock(true)
            };
            var refusingPick = players.Take(1).ToList();
            var unplayedPlayers = players.Skip(1).ToList();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentDeck).Returns(deckMock.Object);
            gameStateDescriberMock.Setup(m => m.GetTurnType()).Returns(TurnType.Pick);
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<SheepCard>>()))
                .Callback(() => Assert.Fail("Should not have attempted to create a hand"));

            var game = new Game(players, null, handFactoryMock.Object, gameStateDescriberMock.Object);
            var picker = game.PlayNonHumanPickTurns();

            Assert.IsNull(picker, "Picker should be null because the computer players didn't pick and we didn't ask the second human yet.");
        }

        [TestMethod]
        public void Game_PlayNonHumanPickTurns_FindAPicker()
        {
            var playerList = new List<IPlayer>() {
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(true),
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = playerList.Skip(1).ToList();
            var expectedPicker = playerList[2] as IComputerPlayer;
            var refusingPick = playerList.Take(1).ToList();
            var handCreated = false;
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock
                .Setup(m => m.GetHand(It.IsAny<IDeck>(), expectedPicker, It.IsAny<List<SheepCard>>()))
                .Callback(() => handCreated = true);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentDeck).Returns(deckMock.Object);
            gameStateDescriberMock.Setup(m => m.GetTurnType()).Returns(TurnType.Pick);

            var game = new Game(playerList, null, handFactoryMock.Object, gameStateDescriberMock.Object);
            var picker = game.PlayNonHumanPickTurns();

            Assert.IsTrue(handCreated);
            Assert.AreEqual(expectedPicker, picker);
        }

        [TestMethod]
        public void Game_PlayNonHumanPickTurns_LeastersGame()
        {
            var playerList = new List<IPlayer>() {
                new Mock<IHumanPlayer>().Object,
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false),
                new ComputerPlayerPickingMock(false)
            };
            var unplayedPlayers = playerList.Skip(1).ToList();
            var refusingPick = playerList.Take(1).ToList();
            var handCreated = false;
            var deckMock = new Mock<IDeck>();
            deckMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            deckMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPick);
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock.Setup(m => m.GetHand(deckMock.Object, null, It.IsAny<List<SheepCard>>()))
                .Callback(() => handCreated = true)
                .Returns(() => new Mock<IHand>().Object);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentDeck).Returns(deckMock.Object);
            gameStateDescriberMock.Setup(m => m.GetTurnType()).Returns(TurnType.Pick);

            var game = new Game(playerList, null, handFactoryMock.Object, gameStateDescriberMock.Object);
            var picker = game.PlayNonHumanPickTurns();

            Assert.IsTrue(handCreated);
            Assert.IsNull(picker);
        }

        private bool PlayerListsMatch(List<IPlayer> list1, List<IPlayer> list2)
        {
            var match = true;
            var tempList = list1;
            foreach (var item in list2)
            {
                if (tempList.Contains(item))
                    tempList.Remove(item);
                else
                    match = false;
            }
            return match && !tempList.Any();
        }
    }
}
