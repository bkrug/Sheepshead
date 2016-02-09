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
            var pickStatRepository = new PickStatRepository();
            var moveStatRepository = new MoveStatRepository();
            var buryStatRepository = new BuryStatRepository();
            var leasterStatRepository = new LeasterStatRepository();
            InstantiateLearningHelper learningDel = (IHand hand) => { return new LearningHelper(hand, SaveLocations.FIRST_SAVE, pickStatRepository, moveStatRepository, buryStatRepository, leasterStatRepository); };
            PlayGame(playerList, 500, learningDel);
            pickStatRepository.Save(@"c:\temp\basic-player-pick-stats.json");
            moveStatRepository.Save(@"c:\temp\basic-player-move-stats.json");
        }

        //[TestMethod]
        public void LearningHelper_GenerateKeys()
        {
            var movePredictorMock = new Mock<IStatResultPredictor>();
            movePredictorMock.Setup(m => m.GetWeightedStat(It.IsAny<MoveStatUniqueKey>())).Returns(null as MoveStat);
            var pickPredictorMock = new Mock<IPickResultPredictor>();
            var buryPredictorMock = new Mock<IBuryResultPredictor>();
            var leasterPredictorMock = new Mock<ILeasterResultPredictor>();
            using (_sw = new StreamWriter(@"C:\Temp\GeneratedKeys.csv"))
            {
                var playerList = new List<IPlayer>();
                var moveKeyGenerator = new MoveKeyGenerator();
                var pickKeyGenerator = new Mock<IPickKeyGenerator>();
                var buryKeyGenerator = new Mock<IBuryKeyGenerator>();
                var leasterKeyGenerator = new Mock<ILeasterKeyGenerator>();
                for (var i = 0; i < 5; ++i)
                {
                    var player = new LearningPlayer(moveKeyGenerator, movePredictorMock.Object, pickKeyGenerator.Object, pickPredictorMock.Object, buryKeyGenerator.Object, buryPredictorMock.Object, 
                        leasterKeyGenerator.Object, leasterPredictorMock.Object);
                    playerList.Add(player);
                    player.OnMove += (object sender, LearningPlayer.OnMoveEventArgs args) => {
                        var key = moveKeyGenerator.GenerateKey(args.Trick, sender as IPlayer, args.Card);
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
                var pickStatRepository = new PickStatRepository();
                var moveStatRepository = new MoveStatRepository();
                var buryStatRepository = new BuryStatRepository();
                var leasterStatRepository = new LeasterStatRepository();
                InstantiateLearningHelper learningDel = (IHand hand) => { return new LearningHelper(hand, @"c:\temp\LearningPlayerStage1.txt", pickStatRepository, moveStatRepository, buryStatRepository, leasterStatRepository); };
                PlayGame(playerList, 1, learningDel);
            }
        }

        //[TestMethod]
        public void LearningVsBasicPlayer()
        {
            MoveStatRepository moveRepository = RepositoryRepository.Instance.MoveStatRepository;
            PickStatRepository pickRepository = RepositoryRepository.Instance.PickStatRepository;
            BuryStatRepository buryRepository = RepositoryRepository.Instance.BuryStatRepository;
            LeasterStatRepository leasterRepository = RepositoryRepository.Instance.LeasterStatRepository;
            var movePredictor = new MoveStatResultPredictor(moveRepository);
            var guessPickRepository = new PickStatGuesser();
            var pickPredictor = new PickStatResultPredictor(pickRepository, guessPickRepository);
            var guessBuryRepository = new BuryStatGuesser();
            var buryPredictor = new BuryStatResultPredictor(buryRepository, guessBuryRepository);
            var leasterPredictor = new LeasterStatResultPredictor(leasterRepository);
            using (var sw = new StreamWriter(@"C:\Temp\learningVsBasicPlayer.txt"))
            {
                var playerList = new List<IPlayer>() {
                    new BasicPlayer(),
                    new LearningPlayer(new MoveKeyGenerator(), movePredictor, new PickKeyGenerator(), pickPredictor, new BuryKeyGenerator(), buryPredictor, new LeasterKeyGenerator(), leasterPredictor),
                    new BasicPlayer(),
                    new LearningPlayer(new MoveKeyGenerator(), movePredictor, new PickKeyGenerator(), pickPredictor, new BuryKeyGenerator(), buryPredictor, new LeasterKeyGenerator(), leasterPredictor),
                    new BasicPlayer()
                };
                var wins = new double[5];
                var handNumber = 1 * 1000 * 1000;
                var handsCompleted = 0;
                var nextReport = 2;
                sw.WriteLine("Players 1 and 3 are Learning Players");
                sw.Flush();
                var gameStartTime = DateTime.Now;
                var handSummaries = @"C:\Temp\learningVsBasicPlayerHandSummaries.txt";
                File.Delete(handSummaries);
                InstantiateLearningHelper learningDel = (IHand hand) => { return new LearningHelper(hand, handSummaries, pickRepository, moveRepository, buryRepository, leasterRepository); };
                PlayGame(playerList, handNumber, learningDel, (object sender, EventArgs args) =>
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
                        sw.WriteLine("Average game time: " + ((DateTime.Now - gameStartTime).TotalSeconds / handsCompleted));
                        sw.WriteLine();
                        sw.Flush();
                    }
                });
            }
        }

        //[TestMethod]
        public void RecordingPlayerOnly()
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < 5; ++i)
                playerList.Add(new RecordingPlayer());
            var pickStatRepository = new PickStatRepository();
            var moveStatRepository = new MoveStatRepository();
            var buryStatRepository = new BuryStatRepository();
            var leasterStatRepository = new LeasterStatRepository();
            InstantiateLearningHelper learningDel = (IHand hand) => { return new LearningHelper(hand, @"c:\temp\recordingPlayerSummaries.json", pickStatRepository, moveStatRepository, buryStatRepository, leasterStatRepository); };
            PlayGame(playerList, 1000, learningDel);
            pickStatRepository.Save(@"c:\temp\recording-pick-stats.json");
            moveStatRepository.Save(@"c:\temp\recording-move-stats.json");
        }

        private static void PlayGame(List<IPlayer> playerList, int handNumber, InstantiateLearningHelper learningDel = null, EventHandler<EventArgs> handEndDel = null)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var rnd = new RandomWrapper();
            for (var g = 0; g < handNumber; ++g)
            {
                var game = repository.CreateGame("Poker", playerList, rnd);
                game.RearrangePlayers();
                var deck = new Deck(game, rnd);
                var picker = game.PlayNonHumanPickTurns(deck) as ComputerPlayer;
                var buriedCards = picker != null ? picker.DropCardsForPick(deck) : new List<ICard>();
                var hand = new Hand(deck, picker, buriedCards);
                if (learningDel != null)
                    learningDel(hand);
                if (handEndDel != null)
                    hand.OnHandEnd += handEndDel;
                while (!hand.IsComplete())
                {
                    var trick = new Trick(hand);
                    game.PlayNonHumans(trick);
                }
            }
        }

        delegate LearningHelper InstantiateLearningHelper(IHand hand);
    }
}
