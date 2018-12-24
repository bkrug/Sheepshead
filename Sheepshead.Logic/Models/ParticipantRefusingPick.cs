using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class ParticipantRefusingPick
    {
        public int HandId { get; set; }
        public int ParticipantId { get; set; }

        public Hand Hand { get; set; }
        public Participant Participant { get; set; }
    }
}
