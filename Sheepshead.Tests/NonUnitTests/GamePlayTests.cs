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
        private StreamWriter _sw;

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
        public void LearningVsBasicPlayer()
        {
            var repository = MoveStatRepository.Instance;
            var predictor = new StatResultPredictor(repository);
            using (var sw = new StreamWriter(@"C:\Temp\learningVsBasicPlayer.txt"))
            {
                var playerList = new List<IPlayer>() {
                    new BasicPlayer(),
                    new LearningPlayer(new KeyGenerator(), predictor),
                    new BasicPlayer(),
                    new LearningPlayer(new KeyGenerator(), predictor),
                    new BasicPlayer()
                };
                var wins = new double[5];
                var handNumber = 1 * 1000 * 1000;
                var handsCompleted = 0;
                var nextReport = 2;
                sw.WriteLine("Players 1 and 3 are Learning Players");
                sw.Flush();
                PlayGame(playerList, handNumber, @"C:\Temp\learningVsBasicPlayerHandSummaries.txt", (object sender, EventArgs args) =>
                {
                    ++handsCompleted;
                    var hand = sender as IHand;
                    var scores = hand.Scores();
                    for (var s = 0; s < scores.Count; ++s)
                    {
                        if (scores[playerList[s]] > 0)
                            wins[s] += 1;
                    }
                    if (handsCompleted == nextReport)
                    {
                        nextReport *= 2;
                        //nextReport *= (int)Math.Round(Math.Sqrt(10));
                        sw.WriteLine("Games Played: " + handsCompleted);
                        for (var i = 0; i < wins.Length; ++i)
                            sw.WriteLine("Player " + i + ": " + (wins[i] / handsCompleted).ToString("P3"));
                        sw.WriteLine();
                        sw.Flush();
                    }
                });
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
