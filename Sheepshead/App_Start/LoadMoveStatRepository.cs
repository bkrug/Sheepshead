using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheepshead
{
    public class LoadMoveStatRepository
    {
        private const string SAVE_LOCATION = @"c:\temp\game-stat.json";

        public static void Load() {
            MoveStatRepository.SaveLocation = SAVE_LOCATION;
            using (var reader = new StreamReaderWrapper(SAVE_LOCATION))
                MoveStatRepository.FromFile(reader);
        }
    }
}