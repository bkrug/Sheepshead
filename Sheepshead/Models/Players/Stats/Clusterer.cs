using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Models.Players.Stats
{
    public class Clusterer
    {
        private int _numClusters;
        private IRandomWrapper _rnd;

        public Clusterer(int numClusters, IRandomWrapper rnd)
        {
            _numClusters = numClusters;
            _rnd = rnd;
        }

        public ClusterResult Cluster(List<MoveStatUniqueKey> data)
        {
            var dataInRooms = data.GroupBy(m => m.CentroidRoom).ToList();
            var clusterCounts = new List<int>();
            SetClusterCounts(dataInRooms, out clusterCounts);
            var clusterers = clusterCounts.Select(c => new InnerClusterer(c, _rnd)).ToList();
            var results = new List<ClusterResult>();
            for (var i = 0; i < dataInRooms.Count(); ++i)
                results.Add(clusterers[i].Cluster(dataInRooms[i].ToList()));
            var combinedResults = new ClusterResult();
            combinedResults.Data = new List<MoveStatUniqueKey>();
            combinedResults.ClusterIndicies = new int[data.Count()];
            foreach (var result in results)
            {
                combinedResults.Data.AddRange(result.Data);
                Array.Copy(result.ClusterIndicies, combinedResults.ClusterIndicies, result.ClusterIndicies.Count());
                combinedResults.Centroids.AddRange(result.Centroids);
            }
            return combinedResults;
        }

        private void SetClusterCounts(List<IGrouping<int, MoveStatUniqueKey>> dataInRooms, out List<int> clusterCounts)
        {
            clusterCounts = new List<int>();
            foreach (var room in dataInRooms.OrderBy(d => d.Key))
            {
                var count = (int)Math.Round(Math.Sqrt(room.Count()));
                clusterCounts.Add(count);
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