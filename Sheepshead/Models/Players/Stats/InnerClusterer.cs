using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class InnerClusterer
    {
        private int _numClusters;
        private int[] _clustering;
        private MoveStatCentroid[] _centroids;
        private Random _rnd;

        public InnerClusterer(int numClusters, Random rnd)
        {
            _numClusters = numClusters;
            _centroids = new MoveStatCentroid[numClusters];
            _rnd = rnd;
        }

        public ClusterResult Cluster(List<MoveStatUniqueKey> data)
        {
            var numTuples = data.Count();
            _clustering = new int[numTuples];

            for (int k = 0; k < _numClusters; ++k)
                _centroids[k] = new MoveStatCentroid();

            InitRandom(data);

            var changed = true;
            var maxCount = numTuples * 10;
            var ct = 0;
            while (changed && ct <= maxCount)
            {
                ++ct;
                UpdateCentroids(data);
                changed = UpdateClustering(data);
            }

            var clusterResult = new ClusterResult()
            {
                Data = data,
                ClusterIndicies = new int[_clustering.Length],
                Centroids = _centroids.ToList()
            };
            Array.Copy(_clustering, clusterResult.ClusterIndicies, _clustering.Length);
            return clusterResult;
        }

        private void InitRandom(List<MoveStatUniqueKey> data)
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

        //TODO: Either all input data elements should have a null value for Partner or none should
        private void UpdateCentroids(List<MoveStatUniqueKey> data)
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
            if (data[0].Partner != null)
                for (var k = 0; k < _centroids.Length; ++k)
                    _centroids[k].Partner = 0;

            for (var i = 0; i < data.Count; ++i)
            {
                int clusterID = _clustering[i];
                _centroids[clusterID].HigherRankingCardsPlayedPreviousTricks += data[i].HigherRankingCardsPlayedPreviousTricks;
                _centroids[clusterID].HigherRankingCardsPlayedThisTrick += data[i].HigherRankingCardsPlayedThisTrick;
                _centroids[clusterID].MoveWithinTrick += data[i].MoveWithinTrick;
                if (_centroids[clusterID].Partner != null)
                    _centroids[clusterID].Partner += data[i].Partner;
                _centroids[clusterID].PartnerCard += (data[i].PartnerCard ? 1 : 0);
                _centroids[clusterID].Picker += data[i].Picker;
                _centroids[clusterID].PointsAlreadyInTrick += data[i].PointsAlreadyInTrick;
                _centroids[clusterID].PointsInThisCard += data[i].PointsInThisCard;
                _centroids[clusterID].RankOfThisCard += data[i].RankOfThisCard;
                _centroids[clusterID].TotalPointsInPreviousTricks += data[i].TotalPointsInPreviousTricks;
                _centroids[clusterID].Trick += data[i].Trick;
            }

            for (var k = 0; k < _centroids.Length; ++k)
            {
                _centroids[k].HigherRankingCardsPlayedPreviousTricks /= clusterCounts[k];
                _centroids[k].HigherRankingCardsPlayedThisTrick /= clusterCounts[k];
                _centroids[k].MoveWithinTrick /= clusterCounts[k];
                if (_centroids[k].Partner != null)
                    _centroids[k].Partner /= clusterCounts[k];
                _centroids[k].PartnerCard /= clusterCounts[k];
                _centroids[k].Picker /= clusterCounts[k];
                _centroids[k].PointsAlreadyInTrick /= clusterCounts[k];
                _centroids[k].PointsInThisCard /= clusterCounts[k];
                _centroids[k].RankOfThisCard /= clusterCounts[k];
                _centroids[k].TotalPointsInPreviousTricks /= clusterCounts[k];
                _centroids[k].Trick /= clusterCounts[k];
            }
        }

        private bool UpdateClustering(List<MoveStatUniqueKey> data)
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