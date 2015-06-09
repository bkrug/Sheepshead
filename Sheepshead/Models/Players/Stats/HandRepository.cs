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
    public interface IHandRepository<K, V>
    {
        List<K> Keys { get; }
        V GetRecordedResults(K key);
    }

    public abstract class HandRepository<K, V> : IHandRepository<K, V>
    {
        public HandRepository() {
        }

        public HandRepository(string saveLocation)
        {
            SaveLocation = saveLocation;
        }

        protected Dictionary<K, V> _dict = new Dictionary<K, V>();
        public virtual List<K> Keys { get { return _dict.Keys.ToList(); } }

        protected abstract V CreateDefaultStat();

        public virtual V GetRecordedResults(K key)
        {
            if (_dict.ContainsKey(key))
                return _dict[key];
            return CreateDefaultStat();
        }

        public virtual string SaveLocation { get; private set; }

        /// <summary>
        /// To be called periodically from a timer that the constructor instantiated.
        /// </summary>
        protected virtual void SaveToFile(Object source, ElapsedEventArgs e)
        {
            using (var writer = new StreamWriterWrapper(SaveLocation))
            {
                SaveToFile(writer);
            }
        }

        public virtual void SaveToFile(IStreamWriterWrapper writer)
        {
            var serializer = new JavaScriptSerializer();
            foreach (var entry in _dict)
            {
                writer.WriteLine(serializer.Serialize(entry.Key));
                writer.WriteLine(serializer.Serialize(entry.Value));
            }
        }

        public virtual void UnitTestRefresh()
        {
            if (!Assembly.GetCallingAssembly().FullName.Contains("Sheepshead.Tests"))
                throw new InvalidOperationException("Method must only be called from Unit Testing assembly.");
            _dict = new Dictionary<K, V>();
        }
    }
}
