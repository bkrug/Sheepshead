using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests.NonUnitTests
{
    [TestClass]
    public class GamePlayTests
    {
        //[TestMethod]
        public void LearningHelper_PlayGameAndSaveResults()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new BasicPlayer());
            PlayGame(playerList, 500, SaveLocations.FIRST_SAVE);
        }

        //[TestMethod]
        public void LearningHelper_GenerateKeys()
        {
            var predictorMock = new Mock<IStatResultPredictor>();
            predictorMock.Setup(m => m.GetWeightedStat(It.IsAny<MoveStatUniqueKey>())).Returns(null as MoveStat);
            using (_sw = new StreamWriter(@"C:\Temp\GeneratedKeys.csv"))
            {
                var playerList = new List<IPlayer>();
                var generator = new KeyGenerator();
                for (var i = 0; i < 5; ++i)
                {
                    var player = new LearningPlayer(generator, predictorMock.Object);
                    playerList.Add(player);
                    player.OnMove += (object sender, LearningPlayer.OnMoveEventArgs args) => {
                        var key = generator.GenerateKey(args.Trick, sender as IPlayer, args.Card);
                        _sw.WriteLine(
                            args.Card.ToAbbr() + "," +
                            args.Card.Rank + "," +
                            args.Card.Points + "," +
                            key.CardWillOverpower + "," +
                            key.OpponentPercentDone + "," +
                            key.CardPoints + "," +
                            key.UnknownStrongerCards + "," +
                            key.HeldStrongerCards
                        );
                    };
                }
                _sw.WriteLine("Card,Rank,Points,Card Will Overpower,Opponent Percent Done,Card Points,Unknown Stronger Cards,Held Stronger Cards");
                PlayGame(playerList, 1, @"c:\temp\LearningPlayerStage1.txt");
            }
        }

        //[TestMethod]
        public void LearningHelper_Read()
        {
            var loadSize = 2500;
            var loader = new SummaryLoader(SaveLocations.FIRST_SAVE, 0, loadSize);
            _predictor1 = loader.ResultPredictor;
            var loader1 = new SummaryLoader(SaveLocations.FIRST_SAVE, loadSize, loadSize);
            _predictor2 = loader1.ResultPredictor;
            _predictor1.SaveStats(@"C:\Temp\learningStatsStage1.json");
            var playerList = new List<IPlayer>();
            using (_sw = new StreamWriter(@"C:\Temp\learningResults.csv"))
            {
                _sw.WriteLine("Trick % 1,Trick % 2,Diff,Hand % 1,Hand % 2,Diff,Offense Side,Picker Done,Partner Done,Points In Trick,Highest Rank In Trick,Winning Side,This Card More Power,More Powerful Unknown Cards,Remaining Unknown Points,More Powerful Held,Points Held,Cards Held With Points,Move Index,Trick Index");
                for (var i = 0; i < 5; ++i)
                {
                    //var lPlayer = new LearningPlayer(new KeyGenerator(), _predictor1);
                    //playerList.Add(lPlayer);
                    //lPlayer.OnMove += lPlayer_OnMove;
                }
                var handNumber = loadSize * 2;
                PlayGame(playerList, handNumber, @"c:\temp\LearningPlayerStage1.txt");
                _sw.WriteLine(",," + (_trickDiffSum / handNumber / 30) + ",,," + (_handDiffSum / handNumber / 30));
            }
        }

        private StreamWriter _sw;
        private CentroidResultPredictor _predictor1;
        private CentroidResultPredictor _predictor2;
        private decimal _trickDiffSum = 0;
        private decimal _handDiffSum = 0;
        void lPlayer_OnMove(object sender, LearningPlayer.OnMoveEventArgs e)
        {
            IPlayer player = (IPlayer)sender;
            var generator = new Key2Generator();
            var key = generator.GenerateKey(e.Trick, player, e.Card);
            var moveStat1 = _predictor1.GetPrediction(key);
            var moveStat2 = _predictor2.GetPrediction(key);
            var trickDiff = moveStat1 == null || moveStat2 == null ? -1 : Math.Abs((decimal)moveStat1.TrickPortionWon - (decimal)moveStat2.TrickPortionWon);
            var handDiff = moveStat1 == null || moveStat2 == null ? -1 : Math.Abs((decimal)moveStat1.HandPortionWon - (decimal)moveStat2.HandPortionWon);
            _sw.WriteLine(
                (moveStat1 == null ? "" : moveStat1.TrickPortionWon.ToString()) + "," +
                (moveStat2 == null ? "" : moveStat2.TrickPortionWon.ToString()) + "," +
                trickDiff + "," +
                (moveStat1 == null ? "" : moveStat1.HandPortionWon.ToString()) + "," +
                (moveStat2 == null ? "" : moveStat2.HandPortionWon.ToString()) + "," +
                handDiff + "," +
                key.OffenseSide + "," + //Card is played by Offense
                key.PickerDone + "," + //Picker already played this trick
                (!key.PartnerDone.HasValue ? "" : key.PartnerDone.Value ? "true" : "false") + "," +
                key.PointsInTrick + "," +
                key.HighestRankInTrick + "," +
                key.WinningSide + "," + //Card is played by side currently winning trick
                key.ThisCardMorePowerful + "," + //This card is more powerful than highest card played in trick
                key.MorePowerfulUnknownCards + "," + //Includes cards in other players' hands, or in the blind unless this is the picker.
                key.RemainingUnknownPoints + "," + //Total points, not total cards
                key.MorePowerfulHeld + "," +
                key.PointsHeld + "," + //Total points, not total cards
                key.CardsHeldWithPoints + "," +
                key.MoveIndex + "," +
                key.TrickIndex
            );
            _trickDiffSum += trickDiff;
            _handDiffSum += handDiff;
        }

        //[TestMethod]
        public void LearningVsBasicPlayer()
        {
            var predictor = CentroidResultPredictor.FromFile(@"C:\Temp\learningStatsStage1-2500hands.json");
            using (var sw = new StreamWriter(@"C:\Temp\learningVsBasicPlayer.txt"))
            {
                for (var noOfLearning = 1; noOfLearning <= 4; ++noOfLearning)
                {
                    var playerList = new List<IPlayer>();
                    var wins = new double[5];
                    for (var i = 0; i < 5; ++i)
                    {
                        //BasicPlayer lPlayer = i < noOfLearning ? new LearningPlayer(new Key2Generator(), predictor) : new BasicPlayer();
                        //playerList.Add(lPlayer);
                    }
                    var handNumber = 1 * 1000 * 1000;
                    PlayGame(playerList, handNumber, "", (object sender, EventArgs args) =>
                    {
                        var hand = sender as IHand;
                        var scores = hand.Scores();
                        for (var s = 0; s < scores.Count; ++s)
                        {
                            if (scores[playerList[s]] > 0)
                                wins[s] += 1;
                        }
                    });
                    sw.WriteLine(noOfLearning + " learning players.");
                    for (var i = 0; i < wins.Length; ++i)
                        sw.WriteLine("Player " + i + ": " + (wins[i]/handNumber).ToString("P3"));
                    sw.WriteLine();
                }
            }
        }

        private static void PlayGame(List<IPlayer> playerList, int handNumber, string saveLocation, EventHandler<EventArgs> del = null)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var rnd = new RandomWrapper();
            for (var g = 0; g < handNumber; ++g)
            {
                var game = repository.CreateGame("Poker", playerList, rnd);
                game.RearrangePlayers();
                var deck = new Deck(game, rnd);
                var picker = game.PlayNonHumans(deck) as ComputerPlayer;
                var buriedCards = picker != null ? picker.DropCardsForPick(deck) : new List<ICard>();
                var hand = new Hand(deck, picker, buriedCards);
                if (!String.IsNullOrWhiteSpace(saveLocation))
                    new LearningHelper(hand, saveLocation);
                if (del != null)
                    hand.OnHandEnd += del;
                while (!hand.IsComplete())
                {
                    var trick = new Trick(hand);
                    game.PlayNonHumans(trick);
                }
            }
        }
    }
}
