using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameTests
    {
        private class ExposeGame : Game {
            public ExposeGame() : base (0, new List<IPlayer>(), new RandomWrapper())
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
            var player1 = new Mock<BasicPlayer>();
            var player2 = new HumanPlayer(new User());
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<BasicPlayer>();
            var player5 = new HumanPlayer(new User());
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5, player1.Object, player2 };
            var handMock = new Mock<IHand>();
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.Hand).Returns(handMock.Object);
            var game = new Game(42340, playerList, new RandomWrapper());
            trickMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            bool player1Moved = false;
            bool player3Moved = false;
            bool player4Moved = false;
            player1.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player1Moved = true);
            player3.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player3Moved = true);
            player4.Setup(m => m.GetMove(It.IsAny<ITrick>())).Callback(() => player4Moved = true);
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>());
            game.PlayNonHumans(trickMock.Object);
            Assert.IsTrue(player1Moved, "All players from the starting player to the first human should have been played.");
            Assert.IsFalse(player3Moved, "Plyaer 3 should not have been played yet.");
            Assert.IsFalse(player4Moved, "Player 4 should not have been played yet.");
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>() { { player1.Object, new Card() }, { player2, new Card() } });
            game.PlayNonHumans(trickMock.Object);
            player1Moved = false;
            Assert.IsFalse(player1Moved, "PLayer 1 should not have been played a second time.");
            Assert.IsTrue(player3Moved, "Player 3 should have been played when a second call was made to PlayNonHumans()");
            Assert.IsTrue(player4Moved, "Player 4 should have been played when a second call was made to PlayNonHumans()");
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>() { { player1.Object, new Card() }, { player2, new Card() }, { player3.Object, new Card() }, { player4.Object, new Card() }, { player5, new Card() } } );
            game.PlayNonHumans(trickMock.Object);
            Assert.IsTrue(true, "Did not end up in infinite loop, when last player was human.");
        }

        [TestMethod]
        public void Game_PlayNonHuman_LastPlayerIsNonhuman()
        {
            var player1 = new Mock<BasicPlayer>();
            var player2 = new Mock<BasicPlayer>();
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<BasicPlayer>();
            var player5 = new Mock<BasicPlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            var trickMock = new Mock<ITrick>();
            var game = new Game(42340, playerList, new RandomWrapper());
            trickMock.Setup(m => m.Game).Returns(game);
            trickMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>());
            game.PlayNonHumans(trickMock.Object);
            Assert.IsTrue(true, "Did not enter infinite loop");
        }

        [TestMethod]
        public void Game_PlayNonHuman_FindPicker()
        {
            var player1 = new Mock<IComputerPlayer>();
            var player2 = new HumanPlayer(new User());
            var player3 = new Mock<IComputerPlayer>();
            var player4 = new Mock<IComputerPlayer>();
            var player5 = new HumanPlayer(new User());
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5, player1.Object, player2 };
            var deckMock = new Mock<IDeck>();
            var game = new Game(42340, playerList, new RandomWrapper());
            deckMock.Setup(m => m.Game).Returns(game);
            deckMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            bool player1Moved = false;
            bool player3Moved = false;
            bool player4Moved = false;
            player1.Setup(m => m.WillPick(It.IsAny<IDeck>())).Callback(() => player1Moved = true);
            player3.Setup(m => m.WillPick(It.IsAny<IDeck>())).Callback(() => player3Moved = true);
            player4.Setup(m => m.WillPick(It.IsAny<IDeck>())).Callback(() => player4Moved = true);
            var refusingPick = new List<IPlayer>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.PlayerWontPick(It.IsAny<IPlayer>())).Callback((IPlayer givenPlyaer) => { refusingPick.Add(givenPlyaer); });
            var picker = game.PlayNonHumans(deckMock.Object);
            Assert.IsTrue(player1Moved, "All players from the starting player to the first human should have been played.");
            Assert.IsFalse(player3Moved, "Player 3 should not have been played yet.");
            Assert.IsFalse(player4Moved, "Player 4 should not have been played yet.");
            Assert.IsTrue(PlayerListsMatch(refusingPick, new List<IPlayer>() {player1.Object}), "Player1 is on the refused list.");
            Assert.AreEqual(null, picker, "No one picked");
            refusingPick.Add(player1.Object);
            refusingPick.Add(player2);
            game.PlayNonHumans(deckMock.Object);
            player1Moved = false;
            Assert.IsFalse(player1Moved, "Player 1 should not have been played a second time.");
            Assert.IsTrue(player3Moved, "Player 3 should have been played when a second call was made to PlayNonHumans()");
            Assert.IsTrue(player4Moved, "Player 4 should have been played when a second call was made to PlayNonHumans()");
            Assert.IsTrue(PlayerListsMatch(refusingPick, new List<IPlayer>() { player1.Object, player2, player3.Object, player4.Object }), "Player1, 3, and 4 is on the refused list.");
            refusingPick.Add(player3.Object);
            refusingPick.Add(player4.Object);
            refusingPick.Add(player5);
            game.PlayNonHumans(deckMock.Object);
            Assert.IsTrue(true, "Did not enter an infinite loop.");
        }

        [TestMethod]
        public void Game_PlayNonHuman_FindPicker_OnePlayerPicked()
        {
            var player1 = new Mock<IComputerPlayer>();
            var player2 = new Mock<IComputerPlayer>();
            var player3 = new Mock<IComputerPlayer>();
            var player4 = new Mock<IComputerPlayer>();
            var player5 = new Mock<IComputerPlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            player3.Setup(m => m.WillPick(It.IsAny<IDeck>())).Returns(true);
            player3.Setup(m => m.DropCardsForPick(It.IsAny<IDeck>())).Returns(new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object });
            var game = new Game(42340, playerList, new RandomWrapper());
            var deckMock = new Mock<IDeck>();
            var refusingPick = new List<IPlayer>();
            var discards = new List<ICard>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.Game).Returns(game);
            deckMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            deckMock.Setup(m => m.Buried).Returns(discards);
            var picker = game.PlayNonHumans(deckMock.Object);
            Assert.AreEqual(player3.Object, picker, "Player 3 is picker");
            Assert.AreEqual(2, discards.Count(), "There are two discards");
        }

        [TestMethod]
        public void Game_PlayNonHuman_FindPicker_LastPlayerIsntHuman()
        {
            var player1 = new Mock<IComputerPlayer>();
            var player2 = new Mock<IComputerPlayer>();
            var player3 = new Mock<IComputerPlayer>();
            var player4 = new Mock<IComputerPlayer>();
            var player5 = new Mock<IComputerPlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            var game = new Game(42340, playerList, new RandomWrapper());
            var deckMock = new Mock<IDeck>();
            var refusingPick = new List<IPlayer>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.Game).Returns(game);
            deckMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            game.PlayNonHumans(deckMock.Object);
            Assert.IsTrue(true, "Didn't end up in an infinite loop.");
        }

        [TestMethod]
        public void Game_PlayNonHuman_FindPicker_AlsoBurriedCards()
        {
            var player1 = new Mock<IComputerPlayer>();
            var player2 = new Mock<IComputerPlayer>();
            var player3 = new Mock<IComputerPlayer>();
            var player4 = new Mock<IComputerPlayer>();
            var player5 = new Mock<IComputerPlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            var playerBuriedCards = false;
            player2.Setup(m => m.WillPick(It.IsAny<IDeck>())).Returns(true);
            player2.Setup(m => m.DropCardsForPick(It.IsAny<IDeck>()))
                .Callback((IDeck givenDeck) => { playerBuriedCards = true; })
                .Returns(new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object });
            var game = new Game(42340, playerList, new RandomWrapper());
            var deckMock = new Mock<IDeck>();
            var refusingPick = new List<IPlayer>();
            var discards = new List<ICard>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(refusingPick);
            deckMock.Setup(m => m.Game).Returns(game);
            deckMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            deckMock.Setup(m => m.Buried).Returns(discards);
            game.PlayNonHumans(deckMock.Object);
            Assert.IsTrue(playerBuriedCards, "Player 2 buried cards after picking.");
            Assert.AreEqual(2, discards.Count(), "There are two buried cards.");
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
        public void Game_PlayNonHuman_FindPicker_EveryoneHasPassed()
        {
            var player1 = new Mock<IComputerPlayer>();
            var player2 = new Mock<IComputerPlayer>();
            var player3 = new Mock<IComputerPlayer>();
            var player4 = new Mock<IComputerPlayer>();
            var player5 = new Mock<IHumanPlayer>();
            var players = new List<IPlayer>() { player1.Object, player2.Object, player3.Object, player4.Object, player5.Object };
            var deckMock = new Mock<IDeck>();
            deckMock.Setup(m => m.PlayersRefusingPick).Returns(players.ToList());
            deckMock.Setup(m => m.StartingPlayer).Returns(player1.Object);
            player1.Setup(m => m.QueueRankInDeck(It.IsAny<IDeck>())).Returns(1);

            var game = new Game(1, players, new RandomWrapper());
            var picker = game.PlayNonHumans(deckMock.Object);

            Assert.AreEqual(null, picker, "There is no picker if everyone has already passed.");
        }

        [TestMethod]
        public void Deck_MakeDeck()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new Player());
            var game = new Game(4982, playerList, new RandomWrapper());
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
