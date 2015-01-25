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
        void IncrementTrickResult(MoveStatUniqueKey key, bool wonTrick);
        void IncrementHandResult(MoveStatUniqueKey key, bool wonGame);
        MoveStat GetRecordedResults(MoveStatUniqueKey key);
    }

    public class MoveStatRepository : IMoveStatRepository
    {
        private static MoveStatRepository _instance = new MoveStatRepository();

        private Dictionary<MoveStatUniqueKey, MoveStat> _dict = new Dictionary<MoveStatUniqueKey, MoveStat>();

        public static string SaveLocation { get; set; }

        public static MoveStatRepository FromFile(IStreamReaderWrapper streamReader)
        {
            if (_instance != null && _instance._dict.Any())
                throw new InvalidOperationException("Cannot reinitialize Repository");
            var serializer = new JavaScriptSerializer();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var key = serializer.Deserialize<MoveStatUniqueKey>(line);
                var value = serializer.Deserialize<MoveStat>(streamReader.ReadLine());
                Instance._dict.Add(key, value);
            }
            Instance.SetupTimer();
            return Instance;
        }

        private void SetupTimer() 
        {
            var aTimer = new System.Timers.Timer(60 * 1000);
            aTimer.Elapsed += SaveToFile;
            aTimer.Enabled = true;
        }

        public static MoveStatRepository Instance { get { return _instance; } }

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

        public MoveStat GetRecordedResults(MoveStatUniqueKey key)
        {
            if (_dict.ContainsKey(key))
                return _dict[key];
            return new MoveStat();
        }

        /// <summary>
        /// To be called periodically from a timer that the constructor instantiated.
        /// </summary>
        protected void SaveToFile(Object source, ElapsedEventArgs e)
        {
            using (var writer = new StreamWriterWrapper(SaveLocation))
            {
                SaveToFile(writer);
            }
        }

        public void SaveToFile(IStreamWriterWrapper writer)
        {
            var serializer = new JavaScriptSerializer();
            foreach (var entry in _dict)
            {
                writer.WriteLine(serializer.Serialize(entry.Key));
                writer.WriteLine(serializer.Serialize(entry.Value));
            }
        }

        public void UnitTestRefresh()
        {
            if (!Assembly.GetCallingAssembly().FullName.Contains("Sheepshead.Tests"))
                throw new InvalidOperationException("Method must only be called from Unit Testing assembly.");
            _dict = new Dictionary<MoveStatUniqueKey, MoveStat>();
        }
    }
}

