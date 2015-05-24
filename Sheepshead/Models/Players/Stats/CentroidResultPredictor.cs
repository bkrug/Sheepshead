using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace Sheepshead.Models.Players.Stats
{
    public interface ICentroidResultPredictor
    {
        MoveStat GetPrediction(MoveStatUniqueKey key);
    }

    public class CentroidResultPredictor : ICentroidResultPredictor
    {
        private Dictionary<int, Dictionary<MoveStatCentroid, MoveStat>> _centroidAndStats;

        private CentroidResultPredictor() { }

        public CentroidResultPredictor(Dictionary<int, Dictionary<MoveStatCentroid, MoveStat>> centroidAndStats)
        {
            _centroidAndStats = centroidAndStats;
        }

        public MoveStat GetPrediction(MoveStatUniqueKey key)
        {
            var roomNo = key.CentroidRoom;
            var nearestCentroid = GetNearestCentroid(key);
            return nearestCentroid.HasValue ? _centroidAndStats[roomNo][nearestCentroid.Value] : null;
        }

        private MoveStatCentroid? GetNearestCentroid(MoveStatUniqueKey key)
        {
            var minDistance = Double.MaxValue;
            MoveStatCentroid bestMatch = new MoveStatCentroid();
            if (!_centroidAndStats.ContainsKey(key.CentroidRoom))
                return null;
            foreach (var centroid in _centroidAndStats[key.CentroidRoom].Keys)
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

        public void SaveStats(string filename)
        {
            var jss = new JavaScriptSerializer();
            var dictWithStringKey = new Dictionary<string, Dictionary<string, string>>();
            foreach (var centroidAndStatElem in _centroidAndStats)
            {
                var innerDict = new Dictionary<string, string>();
                foreach (var statElem in centroidAndStatElem.Value)
                    innerDict.Add(jss.Serialize(statElem.Key), jss.Serialize(statElem.Value));
                dictWithStringKey.Add(centroidAndStatElem.Key.ToString(), innerDict);
            }
            var text = jss.Serialize(dictWithStringKey);
            using (var sw = new StreamWriter(filename))
                sw.WriteLine(text);
        }
    }
}