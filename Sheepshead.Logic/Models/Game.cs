using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Game
    {
        public Game()
        {
            Hands = new HashSet<Hand>();
            Participants = new HashSet<Participant>();
            CreationTime = CreationTime ?? DateTime.Now;
        }

        public Guid Id { get; set; }
        public bool LeastersEnabled { get; set; }
        public string PartnerMethod { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }

        public ICollection<Hand> Hands { get; set; }
        public ICollection<Participant> Participants { get; set; }
    }
}
