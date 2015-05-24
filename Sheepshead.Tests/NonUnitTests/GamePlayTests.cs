﻿using System;
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
            var predictorMock = new Mock<ICentroidResultPredictor>();
            predictorMock.Setup(m => m.GetPrediction(It.IsAny<MoveStatUniqueKey>())).Returns(null as MoveStat);
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
                            key.OffenseSide + "," +
                            key.PickerDone + "," +
                            key.PartnerDone + "," +
                            key.PointsInTrick + "," +
                            key.HighestRankInTrick + "," +
                            key.WinningSide + "," +
                            key.ThisCardMorePowerful + "," +
                            key.MorePowerfulUnknownCards + "," +
                            key.RemainingUnknownPoints + "," +
                            key.MorePowerfulHeld + "," +
                            key.PointsHeld + "," +
                            key.CardsHeldWithPoints + "," +
                            key.MoveIndex + "," +
                            key.TrickIndex
                        );
                    };
                }
                _sw.WriteLine("Card,Rank,Points,Offense Side,Picker Done,Partner Done,Points In Trick,Highest Rank In Trick,Winning Side,This Card More Power,More Powerful Unknown Cards,Remaining Unknown Points,More Powerful Held,Points Held,Cards Held With Points,Move Index,Trick Index");
                PlayGame(playerList, 1, @"c:\temp\LearningPlayerStage1.txt");
            }
        }

        [TestMethod]
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
                    var lPlayer = new LearningPlayer(new KeyGenerator(), _predictor1);
                    playerList.Add(lPlayer);
                    lPlayer.OnMove += lPlayer_OnMove;
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
            var generator = new KeyGenerator();
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

        private static void PlayGame(List<IPlayer> playerList, int handNumber, string saveLocation)
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
                new LearningHelper(hand, saveLocation);
                while (!hand.IsComplete())
                {
                    var trick = new Trick(hand);
                    game.PlayNonHumans(trick);
                }
            }
        }
    }
}
