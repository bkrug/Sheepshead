using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using Sheepshead.Models.Wrappers;
using System.Web.Script.Serialization;
using System.Timers;

namespace Sheepshead.Models.Players.Stats
{
    public interface IBuryStatRepository : IStatRepository<BuryStatUniqueKey, BuryStat>
    {
        void IncrementResult(BuryStatUniqueKey key, int handPoints);
    }

    public class BuryStatRepository : StatRepository<BuryStatUniqueKey, BuryStat>, IBuryStatRepository
    {
        protected override BuryStat CreateDefaultStat()
        {
            return new BuryStat();
        }

        public static BuryStatRepository FromFile(IStreamReaderWrapper streamReader)
        {
            var instance = new BuryStatRepository();
            return (BuryStatRepository)StatRepository<BuryStatUniqueKey, BuryStat>.FromFile(instance, streamReader);
        }

        public void IncrementResult(BuryStatUniqueKey key, int handPoints)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new BuryStat();
            _dict[key].TotalPoints += handPoints;
            ++_dict[key].HandsPicked;
        }
    }
}
