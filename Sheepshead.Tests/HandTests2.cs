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
            var participantList = new List<Participant>();
            for (var i = 0; i < 5; ++i)
                participantList.Add(new Participant());
            var game = new Game(participantList, PartnerMethod.JackOfDiamonds, new RandomWrapper(), null) {
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
            var player1 = new Mock<IPlayer>();
            var player2 = new Mock<IPlayer>();
            var player3 = new Mock<IPlayer>();
            var player4 = new Mock<IPlayer>();
            var player5 = new Mock<IPlayer>();
            var playerMockList = new List<Mock<IPlayer>>() { player3, player4, player5, player1, player2 };
            var playerList = playerMockList.Select(m => m.Object).ToList();
            playerMockList.ForEach(m => m.Setup(mm => mm.Cards).Returns(new List<SheepCard>()));
            var mockGame = new MockGame();
            mockGame.SetPlayers(playerList);
            mockGame.SetPlayerCount(5);
            var mockHand = new MockHand();
            mockHand.SetIGame(mockGame);
            var handList = new List<Hand>() { mockHand };
            mockGame.Hands = handList;
            mockGame.SetLastHandIsComplete(true);

            mockHand.SetStartingPlayer(player1.Object);
            IHand hand;
            hand = new Hand(mockGame, new RandomWrapper());
            //We won't test the Starting Player for the first hand in the game.  It should be random.
            Assert.AreEqual(player2.Object, hand.StartingPlayer, "The starting player for one hand should be the player to the left of the previous starting player.");

            mockGame.Hands.Remove(mockGame.Hands.ElementAt(1));
            mockHand.SetStartingPlayer(player2.Object);
            hand = new Hand(mockGame, new RandomWrapper());
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
