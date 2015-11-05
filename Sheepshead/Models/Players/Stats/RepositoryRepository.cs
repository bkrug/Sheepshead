using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Models.Players.Stats
{
    public class RepositoryRepository
    {
        public static string MOVE_SAVE_LOCATION = @"c:\temp\move-stat.json";
        public static string PICK_SAVE_LOCATION = @"c:\temp\pick-stat.json";
        public static string BURY_SAVE_LOCATION = @"c:\temp\bury-stat.json";

        private static RepositoryRepository _instance = new RepositoryRepository();

        private RepositoryRepository()
        {
            using (var reader = new StreamReaderWrapper(MOVE_SAVE_LOCATION))
                MoveStatRepository = MoveStatRepository.FromFile(reader);
            using (var reader = new StreamReaderWrapper(PICK_SAVE_LOCATION))
                PickStatRepository = PickStatRepository.FromFile(reader);
            using (var reader = new StreamReaderWrapper(BURY_SAVE_LOCATION))
                BuryStatRepository = BuryStatRepository.FromFile(reader);
        }

        public static RepositoryRepository Instance { get { return _instance; } }

        public MoveStatRepository MoveStatRepository { get; private set; }
        public PickStatRepository PickStatRepository { get; private set; }
        public BuryStatRepository BuryStatRepository { get; private set; }
    }
}