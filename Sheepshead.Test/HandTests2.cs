using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Model;
using Sheepshead.Model.Players;
using Sheepshead.Model.Wrappers;
using Sheepshead.Logic.Models;

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
            var game = new Game(participantList, PartnerMethod.JackOfDiamonds, true) {
                Hand = new List<Hand>()
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
            var player1 = new Participant().Player;
            var player2 = new Participant().Player;
            var player3 = new Participant().Player;
            var player4 = new Participant().Player;
            var player5 = new Participant().Player;
            var playerList = new List<IPlayer>() { player3, player4, player5, player1, player2 };
            var mockGame = new MockGame();
            mockGame.SetPlayers(playerList);
            mockGame.SetPlayerCount(5);
            var mockHand = new MockHand();
            mockHand.SetIGame(mockGame);
            var handList = new List<Hand>() { mockHand };
            mockGame.Hand = handList;
            mockGame.SetLastHandIsComplete(true);

            mockHand.SetStartingPlayer(player1);
            IHand hand;
            hand = new Hand(mockGame, new RandomWrapper());
            //We won't test the Starting Player for the first hand in the game.  It should be random.
            Assert.AreEqual(player2, hand.StartingPlayer, "The starting player for one hand should be the player to the left of the previous starting player.");

            mockGame.Hand.Remove(mockGame.Hand.ElementAt(1));
            mockHand.SetStartingPlayer(player2);
            hand = new Hand(mockGame, new RandomWrapper());
            //We won't test the Starting Player for the first hand in the game.  It should be random.
            Assert.AreEqual(player3, hand.StartingPlayer, "Again, the starting player for one hand should be the player to the left of the previous starting player.");
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
