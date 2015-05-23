using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Models.Players.Stats
{
    public class InnerClusterer
    {
        private int _numClusters;
        private int[] _clustering;
        private MoveStatCentroid[] _centroids;
        private IRandomWrapper _rnd;

        public InnerClusterer(int numClusters, IRandomWrapper rnd)
        {
            _numClusters = numClusters;
            _centroids = new MoveStatCentroid[numClusters];
            _rnd = rnd;
        }

        public ClusterResult Cluster(List<MoveStatUniqueKey> data)
        {
            if (!data.Any())
                return new ClusterResult()
                {
                    Data = data,
                    ClusterIndicies = new int[] { },
                    Centroids = new List<MoveStatCentroid>()
                };
            var weighedData = WeighData(data);
            var numTuples = data.Count();
            _clustering = new int[numTuples];

            for (int k = 0; k < _numClusters; ++k)
                _centroids[k] = new MoveStatCentroid();

            InitRandom(weighedData);

            var changed = true;
            var maxCount = numTuples * 10;
            var ct = 0;
            while (changed && ct <= maxCount)
            {
                ++ct;
                UpdateCentroids(weighedData);
                changed = UpdateClustering(weighedData);
            }
            _centroids = UnWeighCentroids(_centroids).ToArray();
            var unWeighedData = UnWeighCentroids(weighedData.ToArray());
            UpdateClustering(unWeighedData);

            var clusterResult = new ClusterResult()
            {
                Data = data,
                ClusterIndicies = new int[_clustering.Length],
                Centroids = _centroids.ToList()
            };
            Array.Copy(_clustering, clusterResult.ClusterIndicies, _clustering.Length);
            return clusterResult;
        }

        private List<MoveStatCentroid> WeighData(List<MoveStatUniqueKey> data) {
            var weighedData = new List<MoveStatCentroid>();
            for (var i = 0; i < data.Count(); ++i )
            {
                var key = new MoveStatCentroid();
                key.PointsInTrick = data[i].PointsInTrick * 120.0 / 44;
                key.HighestRankInTrick = data[i].HighestRankInTrick * 120.0 / 32;
                key.MorePowerfulUnknownCards = data[i].MorePowerfulUnknownCards * 120.0 / 26;
                key.RemainingUnknownPoints = data[i].RemainingUnknownPoints * 120.0 / 120;
                key.MorePowerfulHeld = data[i].MorePowerfulHeld * 120.0 / 5;
                key.PointsHeld = data[i].PointsHeld * 120.0 / 54;
                key.CardsHeldWithPoints = data[i].CardsHeldWithPoints * 120.0 / 5;
                key.MoveIndex = data[i].MoveIndex * 120.0 / 5;
                key.TrickIndex = data[i].TrickIndex * 120.0 / 6;
                weighedData.Add(key);
            }
            return weighedData;
        }

        private List<MoveStatCentroid> UnWeighCentroids(MoveStatCentroid[] centroids)
        {
            List<MoveStatCentroid> list = new List<MoveStatCentroid>();
            for (var i = 0; i < centroids.Length; ++i)
            {
                var key = centroids[i];
                key.PointsInTrick *= 44.0 / 120;
                key.HighestRankInTrick *= 32.0 / 120.0;
                key.MorePowerfulUnknownCards *= 26.0 / 120.0;
                key.RemainingUnknownPoints *= 120.0 / 120.0;
                key.MorePowerfulHeld *= 5.0 / 120.0;
                key.PointsHeld *= 54.0 / 120.0;
                key.CardsHeldWithPoints *= 5.0 / 120.0;
                key.MoveIndex *= 5.0 / 120.0;
                key.TrickIndex *= 6.0 / 120.0;
                list.Add(key);
            }
            return list;
        }

        private void InitRandom(List<MoveStatCentroid> data)
        {
            int numTuples = data.Count();

            int clusterID = 0;
            for (int i = 0; i < numTuples; ++i)
            {
                _clustering[i] = clusterID++;
                if (clusterID == _numClusters)
                    clusterID = 0;
            }
            for (int i = 0; i < numTuples; ++i)
            {
                int r = _rnd.Next(i, _clustering.Count());
                int tmp = _clustering[r];
                _clustering[r] = _clustering[i];
                _clustering[i] = tmp;
            }
        }

        //This accepts list of centroid instead of MoveStateKeys because it must accept weighed data.
        //That is, the data must be double's and not int's.
        private void UpdateCentroids(List<MoveStatCentroid> data)
        {
            int[] clusterCounts = new int[_numClusters];
            for (int i = 0; i < data.Count; ++i)
            {
                int clusterID = _clustering[i];
                ++clusterCounts[clusterID];
            }

            //zero-out this centroids so it can be used as scratch
            for (var k = 0; k < _centroids.Length; ++k)
                _centroids[k] = new MoveStatCentroid();

            for (var i = 0; i < data.Count; ++i)
            {
                int clusterID = _clustering[i];
                _centroids[clusterID].PointsInTrick += data[i].PointsInTrick;
                _centroids[clusterID].HighestRankInTrick += data[i].HighestRankInTrick;
                _centroids[clusterID].MorePowerfulUnknownCards += data[i].MorePowerfulUnknownCards;
                _centroids[clusterID].RemainingUnknownPoints += data[i].RemainingUnknownPoints;
                _centroids[clusterID].MorePowerfulHeld += data[i].MorePowerfulHeld;
                _centroids[clusterID].PointsHeld += data[i].PointsHeld;
                _centroids[clusterID].CardsHeldWithPoints += data[i].CardsHeldWithPoints;
                _centroids[clusterID].MoveIndex += data[i].MoveIndex;
                _centroids[clusterID].TrickIndex += data[i].TrickIndex;
            }

            for (var k = 0; k < _centroids.Length; ++k)
            {
                _centroids[k].PointsInTrick /= clusterCounts[k];
                _centroids[k].HighestRankInTrick /= clusterCounts[k];
                _centroids[k].MorePowerfulUnknownCards /= clusterCounts[k];
                _centroids[k].RemainingUnknownPoints /= clusterCounts[k];
                _centroids[k].MorePowerfulHeld /= clusterCounts[k];
                _centroids[k].PointsHeld /= clusterCounts[k];
                _centroids[k].CardsHeldWithPoints /= clusterCounts[k];
                _centroids[k].MoveIndex /= clusterCounts[k];
                _centroids[k].TrickIndex /= clusterCounts[k];
            }
        }

        //This accepts list of centroid instead of MoveStateKeys because it must accept weighed data.
        //That is, the data must be double's and not int's.
        private bool UpdateClustering(List<MoveStatCentroid> data)
        {
            bool changed = false;

            int[] newClustering = new int[_clustering.Length];
            Array.Copy(_clustering, newClustering, _clustering.Length);

            double[] distance = new double[_numClusters];

            for (int i = 0; i < data.Count; ++i)
            {
                for (int k = 0; k < _numClusters; ++k)
                    distance[k] = ClusterUtils.Distance(data[i], _centroids[k]);

                int newClusterID = MinIndex(distance);
                if (newClusterID != newClustering[i])
                {
                    changed = true;
                    newClustering[i] = newClusterID;
                }
            }

            if (changed == false)
                return false;

            int[] clusterCounts = new int[_numClusters];
            for (int i = 0; i < data.Count; ++i)
            {
                int clusterID = newClustering[i];
                ++clusterCounts[clusterID];
            }

            for (int k = 0; k < _numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            Array.Copy(newClustering, _clustering, newClustering.Length);
            return true;
        }

        private static int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 1; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }
    }
}