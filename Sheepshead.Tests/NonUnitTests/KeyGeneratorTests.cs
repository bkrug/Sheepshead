using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests.NonUnitTests
{
    [TestClass]
    public class KeyGeneratorTests
    {
        //[TestMethod]
        public void GenerateKey()
        {
            var summary = "9DJH,39HAS,8H7C9CKH7H,KSKC9DTHAH,7D8DTDQDJS,TCACJD9S7S,KDQSJHQH8S,ADQCJC8CTS";
            var hand = SummaryReader.FromSummary(summary);
            var generator = new KeyGenerator();
            using (var sw = new StreamWriter(@"c:\temp\keys.txt"))
            {
                sw.WriteLine(summary);
                foreach (var trick in hand.Tricks)
                {
                    foreach (var move in trick.OrderedMoves)
                    {
                        var player = move.Key;
                        var card = move.Value;
                        var key = generator.GenerateKey(trick, player, card);
                        var pickI = trick.QueueRankOfPicker;
                        var partI = trick.QueueRankOfPartner;
                        var playerType = "Defence";
                        if (hand.Picker == player)
                            playerType = "Picker ";
                        if (hand.Partner == player)
                            playerType = "Partner";
                        sw.WriteLine("{5} {6}, Card Will Overpower: {0}, Card Points: {1},  Opponent Percent Done: {2}, Unknown Stronger Cards: {3}," + 
                            " Held Stronger Cards: {4}", 
                            key.CardWillOverpower ? "True " : "False", key.CardPoints.ToString("D2"), key.OpponentPercentDone.ToString("D3"), 
                            key.UnknownStrongerCards.ToString("D2"), key.HeldStrongerCards, playerType, card.ToAbbr());
                    }
                    sw.WriteLine("----------------------------------------------------------------------------");
                }
            }
        }
    }
}
