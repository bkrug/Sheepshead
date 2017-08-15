using System;
using System.Collections.Generic;

namespace Sheepshead.Model
{
    public static class PlayerDictionary
    {
        private static Dictionary<string, DateTime> _playerExpiration = new Dictionary<string, DateTime>();
        private static TimeSpan _expirationPeriod = new TimeSpan(2, 0, 0);

        public static string GeneratePlayerKey()
        {
            var guid = Guid.NewGuid();
            _playerExpiration.Add(guid.ToString(), DateTime.Now.Add(_expirationPeriod));
            return guid.ToString();
        }

        public static bool PlayerFound(Guid key)
        {
            var guid = key.ToString();
            if (!_playerExpiration.ContainsKey(guid))
                return false;
            if (_playerExpiration[guid] < DateTime.Now)
            {
                _playerExpiration.Remove(key.ToString());
                return false;
            }
            _playerExpiration[guid] = DateTime.Now.Add(_expirationPeriod);
            return true;
        }
    }
}
