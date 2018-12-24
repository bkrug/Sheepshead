using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Game
    {
        public Game()
        {
            Hand = new HashSet<Hand>();
            Participant = new HashSet<Participant>();
        }

        public Guid Id { get; set; }
        public bool LeastersEnabled { get; set; }
        public string PartnerMethod { get; set; }

        public ICollection<Hand> Hand { get; set; }
        public ICollection<Participant> Participant { get; set; }
    }
}
