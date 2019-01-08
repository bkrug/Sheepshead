using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Logic;
using Sheepshead.Logic.Players;
using Sheepshead.Tests.PlayerMocks;
using Sheepshead.Logic.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void Game_CanCountAllPlayersInGame()
        {
            var mockPlayer1 = new Mock<IPlayer>();
            var mockPlayer2 = new Mock<IPlayer>();
            var mockPlayer3 = new Mock<IPlayer>();
            var game = new Game(new List<IPlayer>() { mockPlayer1.Object, mockPlayer2.Object, mockPlayer3.Object }, PartnerMethod.JackOfDiamonds, true);
            Assert.AreEqual(3, game.PlayerCount, "Returned correct number of players");
            Assert.AreEqual(3, game.Players.Count, "Returned correct number of players");
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

            var game = new Game(playersDifferentOrder, PartnerMethod.JackOfDiamonds, null, gameStateDescriberMock.Object);
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

            var game = new Game(playersDifferentOrder, PartnerMethod.JackOfDiamonds, null, gameStateDescriberMock.Object);
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
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.ITricks).Returns(new List<ITrick>() { trickMock.Object });
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);

            var game = new Game(playersDifferentOrder, PartnerMethod.JackOfDiamonds, null, gameStateDescriberMock.Object);
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
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            handMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentHand).Returns(handMock.Object);
            gameStateDescriberMock.Setup(m => m.GetTurnType()).Returns(TurnType.Pick);
            
            var game = new Game(players, PartnerMethod.JackOfDiamonds, null, gameStateDescriberMock.Object);
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
            var handMock = new Mock<IHand>();
            handMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            handMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            handMock.Setup(m => m.IGame.PartnerMethodEnum);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentHand).Returns(handMock.Object);
            gameStateDescriberMock.Setup(m => m.GetTurnType()).Returns(TurnType.Pick);

            var game = new Game(playerList, PartnerMethod.JackOfDiamonds, null, gameStateDescriberMock.Object);
            var picker = game.PlayNonHumanPickTurns();

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
            var handMock = new Mock<IHand>();
            handMock.SetupGet(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            handMock.SetupGet(m => m.PlayersRefusingPick).Returns(refusingPick);
            handMock.SetupGet(m => m.IGame.LeastersEnabled).Returns(true);
            var gameStateDescriberMock = new Mock<IGameStateDescriber>();
            gameStateDescriberMock.Setup(m => m.CurrentHand).Returns(handMock.Object);
            gameStateDescriberMock.Setup(m => m.GetTurnType()).Returns(TurnType.Pick);

            var game = new Game(playerList, PartnerMethod.JackOfDiamonds, null, gameStateDescriberMock.Object);
            var picker = game.PlayNonHumanPickTurns();

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

        [TestMethod]
        public void Game_UnassignedPlayers_ReturnsOnlyHumans()
        {
            var human1 = new Mock<IHumanPlayer>();
            var human2 = new Mock<IHumanPlayer>();
            var game = new Game(new List<IPlayer>()
            {
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object,
                human1.Object,
                new Mock<IPlayer>().Object,
                human2.Object
            }, PartnerMethod.JackOfDiamonds, true);
            Assert.AreEqual(2, game.UnassignedPlayers.Count);
            Assert.IsTrue(game.UnassignedPlayers.Contains(human1.Object));
            Assert.IsTrue(game.UnassignedPlayers.Contains(human2.Object));
        }

        [TestMethod]
        public void Game_UnassignedPlayers_ReturnsOnlyUnassignedHumans()
        {
            var human1 = new Mock<IHumanPlayer>();
            var human2 = new Mock<IHumanPlayer>();
            human2.Setup(m => m.AssignedToClient).Returns(true);
            var game = new Game(new List<IPlayer>()
            {
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object,
                human1.Object,
                new Mock<IPlayer>().Object,
                human2.Object
            }, PartnerMethod.JackOfDiamonds, true);
            Assert.AreEqual(1, game.UnassignedPlayers.Count);
            Assert.IsTrue(game.UnassignedPlayers.Contains(human1.Object));
        }

        [TestMethod]
        public void Game_RecordTurn_IllegalMove()
        {
            var humanPlayerMock = new Mock<IHumanPlayer>();
            humanPlayerMock.Setup(m => m.Cards).Returns(new List<SheepCard>()
            {
                SheepCard.N7_CLUBS, SheepCard.N7_HEARTS, SheepCard.KING_HEARTS
            });
            var players = new List<IPlayer> {
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object,
                humanPlayerMock.Object,
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players.Skip(2).ToList());
            trickMock.Setup(m => m.IsLegalAddition(SheepCard.N7_CLUBS, (IHumanPlayer)players[2])).Returns(false);
            var gamestateDescriberMock = new Mock<IGameStateDescriber>();
            gamestateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);

            var game = new Game(players, PartnerMethod.JackOfDiamonds, null, gamestateDescriberMock.Object);
            try
            {
                game.RecordTurn((IHumanPlayer)players[2], SheepCard.N7_CLUBS);
                Assert.Fail("An exception should have been thrown because the move was illegal");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("card", ex.ParamName, "Argument exception should have been thrown because that move was not legal.");
            }
        }

        [TestMethod]
        public void Game_RecordTurn_DoesNotHaveCard()
        {
            var humanPlayerMock = new Mock<IHumanPlayer>();
            humanPlayerMock.Setup(m => m.Cards).Returns(new List<SheepCard>()
            {
                SheepCard.N7_CLUBS, SheepCard.N7_HEARTS, SheepCard.KING_HEARTS
            });
            var players = new List<IPlayer> {
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object,
                humanPlayerMock.Object,
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players.Skip(2).ToList());
            trickMock.Setup(m => m.CardsByPlayer).Returns(new Dictionary<IPlayer, SheepCard>()
            {
                { players[0], SheepCard.ACE_HEARTS },
                { players[1], SheepCard.N7_SPADES }
            });
            var gamestateDescriberMock = new Mock<IGameStateDescriber>();
            gamestateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);

            var game = new Game(players, PartnerMethod.JackOfDiamonds, null, gamestateDescriberMock.Object);
            try
            {
                game.RecordTurn((IHumanPlayer)players[2], SheepCard.N8_HEARTS);
                Assert.Fail("An exception should have been thrown because player does not have this card.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("card", ex.ParamName, "Argument exception should have been thrown because player does not have this card.");
                Assert.IsTrue(ex.Message.StartsWith("Player does not have this card"));
            }
        }

        [TestMethod]
        public void Game_RecordTurn_NotPlayersTurn()
        {
            var humanPlayerMock = new Mock<IHumanPlayer>();
            humanPlayerMock.Setup(m => m.Cards).Returns(new List<SheepCard>()
            {
                SheepCard.N7_CLUBS, SheepCard.N7_HEARTS, SheepCard.KING_HEARTS
            });
            var players = new List<IPlayer> {
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object,
                humanPlayerMock.Object,
                new Mock<IPlayer>().Object,
                new Mock<IPlayer>().Object
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players.Skip(1).ToList());
            trickMock.Setup(m => m.CardsByPlayer).Returns(new Dictionary<IPlayer, SheepCard>()
            {
                { players[0], SheepCard.ACE_HEARTS }
            });
            var gamestateDescriberMock = new Mock<IGameStateDescriber>();
            gamestateDescriberMock.Setup(m => m.CurrentTrick).Returns(trickMock.Object);

            var game = new Game(players, PartnerMethod.JackOfDiamonds, null, gamestateDescriberMock.Object);
            try
            {
                game.RecordTurn((IHumanPlayer)players[2], SheepCard.N7_HEARTS);
                Assert.Fail("An exception should have been thrown because it is not the player's turn.");
            }
            catch (NotPlayersTurnException)
            {
                Assert.IsTrue(true, "Not Players turn.");
            }
        }
    }
}
