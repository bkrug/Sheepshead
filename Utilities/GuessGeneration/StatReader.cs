using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Sheepshead.Models.Wrappers;

namespace Utilities.GuessGeneration
{
    public class StatReader<K, V>
    {
        protected Dictionary<K, V> _dict = new Dictionary<K, V>();
        public virtual List<K> Keys { get { return _dict.Keys.ToList(); } }

        public static StatReader<K, V> FromFile(IStreamReaderWrapper streamReader)
        {
            var instance = new StatReader<K, V>();
            var serializer = new JavaScriptSerializer();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var key = serializer.Deserialize<K>(line);
                var value = serializer.Deserialize<V>(streamReader.ReadLine());
                instance._dict.Add(key, value);
            }
            return instance;
        }

        public void NormalizeStats(int numberOfHands, StatMath statMathDelegat)
        {
            foreach (var key in _dict.Keys)
                statMathDelegat(numberOfHands, _dict[key]);
        }

        public delegate void StatMath(int numberOfHands, V stat);

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
    }
}
