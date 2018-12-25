using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Trick
    {
        public Trick()
        {
            TrickPlay = new HashSet<TrickPlay>();
        }

        public int Id { get; set; }
        public int HandId { get; set; }
        public int StartingParticipantId { get; set; }

        public Hand Hand { get; set; }
        public Participant Participant { get; set; }
        public ICollection<TrickPlay> TrickPlay { get; set; }
    }
}
