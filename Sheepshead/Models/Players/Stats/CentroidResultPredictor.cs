using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public interface ICentroidResultPredictor
    {
        MoveStat GetPrediction(MoveStatUniqueKey key);
    }

    public class CentroidResultPredictor : ICentroidResultPredictor
    {
        private Dictionary<MoveStatCentroid, MoveStat> _centroidAndStats;

        private CentroidResultPredictor() { }

        public CentroidResultPredictor(Dictionary<MoveStatCentroid, MoveStat> centroidAndStats)
        {
            _centroidAndStats = centroidAndStats;
        }

        public MoveStat GetPrediction(MoveStatUniqueKey key)
        {
            var nearestCentroid = GetNearestCentroid(key);
            return nearestCentroid.HasValue ? _centroidAndStats[nearestCentroid.Value] : null;
        }

        private MoveStatCentroid? GetNearestCentroid(MoveStatUniqueKey key)
        {
            var minDistance = Double.MaxValue;
            MoveStatCentroid bestMatch = new MoveStatCentroid();
            foreach (var centroid in _centroidAndStats.Keys)
            {
                var curDistance = ClusterUtils.Distance(key, centroid);
                if (curDistance < minDistance)
                {
                    minDistance = curDistance;
                    bestMatch = centroid;
                }
            }
            if (minDistance == Double.MaxValue)
                return null;
            return bestMatch;
        }
    }
}