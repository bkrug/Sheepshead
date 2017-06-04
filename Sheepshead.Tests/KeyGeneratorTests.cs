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

namespace Sheepshead.Tests
{
    [TestClass]
    public class KeyGeneratorTests
    {
        [TestMethod]
        public void GenerateKey()
        {
            var summary = "9DJH,39HAS,8H7C9CKH7H,KSKC9DTHAH,7D8DTDQDJS,TCACJD9S7S,KDQSJHQH8S,ADQCJC8CTS";
            var hand = SummaryReader.FromSummary(summary);
            var generator = new MoveKeyGenerator();
            var sb = new StringBuilder();
            sb.AppendLine(summary);
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
                    sb.AppendLine(String.Format("{5} {6}, Card Will Overpower: {0}, Card Points: {1},  Opponent Percent Done: {2}, Unknown Stronger Cards: {3}," + 
                        " Held Stronger Cards: {4}", 
                        key.CardWillOverpower ? "True " : "False", key.CardPoints.ToString("D2"), key.OpponentPercentDone.ToString("D3"), 
                        key.UnknownStrongerCards.ToString("D2"), key.HeldStrongerCards, playerType, card.ToAbbr()));
                }
                sb.AppendLine("----------------------------------------------------------------------------");
            }
            Assert.AreEqual(expectedResults, sb.ToString());
        }

        private string expectedResults =
@"9DJH,39HAS,8H7C9CKH7H,KSKC9DTHAH,7D8DTDQDJS,TCACJD9S7S,KDQSJHQH8S,ADQCJC8CTS
Defence 8H, Card Will Overpower: True , Card Points: 00,  Opponent Percent Done: 000, Unknown Stronger Cards: 15, Held Stronger Cards: 3
Defence 7C, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 000, Unknown Stronger Cards: 14, Held Stronger Cards: 5
Picker  9C, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 050, Unknown Stronger Cards: 12, Held Stronger Cards: 5
Defence KH, Card Will Overpower: False, Card Points: 04,  Opponent Percent Done: 100, Unknown Stronger Cards: 13, Held Stronger Cards: 3
Partner 7H, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 100, Unknown Stronger Cards: 15, Held Stronger Cards: 2
----------------------------------------------------------------------------
Defence TH, Card Will Overpower: True , Card Points: 10,  Opponent Percent Done: 000, Unknown Stronger Cards: 13, Held Stronger Cards: 2
Partner AH, Card Will Overpower: True , Card Points: 11,  Opponent Percent Done: 033, Unknown Stronger Cards: 13, Held Stronger Cards: 1
Defence KS, Card Will Overpower: False, Card Points: -04,  Opponent Percent Done: 000, Unknown Stronger Cards: 13, Held Stronger Cards: 3
Defence KC, Card Will Overpower: False, Card Points: -04,  Opponent Percent Done: 000, Unknown Stronger Cards: 12, Held Stronger Cards: 4
Picker  9D, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 100, Unknown Stronger Cards: 07, Held Stronger Cards: 4
----------------------------------------------------------------------------
Picker  TD, Card Will Overpower: True , Card Points: 10,  Opponent Percent Done: 000, Unknown Stronger Cards: 06, Held Stronger Cards: 3
Defence QD, Card Will Overpower: True , Card Points: 03,  Opponent Percent Done: 050, Unknown Stronger Cards: 02, Held Stronger Cards: 1
Partner JS, Card Will Overpower: False, Card Points: -02,  Opponent Percent Done: 033, Unknown Stronger Cards: 04, Held Stronger Cards: 0
Defence 7D, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 100, Unknown Stronger Cards: 07, Held Stronger Cards: 2
Defence 8D, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 100, Unknown Stronger Cards: 06, Held Stronger Cards: 2
----------------------------------------------------------------------------
Defence 9S, Card Will Overpower: True , Card Points: 00,  Opponent Percent Done: 000, Unknown Stronger Cards: 09, Held Stronger Cards: 1
Partner 7S, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 033, Unknown Stronger Cards: 09, Held Stronger Cards: 2
Defence TC, Card Will Overpower: False, Card Points: 10,  Opponent Percent Done: 050, Unknown Stronger Cards: 07, Held Stronger Cards: 2
Defence AC, Card Will Overpower: False, Card Points: 11,  Opponent Percent Done: 050, Unknown Stronger Cards: 06, Held Stronger Cards: 2
Picker  JD, Card Will Overpower: True , Card Points: 02,  Opponent Percent Done: 100, Unknown Stronger Cards: 03, Held Stronger Cards: 2
----------------------------------------------------------------------------
Picker  JH, Card Will Overpower: True , Card Points: 02,  Opponent Percent Done: 000, Unknown Stronger Cards: 03, Held Stronger Cards: 1
Defence QH, Card Will Overpower: True , Card Points: 03,  Opponent Percent Done: 050, Unknown Stronger Cards: 02, Held Stronger Cards: 0
Partner 8S, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 033, Unknown Stronger Cards: 06, Held Stronger Cards: 1
Defence KD, Card Will Overpower: False, Card Points: 04,  Opponent Percent Done: 100, Unknown Stronger Cards: 03, Held Stronger Cards: 1
Defence QS, Card Will Overpower: False, Card Points: 03,  Opponent Percent Done: 100, Unknown Stronger Cards: 00, Held Stronger Cards: 1
----------------------------------------------------------------------------
Defence QC, Card Will Overpower: True , Card Points: 03,  Opponent Percent Done: 000, Unknown Stronger Cards: 00, Held Stronger Cards: 0
Picker  JC, Card Will Overpower: False, Card Points: -02,  Opponent Percent Done: 033, Unknown Stronger Cards: 00, Held Stronger Cards: 0
Defence 8C, Card Will Overpower: False, Card Points: 00,  Opponent Percent Done: 050, Unknown Stronger Cards: 01, Held Stronger Cards: 0
Partner TS, Card Will Overpower: False, Card Points: -10,  Opponent Percent Done: 067, Unknown Stronger Cards: 02, Held Stronger Cards: 0
Defence AD, Card Will Overpower: False, Card Points: 11,  Opponent Percent Done: 100, Unknown Stronger Cards: 00, Held Stronger Cards: 0
----------------------------------------------------------------------------
";
    }
}
