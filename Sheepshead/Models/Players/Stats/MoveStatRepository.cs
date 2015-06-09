using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Sheepshead.Models.Wrappers;
using System.Web.Script.Serialization;
using System.Timers;

namespace Sheepshead.Models.Players.Stats
{
    public interface IMoveStatRepository
    {
        List<MoveStatUniqueKey> Keys { get; }
        void IncrementTrickResult(MoveStatUniqueKey key, bool wonTrick);
        void IncrementHandResult(MoveStatUniqueKey key, bool wonGame);
        MoveStat GetRecordedResults(MoveStatUniqueKey key);
    }

    public class MoveStatRepository : HandRepository<MoveStatUniqueKey, MoveStat>, IMoveStatRepository
    {
        protected override MoveStat CreateDefaultStat()
        {
            return new MoveStat();
        }

        public MoveStatRepository() { }

        public MoveStatRepository(string saveLocation) : base(saveLocation) { }

        public static MoveStatRepository FromFile(string filename, IStreamReaderWrapper streamReader)
        {
            var instance = new MoveStatRepository(filename);
            var serializer = new JavaScriptSerializer();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var key = serializer.Deserialize<MoveStatUniqueKey>(line);
                var value = serializer.Deserialize<MoveStat>(streamReader.ReadLine());
                instance._dict.Add(key, value);
            }
            instance.SetupTimer();
            return instance;
        }

        private void SetupTimer()
        {
            var aTimer = new System.Timers.Timer(60 * 1000);
            aTimer.Elapsed += SaveToFile;
            aTimer.Enabled = true;
        }

        public void IncrementTrickResult(MoveStatUniqueKey key, bool wonTrick)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new MoveStat();
            if (wonTrick)
                ++_dict[key].TricksWon;
            ++_dict[key].TricksTried;
        }

        public void IncrementHandResult(MoveStatUniqueKey key, bool wonGame)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new MoveStat();
            if (wonGame)
                ++_dict[key].HandsWon;
            ++_dict[key].HandsTried;
        }
    }

    public class RepositoryRepository
    {
        public static string MOVE_SAVE_LOCATION = @"c:\temp\game-stat.json";

        private static RepositoryRepository _instance = new RepositoryRepository();

        private RepositoryRepository()
        {
            using (var reader = new StreamReaderWrapper(MOVE_SAVE_LOCATION))
                MoveStatRepository = MoveStatRepository.FromFile(MOVE_SAVE_LOCATION, reader);
        }

        internal static RepositoryRepository Instance { get { return _instance; } }

        public MoveStatRepository MoveStatRepository { get; private set; }
    }
}
