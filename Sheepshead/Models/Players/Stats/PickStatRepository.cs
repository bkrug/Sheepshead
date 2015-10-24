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
    public interface IPickStatRepository : IStatRepository<PickStatUniqueKey, PickStat>
    {
        List<PickStatUniqueKey> Keys { get; }
        void IncrementPickResult(PickStatUniqueKey key, int points);
        void IncrementPassResult(PickStatUniqueKey key, int points);
        PickStat GetRecordedResults(PickStatUniqueKey key);
    }

    public class PickStatRepository : StatRepository<PickStatUniqueKey, PickStat>, IPickStatRepository
    {
        protected override PickStat CreateDefaultStat()
        {
            return new PickStat();
        }

        public static PickStatRepository FromFile(IStreamReaderWrapper streamReader)
        {
            var instance = new PickStatRepository();
            return (PickStatRepository)StatRepository<PickStatUniqueKey, PickStat>.FromFile(instance, streamReader);
        }

        public void IncrementPickResult(PickStatUniqueKey key, int points)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new PickStat();
            _dict[key].TotalPickPoints += points;
            ++_dict[key].HandsPicked;
        }

        public void IncrementPassResult(PickStatUniqueKey key, int points)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new PickStat();
            _dict[key].TotalPassedPoints += points;
            ++_dict[key].HandsPassed;
        }
    }
}
