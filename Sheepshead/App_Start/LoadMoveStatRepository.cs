using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead
{
    public class LoadMoveStatRepository
    {
        public static void Load()
        {
            var superRepo = RepositoryRepository.Instance;
        }
    }
}