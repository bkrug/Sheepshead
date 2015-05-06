using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public class Clusterer
    {
        private int _numClusters;
        private Random _rnd;

        public Clusterer(int numClusters, Random rnd)
        {
            _numClusters = numClusters;
            _rnd = rnd;
        }

        public ClusterResult Cluster(List<MoveStatUniqueKey> data)
        {
            var noPartnerData = data.Where(m => m.Partner == null).ToList();
            var partnerData = data.Where(m => m.Partner != null).ToList();
            int noPartnerClusters;
            int partnerClusters;
            SetClusterCounts(data, noPartnerData, out noPartnerClusters, out partnerClusters);
            var noPartnerClusterer = new InnerClusterer(noPartnerClusters, _rnd);
            var partnerClusterer = new InnerClusterer(partnerClusters, _rnd);
            var noPartnerResults = noPartnerClusterer.Cluster(noPartnerData);
            var partnerResults = partnerClusterer.Cluster(partnerData);
            var combinedResults = new ClusterResult();
            combinedResults.Data = noPartnerResults.Data.Union(partnerResults.Data).ToList();
            combinedResults.ClusterIndicies = new int[data.Count()];
            CopyClusterIndicies(ref noPartnerResults, ref partnerResults, ref combinedResults);
            combinedResults.Centroids = noPartnerResults.Centroids.Union(partnerResults.Centroids).ToList();
            return combinedResults;
        }

        private void SetClusterCounts(List<MoveStatUniqueKey> data, List<MoveStatUniqueKey> noPartner, out int noPartnerClusters, out int partnerClusters)
        {
            if (!data.Any())
                noPartnerClusters = 0;
            else
                noPartnerClusters = (int)Math.Round(((decimal)noPartner.Count() / data.Count() * _numClusters), 0);
            partnerClusters = _numClusters - noPartnerClusters;
            if (noPartnerClusters < 1 && noPartner.Any())
            {
                noPartnerClusters = 1;
                partnerClusters = data.Count() - 1;
            }
            if (partnerClusters < 1 && data.Any())
            {
                noPartnerClusters = data.Count() - 1;
                partnerClusters = 1;
            }
        }

        private static void CopyClusterIndicies(ref ClusterResult noPartnerResults, ref ClusterResult partnerResults, ref ClusterResult combinedResults)
        {
            Array.Copy(noPartnerResults.ClusterIndicies, combinedResults.ClusterIndicies, noPartnerResults.ClusterIndicies.Length);
            //The cluster indicies in the original results refer to each centroid's original index.
            //by combining the centroid results, we change the centroid's index.  That must be represented by increasing the index values. 
            var arrayOffset = noPartnerResults.ClusterIndicies.Length;
            var clusterOffset = noPartnerResults.Centroids.Count();
            for (var i = 0; i < partnerResults.ClusterIndicies.Length; ++i)
                combinedResults.ClusterIndicies[arrayOffset + i] = partnerResults.ClusterIndicies[i] + clusterOffset;
        }
    }
}