using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using Sheepshead.Models.Wrappers;
using System.Web.Script.Serialization;
using System.Timers;
using System.Configuration;
using System.IO;

namespace Sheepshead.Models.Players.Stats
{
    public interface IStatRepository<K, V>
    {
        List<K> Keys { get; }
        V GetRecordedResults(K key);
    }

    public abstract class StatRepository<K, V> : IStatRepository<K, V>
    {
        protected Dictionary<K, V> _dict = new Dictionary<K, V>();
        public virtual List<K> Keys { get { return _dict.Keys.ToList(); } }
#if DEBUG
        private int _saveFrequency = 1;  // in minutes
#else
        private int _saveFrequency = Int32.Parse(ConfigurationManager.AppSettings["SaveFrequencyInMinutes"]);  // in minutes
#endif

        protected abstract V CreateDefaultStat();

        public virtual V GetRecordedResults(K key)
        {
            if (_dict.ContainsKey(key))
                return _dict[key];
            return CreateDefaultStat();
        }

        public virtual string SaveLocation { get; private set; }

        protected static StatRepository<K, V> FromFile(StatRepository<K, V> instance, IStreamReaderWrapper streamReader)
        {
            instance.SaveLocation = streamReader.Filename;
            var serializer = new JavaScriptSerializer();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var key = serializer.Deserialize<K>(line);
                var value = serializer.Deserialize<V>(streamReader.ReadLine());
                instance._dict.Add(key, value);
            }
            instance.SetupTimer();
            return instance;
        }


        protected void SetupTimer()
        {
            var aTimer = new System.Timers.Timer(_saveFrequency * 60 * 1000);
            aTimer.Elapsed += SaveToFile;
            aTimer.Enabled = true;
        }

        /// <summary>
        /// To be called periodically from a timer that the constructor instantiated.
        /// </summary>
        protected virtual void SaveToFile(Object source, ElapsedEventArgs e)
        {
            using (var writer = new StreamWriterWrapper(SaveLocation, false))
                SaveToFile(writer);
        }

        public void Save(string saveLocation)
        {
            using (var writer = new StreamWriterWrapper(saveLocation, false))
                SaveToFile(writer);
        }

        public virtual void SaveToFile(IStreamWriterWrapper writer)
        {
            var serializer = new JavaScriptSerializer();
            foreach (var entry in _dict)
            {
                writer.WriteLine(serializer.Serialize(entry.Key));
                writer.WriteLine(serializer.Serialize(entry.Value));
            }
            writer.Flush();
        }

        public void Close()
        {
            using (var writer = new StreamWriterWrapper(SaveLocation, false))
                SaveToFile(writer);
        }
    }
}
