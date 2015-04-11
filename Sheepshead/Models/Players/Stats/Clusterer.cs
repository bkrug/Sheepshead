using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public interface IClusterer
    {
        IReadOnlyList<MoveStatCentroid> Centroids { get; }
    }

    public class Clusterer : IClusterer
    {
        private int _numClusters;
        private MoveStatCentroid[] _results;
        private Random _rnd;

        public IReadOnlyList<MoveStatCentroid> Centroids
        {
            get
            {
                return _results;
            }
        }

        public Clusterer(int numClusters, Random rnd)
        {
            _numClusters = numClusters;
            _rnd = rnd;
        }

        public void Cluster(List<MoveStatUniqueKey> data)
        {
            var noPartnerData = data.Where(m => m.Partner == null).ToList();
            var partnerData = data.Where(m => m.Partner != null).ToList();
            int noPartnerClusters;
            int partnerClusters;
            SetClusterCounts(data, noPartnerData, out noPartnerClusters, out partnerClusters);
            var noPartnerClusterer = new InnerClusterer(noPartnerClusters, _rnd);
            var partnerClusterer = new InnerClusterer(partnerClusters, _rnd);
            noPartnerClusterer.Cluster(noPartnerData);
            partnerClusterer.Cluster(partnerData);
            _results = noPartnerClusterer.Centroids.Union(partnerClusterer.Centroids).ToArray();
        }

        private void SetClusterCounts(List<MoveStatUniqueKey> data, List<MoveStatUniqueKey> noPartner, out int noPartnerClusters, out int partnerClusters)
        {
            noPartnerClusters = (int)Math.Round((decimal)(noPartner.Count() / data.Count() * _numClusters), 0);
            partnerClusters = data.Count() - noPartnerClusters;
            if (noPartnerClusters < 1)
            {
                noPartnerClusters = 1;
                partnerClusters = data.Count() - 1;
            }
            if (partnerClusters < 1)
            {
                noPartnerClusters = data.Count() - 1;
                partnerClusters = 1;
            }
        }
    }
}