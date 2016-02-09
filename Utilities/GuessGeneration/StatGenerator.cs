using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Utilities.GuessGeneration
{
    public class StatGenerator
    {
        public MoveStatRepository MoveStatRepository { get; private set; }
        public PickStatRepository PickStatRepository { get; private set; }
        public BuryStatRepository BuryStatRepository { get; private set; }
        public LeasterStatRepository LeasterStatRepository { get; private set; }
        public static string MOVE_SAVE_LOCATION = "move-stat.json";
        public static string PICK_SAVE_LOCATION = "pick-stat.json";
        public static string BURY_SAVE_LOCATION = "bury-stat.json";
        public static string LEASTER_SAVE_LOCATION = "lester-stat.json";

        public void GenerateStats(string basePath, int handsToPlay)
        {
            basePath = basePath.TrimEnd('\\').TrimEnd('/') + '\\';
            InitRepositories(basePath);
            var movePredictor = new MoveStatResultPredictor(MoveStatRepository);
            var guessPickRepository = new PickStatGuesser();
            var pickPredictor = new PickStatResultPredictor(PickStatRepository, guessPickRepository);
            var guessBuryRepository = new BuryStatGuesser();
            var buryPredictor = new BuryStatResultPredictor(BuryStatRepository, guessBuryRepository);
            var leasterPredictor = new LeasterStatResultPredictor(LeasterStatRepository);
            using (var sw = new StreamWriter(basePath + "learningVsBasicPlayer.txt"))
            {
                var playerList = new List<IPlayer>() {
                    new BasicPlayer(),
                    new LearningPlayer(new MoveKeyGenerator(), movePredictor, new PickKeyGenerator(), pickPredictor, new BuryKeyGenerator(), buryPredictor, new LeasterKeyGenerator(), leasterPredictor),
                    new BasicPlayer(),
                    new LearningPlayer(new MoveKeyGenerator(), movePredictor, new PickKeyGenerator(), pickPredictor, new BuryKeyGenerator(), buryPredictor, new LeasterKeyGenerator(), leasterPredictor),
                    new BasicPlayer()
                };
                sw.WriteLine("Players 1 and 3 are Learning Players");
                sw.Flush();
                var wins = new double[5];
                var handsCompleted = 0;
                var nextReport = 2;
                var gameStartTime = DateTime.Now;
                var handSummaries = basePath + "learningVsBasicPlayerHandSummaries.txt";
                InstantiateLearningHelper learningDel = (IHand hand) => 
                {
                    return new LearningHelper(hand, handSummaries, PickStatRepository, MoveStatRepository, BuryStatRepository, LeasterStatRepository);
                };
                EventHandler<EventArgs> onHandEnd = (object sender, EventArgs args) =>
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
                        sw.WriteLine("Games Played: " + handsCompleted);
                        for (var i = 0; i < wins.Length; ++i)
                            sw.WriteLine("Player " + i + ": " + (wins[i] / handsCompleted).ToString("P3"));
                        sw.WriteLine("Average game time: " + ((DateTime.Now - gameStartTime).TotalSeconds / handsCompleted));
                        sw.WriteLine();
                        sw.Flush();
                    }
                };
                PlayGame(playerList, handsToPlay, learningDel, onHandEnd);
            }
            SaveRepositoryData();
        }

        private void InitRepositories(string basePath)
        {
            using (var reader = new StreamReaderWrapper(basePath + MOVE_SAVE_LOCATION))
                MoveStatRepository = MoveStatRepository.FromFile(reader);
            using (var reader = new StreamReaderWrapper(basePath + PICK_SAVE_LOCATION))
                PickStatRepository = PickStatRepository.FromFile(reader);
            using (var reader = new StreamReaderWrapper(basePath + BURY_SAVE_LOCATION))
                BuryStatRepository = BuryStatRepository.FromFile(reader);
            using (var reader = new StreamReaderWrapper(basePath + LEASTER_SAVE_LOCATION))
                LeasterStatRepository = LeasterStatRepository.FromFile(reader);
        }

        private void SaveRepositoryData()
        {
            MoveStatRepository.Close();
            PickStatRepository.Close();
            BuryStatRepository.Close();
            LeasterStatRepository.Close();
        }

        private static void PlayGame(List<IPlayer> playerList, int handsToPlay, InstantiateLearningHelper learningDel, EventHandler<EventArgs> handEndDel)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var rnd = new RandomWrapper();
            var game = repository.CreateGame("Poker", playerList, rnd);
            game.RearrangePlayers();
            for (var g = 0; g < handsToPlay; ++g)
            {
                var deck = new Deck(game, rnd);
                var picker = game.PlayNonHumanPickTurns(deck) as ComputerPlayer;
                var buriedCards = picker != null ? picker.DropCardsForPick(deck) : new List<ICard>();
                var hand = new Hand(deck, picker, buriedCards);
                learningDel(hand);
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
