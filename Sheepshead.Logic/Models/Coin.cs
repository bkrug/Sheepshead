using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Coin
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int ParticipantId { get; set; }
        public int HandId { get; set; }

        public Hand Hand { get; set; }
        public Participant Participant { get; set; }
    }
}
