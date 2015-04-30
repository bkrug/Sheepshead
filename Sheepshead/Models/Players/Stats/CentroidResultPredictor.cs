using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class CentroidResultPredictor
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
            return _centroidAndStats[nearestCentroid];
        }

        private MoveStatCentroid GetNearestCentroid(MoveStatUniqueKey key)
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
                throw new ApplicationException("No centroid found.");
            return bestMatch;
        }
    }

    public class TestClusterer
    {
        private IMoveStatRepository _repository;

        public TestClusterer(IMoveStatRepository repository)
        {
            _repository = repository;
        }

        //public double TestCluster()
        //{
            //var data = _repository.Keys;
            //var halfOfData = (int)(data.Count() / 2);
            //var numClusters = (int)(Math.Round(Math.Sqrt(halfOfData), 0));
            //var rnd = new Random();
            //var clusterer1 = new Clusterer(numClusters, rnd);
            //var clusterer2 = new Clusterer(numClusters, rnd);
            //var data1 = data.Take(halfOfData);
            //var data2 = data.Skip(halfOfData);
            //clusterer1.Cluster(data1.ToList());
            //clusterer2.Cluster(data2.ToList());
            //var results1 = clusterer1.Centroids;
            //var results2 = clusterer2.Centroids;

            ////Now compare the results and see how similar they are.
            //var count = 0;
            //double total = 0;
            //var predictor1 = new CentroidResultPredictor(clusterer1, _repository);
            //var predictor2 = new CentroidResultPredictor(clusterer2, _repository);
            //while (count < 1000 && count < halfOfData)
            //{
            //    var key = GenerateKey(rnd);
            //    total += (double)Math.Abs((decimal)
            //        (predictor1.GetAverageStat(key).TrickPortionWon - predictor2.GetAverageStat(key).TrickPortionWon));
            //}
            //return total / count;
        //}
    }
}