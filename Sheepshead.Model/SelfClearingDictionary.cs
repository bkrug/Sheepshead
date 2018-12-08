using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Sheepshead.Model
{
    public class SelfClearingDictionary : IDictionary<Guid, IGame>
    {
        private Timer _timer;
        private TimeSpan _minimumStorageTime;
        private IDictionary<Guid, IGame> _dictionary = new Dictionary<Guid, IGame>();
        private IDictionary<Guid, DateTime> _accessTimes = new Dictionary<Guid, DateTime>();

        public SelfClearingDictionary() : this(new TimeSpan(24, 0, 0))
        {
        }

        public SelfClearingDictionary(TimeSpan minimumStorageTime)
        {
            _minimumStorageTime = minimumStorageTime;
            var checkInterval = new TimeSpan(_minimumStorageTime.Ticks / 4);
            _timer = new Timer();
            _timer.Elapsed += DeleteUnusedEntries;
            _timer.Interval = checkInterval.TotalSeconds;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public IGame this[Guid key] {
            get
            {
                UpdateTimestamp(key);
                return _dictionary[key];
            }
            set
            {
                UpdateTimestamp(key);
                _dictionary[key] = value;
            }
        }

        public ICollection<Guid> Keys => _dictionary.Keys;

        public ICollection<IGame> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public void Add(Guid key, IGame value)
        {
            UpdateTimestamp(key);
            _dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<Guid, IGame> item)
        {
            UpdateTimestamp(item.Key);
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<Guid, IGame> item)
        {
            return _dictionary.Contains(item);
        }

        public bool ContainsKey(Guid key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Guid, IGame>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
            foreach (var entry in array)
                UpdateTimestamp(entry.Key);
        }

        public IEnumerator<KeyValuePair<Guid, IGame>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(Guid key)
        {
            _accessTimes.Remove(key);
            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<Guid, IGame> item)
        {
            _accessTimes.Remove(item.Key);
            return _dictionary.Remove(item);
        }

        public bool TryGetValue(Guid key, out IGame value)
        {
            if (_dictionary.TryGetValue(key, out value))
            {
                UpdateTimestamp(key);
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        private void UpdateTimestamp(Guid guid)
        {
            if (_accessTimes.ContainsKey(guid))
                _accessTimes[guid] = DateTime.Now;
            else
                _accessTimes.Add(guid, DateTime.Now);
        }

        private void DeleteUnusedEntries(Object source, ElapsedEventArgs e)
        {
            _accessTimes.Where(at => (DateTime.Now - at.Value) > _minimumStorageTime)
                .ToList()
                .ForEach(at =>
                {
                    _dictionary.Remove(at.Key);
                    _accessTimes.Remove(at.Key);
                });
        }
    }
}
