using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Model;
using Sheepshead.Model.Players;
using Sheepshead.Model.Wrappers;
using Sheepshead.Model.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class HandTests2
    {
        [TestMethod]
        public void Hand_Constructor()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new Player());
            var game = new Game(playerList, PartnerMethod.JackOfDiamonds, new RandomWrapper(), null) {
                Hands = new List<Hand>()
            };
            var hand = new Hand(game);
            Assert.AreEqual(2, hand.Blinds.Count(), "There should be two blinds after dealing");
            Assert.AreEqual(5, game.Players.Count(), "There should be five players");
            foreach (var player in hand.IGame.Players)
                Assert.AreEqual(6, player.Cards.Count(), "There are 6 cards in each players hand.");
        }

        [TestMethod]
        public void Hand_StartingPlayer()
        {
            var player1 = new Mock<IntermediatePlayer>();
            var player2 = new Mock<SimplePlayer>();
            var player3 = new Mock<SimplePlayer>();
            var player4 = new Mock<IntermediatePlayer>();
            var player5 = new Mock<IntermediatePlayer>();
            var playerList = new List<IPlayer>() { player3.Object, player4.Object, player5.Object, player1.Object, player2.Object };
            var mockGame = new Mock<IGame>();
            mockGame.Setup(m => m.Players).Returns(playerList);
            mockGame.Setup(m => m.PlayerCount).Returns(5);
            var mockHand = new MockHand();
            mockHand.SetIGame(mockGame.Object);
            var deckList = new List<Hand>() { mockHand };
            mockGame.Setup(m => m.Hands).Returns(deckList);
            mockGame.Setup(m => m.LastHandIsComplete()).Returns(true);

            mockHand.SetStartingPlayer(player1.Object);
            IHand hand;
            hand = new Hand(mockGame.Object, new RandomWrapper());
            //We won't test the Starting Player for the first hand in the game.  It should be random.
            Assert.AreEqual(player2.Object, hand.StartingPlayer, "The starting player for one hand should be the player to the left of the previous starting player.");

            mockGame.Object.Hands.Remove(mockGame.Object.Hands.ElementAt(1));
            mockHand.SetStartingPlayer(player2.Object);
            hand = new Hand(mockGame.Object, new RandomWrapper());
            //We won't test the Starting Player for the first hand in the game.  It should be random.
            Assert.AreEqual(player3.Object, hand.StartingPlayer, "Again, the starting player for one hand should be the player to the left of the previous starting player.");
        }

        private class MockHand : Hand
        {
            public void SetIGame(IGame game)
            {
                IGame = game;
            }

            public void SetStartingPlayer(IPlayer player)
            {
                StartingPlayer = player;
            }
        }
    }
}
