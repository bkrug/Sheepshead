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
    public class DeckTests
    {
        [TestMethod]
        public void Deck_MakeDeck()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new Player());
            var game = new Game(playerList, PartnerMethod.JackOfDiamonds, new RandomWrapper(), null, null);
            var deck = new Deck(game, new RandomWrapper());
            Assert.AreEqual(2, deck.Blinds.Count(), "There should be two blinds after dealing");
            Assert.AreEqual(5, game.Players.Count(), "There should be five doctores");
            foreach (var player in deck.Game.Players)
                Assert.AreEqual(6, player.Cards.Count(), "There are 6 cards in each players hand.");
        }

        [TestMethod]
        public void Deck_StartingPlayer()
        {
            var player1 = new Mock<IntermediatePlayer>();
            var player2 = new Mock<NewbiePlayer>();
            var player3 = new Mock<NewbiePlayer>();
            var player4 = new Mock<IntermediatePlayer>();
            var player5 = new Mock<IntermediatePlayer>();
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
