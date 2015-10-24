using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    //The Generic T must be the same class being defined
    public interface IStat<T> where T : IStat<T>
    {
        void AddOtherStat(T otherStat);
    }

    public interface IStatUniqueKey
    {
    }
}