using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sheepshead.Logic;
using Sheepshead.Logic.Players;
using System.Text;
using System.IO;
using Moq;
using System.Threading;
using Sheepshead.Logic.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class NonUnitTest
    {
        private Dictionary<string, int> _playerTypeCoins;

        //[TestMethod]
        public void TestAllPlayers()
        {
            _playerTypeCoins = new Dictionary<string, int>();
            var stringBuilder = new StringBuilder();
            var gameNo = 0;
            var handCount = 500 * 1000;
            PartnerMethod partnerMethod;

            partnerMethod = PartnerMethod.JackOfDiamonds;
            PlayTwoFivePlayer(typeof(IntermediatePlayer), typeof(SimplePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(SimplePlayer), typeof(IntermediatePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(AdvancedPlayer), typeof(IntermediatePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(IntermediatePlayer), typeof(AdvancedPlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(AdvancedPlayer), typeof(SimplePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(SimplePlayer), typeof(AdvancedPlayer), stringBuilder, partnerMethod, ref gameNo, handCount);

            partnerMethod = PartnerMethod.CalledAce;
            PlayTwoFivePlayer(typeof(IntermediatePlayer), typeof(SimplePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(SimplePlayer), typeof(IntermediatePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(AdvancedPlayer), typeof(IntermediatePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(IntermediatePlayer), typeof(AdvancedPlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(AdvancedPlayer), typeof(SimplePlayer), stringBuilder, partnerMethod, ref gameNo, handCount);
            PlayTwoFivePlayer(typeof(SimplePlayer), typeof(AdvancedPlayer), stringBuilder, partnerMethod, ref gameNo, handCount);

            PlayTwoThreePlayer(typeof(IntermediatePlayer), typeof(SimplePlayer), stringBuilder, ref gameNo, handCount);
            PlayTwoThreePlayer(typeof(AdvancedPlayer), typeof(IntermediatePlayer), stringBuilder, ref gameNo, handCount);
            PlayTwoThreePlayer(typeof(AdvancedPlayer), typeof(SimplePlayer), stringBuilder, ref gameNo, handCount);

            foreach (var kvp in _playerTypeCoins)
                stringBuilder.AppendLine(StringSegment(kvp.Key) + kvp.Value);

            File.WriteAllText(@"F:\Users\bjkrug\Documents\scores.txt", stringBuilder.ToString());
        }

        private void PlayTwoFivePlayer(Type player1, Type player2, StringBuilder stringBuilder, PartnerMethod partnerMethod, ref int gameNo, int handCount)
        {
            List<IPlayer> players = new List<IPlayer>()
            {
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player2),
                (IPlayer)Activator.CreateInstance(player2),
                (IPlayer)Activator.CreateInstance(player2),
            };
            players[0].Name = player1.Name;
            players[1].Name = player1.Name;
            players[2].Name = player2.Name;
            players[3].Name = player2.Name;
            players[4].Name = player2.Name;
            stringBuilder.Append(RunGame(++gameNo, players, partnerMethod, handCount));

            players = new List<IPlayer>()
            {
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player2),
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player2),
                (IPlayer)Activator.CreateInstance(player2),
            };
            players[0].Name = player1.Name;
            players[1].Name = player2.Name;
            players[2].Name = player1.Name;
            players[3].Name = player2.Name;
            players[4].Name = player2.Name;
            stringBuilder.Append(RunGame(++gameNo, players, partnerMethod, handCount));
        }

        private void PlayTwoThreePlayer(Type player1, Type player2, StringBuilder stringBuilder, ref int gameNo, int handCount)
        {
            List<IPlayer> players = new List<IPlayer>()
            {
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player2),
                (IPlayer)Activator.CreateInstance(player2),
            };
            players[0].Name = player1.Name;
            players[1].Name = player2.Name;
            players[2].Name = player2.Name;
            stringBuilder.Append(RunGame(++gameNo, players, PartnerMethod.JackOfDiamonds, handCount));

            players = new List<IPlayer>()
            {
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player1),
                (IPlayer)Activator.CreateInstance(player2),
            };
            players[0].Name = player1.Name;
            players[1].Name = player1.Name;
            players[2].Name = player2.Name;
            stringBuilder.Append(RunGame(++gameNo, players, PartnerMethod.JackOfDiamonds, handCount));
        }

        private string RunGame(int gameNo, List<IPlayer> players, PartnerMethod partnerMethod, int handsToPlay)
        {
            var game = new Game(players, partnerMethod, enableLeasters: true);
            for (var g = 0; g < handsToPlay; ++g)
            {
                var hand = new Hand(game);
                var picker = game.PlayNonHumanPickTurns();
                var buriedCards = picker != null ? picker.DropCardsForPick(hand) : new List<SheepCard>();
                while (!hand.IsComplete())
                {
                    var trick = new Trick(hand);
                    game.PlayNonHumansInTrick();
                }
            }
            var outputString1 = "";
            var outputString2 = "";
            foreach (var coinSet in game.GameCoins())
            {
                outputString1 += StringSegment(coinSet.Name);
                outputString2 += StringSegment(coinSet.Coins.ToString());
                if (!_playerTypeCoins.ContainsKey(coinSet.Name))
                    _playerTypeCoins[coinSet.Name] = 0;
                _playerTypeCoins[coinSet.Name] += coinSet.Coins;
            }
            //var leastersScores = game.Decks.Where(d => d.Hand.Leasters).Select(d => d.Hand.Scores());
            return "Game: " + gameNo + (game.PlayerCount == 5 ? "  " + game.PartnerMethod.ToString() : "") + Environment.NewLine + 
                outputString1 + Environment.NewLine + 
                outputString2 + Environment.NewLine +
                Environment.NewLine;
        }

        private string StringSegment(string part, int length = 20)
        {
            var spaceCount = length - part.Length;
            var spaces = "";
            for(var a = 0; a < spaceCount; ++a) { spaces += " "; }
            return part + spaces;
        }
    }
}
