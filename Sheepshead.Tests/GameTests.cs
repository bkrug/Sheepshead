using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;
using Sheepshead.Tests.PlayerMocks;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameTests
    {
        private class ExposeGame : Game {
            public ExposeGame() : base (0, new List<IPlayer>(), new RandomWrapper(), null)
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
            var playersDifferentOrder = playerList.Skip(2).Union(playerList.Take(2)).ToList();
            var trickMock = new Mock<ITrick>();
            var deckMock = new Mock<IDeck>();
            var moveList = new List<SheepCard>();
            deckMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { new Mock<ITrick>().Object, trickMock.Object });
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(playerList);
            trickMock
                .Setup(m => m.Add(It.IsAny<IPlayer>(), It.IsAny<SheepCard>()))
                .Callback((IPlayer player, SheepCard card) => { moveList.Add(card); });

            var game = new Game(75291, playersDifferentOrder, null, null);
            game.Decks.Add(new Mock<IDeck>().Object);
            game.Decks.Add(deckMock.Object);
            game.PlayNonHumansInTrick();

            Assert.IsTrue(((ComputerPlayerReportingPlays)playerList[0]).MadeMove);
            Assert.IsTrue(((ComputerPlayerReportingPlays)playerList[1]).MadeMove);
            Assert.IsTrue(((ComputerPlayerReportingPlays)playerList[2]).MadeMove);
            Assert.IsFalse(((ComputerPlayerReportingPlays)playerList[4]).MadeMove);
        }

        [TestMethod]
        public void Game_PlayNonHuman_LastPlayerIsNotHuman()
        {
            var playerList = new List<Mock>() {
                new Mock<IComputerPlayer>(),
                new Mock<IComputerPlayer>(),
                new Mock<IComputerPlayer>(),
                new Mock<IComputerPlayer>()
            };
            var players = playerList.Select(m => m.Object as IPlayer).ToList();
            var playersDifferentOrder = players.Skip(2).Union(players.Take(2)).ToList();
            var hasPlayed = new List<IPlayer>();
            var trickMock = new Mock<ITrick>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { trickMock.Object });
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players);
            trickMock
                .Setup(m => m.Add(It.IsAny<IPlayer>(), It.IsAny<SheepCard>()))
                .Callback((IPlayer player, SheepCard card) => {
                    hasPlayed.Add(player);
                });
            playerList
                .OfType<Mock<IComputerPlayer>>().ToList()
                .ForEach(m => m
                    .Setup(p => p.GetMove(It.IsAny<ITrick>()))
                    .Returns(0)
                );

            var game = new Game(75291, playersDifferentOrder, new RandomWrapper(), null);
            game.Decks.Add(deckMock.Object);
            game.PlayNonHumansInTrick();

            Assert.AreEqual(4, hasPlayed.Count(), "All players have played.");
            Assert.AreSame(hasPlayed[0], players[0]);
            Assert.AreSame(hasPlayed[1], players[1]);
            Assert.AreSame(hasPlayed[2], players[2]);
            Assert.AreSame(hasPlayed[3], players[3]);
        }

        [TestMethod]
        public void Game_PlayNonHuman_HumanHasNextTurn()
        {
            var playerList = new List<Mock>() {
                new Mock<IHumanPlayer>(),
                new Mock<IComputerPlayer>(),
                new Mock<IComputerPlayer>(),
                new Mock<IComputerPlayer>()
            };
            var players = playerList.Select(m => m.Object as IPlayer).ToList();
            var playersDifferentOrder = players.Skip(2).Union(players.Take(2)).ToList();
            var trickMock = new Mock<ITrick>();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.Hand.Tricks).Returns(new List<ITrick>() { trickMock.Object });
            trickMock.Setup(m => m.PlayersWithoutTurn).Returns(players);
            playerList.OfType<Mock<IComputerPlayer>>().ToList()
                .ForEach(m => m
                    .Setup(p => p.GetMove(It.IsAny<ITrick>()))
                    .Callback(() => Assert.Fail("Since it is not a computer's turn, the method should just not play any turns.")));

            var game = new Game(75291, playersDifferentOrder, new RandomWrapper(), null);
            game.Decks.Add(deckMock.Object);
            game.PlayNonHumansInTrick();

            Assert.IsTrue(true, "Got this far without playing a computer player's turn.");
        }

        [TestMethod]
        public void Game_PlayNonHumanPickTurns_StopAtHuman()
        {
            var playerMocks = new List<Mock>() { new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>() };
            var playerList = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var unplayedPlayers = playerList.Skip(1).ToList();
            var refusingPick = playerList.Take(1).ToList();
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.PickProcessor.PlayNonHumanPickTurns())
                .Callback(() => {
                    refusingPick.AddRange(playerList.Skip(1).Take(2));
                    unplayedPlayers.RemoveRange(0, 2);
                    })
                .Returns(() => null);
            deckMock.Setup(m => m.PickProcessor.AcceptComputerPicker(It.IsAny<IComputerPlayer>()))
                .Callback((IComputerPlayer p) =>
                    Assert.IsFalse(true, "Should not have called AcceptComputerPicker().  Should have given human a chance to pick."));
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock.Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<SheepCard>>()))
                .Callback(() =>
                    Assert.IsFalse(true, "Should not have called AcceptComputerPicker().  Should have given human a chance to pick."));

            var game = new Game(42340, playerList, null, handFactoryMock.Object);
            game.Decks.Add(deckMock.Object);
            var picker = game.PlayNonHumanPickTurns();

            Assert.IsNull(picker, "Picker should be null because the computer players didn't pick.  " + 
                "No hand should have been created since not everyone has had a turn.");
        }

        [TestMethod]
        public void Game_PlayNonHumanPickTurns_FindAPicker()
        {
            var playerMocks = new List<Mock>() { new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>() };
            var playerList = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var unplayedPlayers = playerList.Skip(1).ToList();
            var expectedPicker = playerList[2] as IComputerPlayer;
            var refusingPick = playerList.Take(1).ToList();
            var handCreated = false;
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.PickProcessor.PlayNonHumanPickTurns())
                .Callback(() => {
                    refusingPick.AddRange(playerList.Skip(1).Take(1));
                    unplayedPlayers.RemoveRange(0, 1);
                })
                .Returns(expectedPicker);
            deckMock.Setup(m => m.PickProcessor.AcceptComputerPicker(expectedPicker))
                .Callback(() => handCreated = true)
                .Returns(new Mock<IHand>().Object);
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock.Setup(m => m.GetHand(It.IsAny<IDeck>(), It.IsAny<IPlayer>(), It.IsAny<List<SheepCard>>()))
                .Callback(() =>
                    Assert.IsFalse(true, "Should have instantiated hand through AcceptComputerPicker() instead."));

            var game = new Game(42340, playerList, null, handFactoryMock.Object);
            game.Decks.Add(deckMock.Object);
            var picker = game.PlayNonHumanPickTurns();

            Assert.IsTrue(handCreated);
            Assert.AreEqual(expectedPicker, picker);
        }

        [TestMethod]
        public void Game_PlayNonHumanPickTurns_LeastersGame()
        {
            var playerMocks = new List<Mock>() { new Mock<IHumanPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>(), new Mock<IComputerPlayer>() };
            var playerList = playerMocks.Select(m => m.Object as IPlayer).ToList();
            var unplayedPlayers = playerList.Skip(1).ToList();
            var refusingPick = playerList.Take(1).ToList();
            var handCreated = false;
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersWithoutPickTurn).Returns(unplayedPlayers);
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.PickProcessor.PlayNonHumanPickTurns())
                .Callback(() => {
                    refusingPick.AddRange(playerList.Skip(1).Take(4));
                    unplayedPlayers.RemoveRange(0, 4);
                })
                .Returns(() => null);
            deckMock.Setup(m => m.PickProcessor.AcceptComputerPicker(It.IsAny<IComputerPlayer>()))
                .Callback((IComputerPlayer p) =>
                    Assert.IsFalse(true, "Should have instantiated hand through HandFactory.  Should be a leasters hand."));
            var handFactoryMock = new Mock<IHandFactory>();
            handFactoryMock.Setup(m => m.GetHand(deckMock.Object, null, It.IsAny<List<SheepCard>>()))
                .Callback(() => handCreated = true)
                .Returns(() => new Mock<IHand>().Object);

            var game = new Game(42340, playerList, null, handFactoryMock.Object);
            game.Decks.Add(deckMock.Object);
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

        [TestMethod]
        public void Deck_MakeDeck()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new Player());
            var game = new Game(4982, playerList, new RandomWrapper(), null);
            var deck = new Deck(game, new RandomWrapper());
            Assert.AreEqual(2, deck.Blinds.Count(), "There should be two blinds after dealing");
            Assert.AreEqual(5, game.Players.Count(), "There should be five doctores");
            foreach (var player in deck.Game.Players)
                Assert.AreEqual(6, player.Cards.Count(), "There are 6 cards in each players hand.");
        }

        [TestMethod]
        public void Deck_StartingPlayer()
        {
            var player1 = new Mock<BasicPlayer>();
            var player2 = new Mock<NewbiePlayer>();
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<BasicPlayer>();
            var player5 = new Mock<BasicPlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            var mockGame = new Mock<IGame>();
            mockGame.Setup(m => m.Players).Returns(playerList);
            mockGame.Setup(m => m.PlayerCount).Returns(5);
            var mockDeck = new Mock<IDeck>();
            mockDeck.Setup(m => m.Game).Returns(mockGame.Object);
            IDeck deck2;
            var deckList = new List<IDeck>() { mockDeck.Object };
            mockGame.Setup(m => m.Decks).Returns(deckList);
            mockGame.Setup(m => m.LastDeckIsComplete()).Returns(true);

            mockDeck.Setup(m => m.StartingPlayer).Returns(player1.Object);
            deck2 = new Deck(mockGame.Object, new RandomWrapper());
            //We won't test the Starting Player for the first deck in the game.  It should be random.
            Assert.AreEqual(player2.Object, deck2.StartingPlayer, "The starting player for one deck should be the player to the left of the previous starting player.");

            mockGame.Object.Decks.RemoveAt(1);
            mockDeck.Setup(m => m.StartingPlayer).Returns(player2.Object);
            deck2 = new Deck(mockGame.Object, new RandomWrapper());
            //We won't test the Starting Player for the first deck in the game.  It should be random.
            Assert.AreEqual(player3.Object, deck2.StartingPlayer, "Again, the starting player for one deck should be the player to the left of the previous starting player.");
        }
    }
}
