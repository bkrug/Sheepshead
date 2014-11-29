using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class MoveStatRepository
    {
        private static MoveStatRepository _instance = new MoveStatRepository();

        private Dictionary<MoveStatUniqueKey, MoveStat> _dict = new Dictionary<MoveStatUniqueKey, MoveStat>();

        private MoveStatRepository()
        {
        }

        public static MoveStatRepository Instance { get { return _instance; } }

        public void IncrementTrickResult(MoveStatUniqueKey key, bool wonTrick)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new MoveStat();
            if (wonTrick)
                ++_dict[key].TricksWon;
            ++_dict[key].TricksTried;
        }

        public void IncrementGameResult(MoveStatUniqueKey key, bool wonGame)
        {
            if (!_dict.ContainsKey(key))
                _dict[key] = new MoveStat();
            if (wonGame)
                ++_dict[key].GamesWon;
            ++_dict[key].GamesTried;
        }

        public double GetWeightedScore(MoveStatUniqueKey key)
        {
            throw new NotImplementedException();
        }
    }
}