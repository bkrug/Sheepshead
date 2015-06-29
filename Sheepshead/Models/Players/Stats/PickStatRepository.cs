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
    public interface IPickStatRepository
    {
        List<PickStatUniqueKey> Keys { get; }
        void IncrementPickResult(PickStatUniqueKey key, bool wonHand);
        void IncrementPassResult(PickStatUniqueKey key, bool wonHand);
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

        public void IncrementPickResult(PickStatUniqueKey key, bool wonHand)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new PickStat();
            if (wonHand)
                ++_dict[key].PicksWon;
            ++_dict[key].HandsPicked;
        }

        public void IncrementPassResult(PickStatUniqueKey key, bool wonHand)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new PickStat();
            if (wonHand)
                ++_dict[key].PassedWon;
            ++_dict[key].HandsPassed;
        }
    }
}
