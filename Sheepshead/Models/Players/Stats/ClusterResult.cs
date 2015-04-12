using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public struct ClusterResult
    {
        public int[] ClusterIndicies;
        public List<MoveStatCentroid> Centroids;
        public List<MoveStatUniqueKey> Data;

        public MoveStatCentroid GetCentroid(MoveStatUniqueKey key)
        {
            var keyIndex = Data.IndexOf(key);
            if (keyIndex >= 0)
                return GetCentroid(keyIndex);
            throw new IndexOutOfRangeException("Could not find specified MoveStatUniqueKey");
        }

        public MoveStatCentroid GetCentroid(int keyIndex) {
            if (ClusterIndicies.Length <= keyIndex)
                throw new IndexOutOfRangeException("Could not find a ClusterIndex with that index.");
            if (Centroids.Count() <= ClusterIndicies[keyIndex])
                throw new IndexOutOfRangeException("Could not find a Centroid with the index " + keyIndex + ".");
            return Centroids[ClusterIndicies[keyIndex]];
        }

        public List<MoveStatUniqueKey> GetDataInCluster(MoveStatCentroid centroid)
        {
            var centroidIndex = Centroids.IndexOf(centroid);
            if (centroidIndex >= 0)
                return GetDataInCluster(centroidIndex);
            throw new IndexOutOfRangeException("Could not find specified Centroid");
        }

        public List<MoveStatUniqueKey> GetDataInCluster(int centroidIndex)
        {
            var keys = new List<MoveStatUniqueKey>();
            for (var i = 0; i < ClusterIndicies.Count(); ++i)
                if (ClusterIndicies[i] == centroidIndex)
                {
                    if (ClusterIndicies[i] >= Data.Count())
                        throw new IndexOutOfRangeException("Mismatch in the number of ClusterIndecies and Data");
                    else
                        keys.Add(Data[i]);
                }
            return keys;
        }
    }
}