using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using Sheepshead.Models.Wrappers;
using System.Web.Script.Serialization;
using System.Timers;

namespace Sheepshead.Models.Players.Stats
{
    public interface ILeasterStatRepository : IStatRepository<LeasterStatUniqueKey, LeasterStat>
    {
        void IncrementHandResult(LeasterStatUniqueKey key, bool wonHand);
    }

    public class LeasterStatRepository : StatRepository<LeasterStatUniqueKey, LeasterStat>, ILeasterStatRepository
    {
        protected override LeasterStat CreateDefaultStat()
        {
            return new LeasterStat();
        }

        public static LeasterStatRepository FromFile(IStreamReaderWrapper streamReader)
        {
            var instance = new LeasterStatRepository();
            return (LeasterStatRepository)StatRepository<LeasterStatUniqueKey, LeasterStat>.FromFile(instance, streamReader);
        }

        public void IncrementHandResult(LeasterStatUniqueKey key, bool wonHand)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new LeasterStat();
            if (wonHand)
                ++_dict[key].HandsWon;
            ++_dict[key].HandsTried;
        }
    }
}
