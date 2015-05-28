using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Wrappers;

namespace Sheepshead.Models.Players.Stats
{
    public class Clusterer
    {
        private IRandomWrapper _rnd;

        public Clusterer(IRandomWrapper rnd)
        {
            _rnd = rnd;
        }

        public Dictionary<int, ClusterResult> Cluster(List<MoveStatUniqueKey2> data)
        {
            var dataInRooms = data.GroupBy(m => m.CentroidRoom).ToList();
            var clusterCounts = new List<int>();
            SetClusterCounts(dataInRooms, out clusterCounts);
            var clusterers = clusterCounts.Select(c => new InnerClusterer(c, _rnd)).ToList();
            var results = new Dictionary<int, ClusterResult>();
            for (var i = 0; i < dataInRooms.Count(); ++i)
                results.Add(dataInRooms[i].First().CentroidRoom, clusterers[i].Cluster(dataInRooms[i].ToList()));
            return results;
        }

        private void SetClusterCounts(List<IGrouping<int, MoveStatUniqueKey2>> dataInRooms, out List<int> clusterCounts)
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