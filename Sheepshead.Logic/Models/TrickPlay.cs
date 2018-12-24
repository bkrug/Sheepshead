using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class TrickPlay
    {
        public int ParticipantId { get; set; }
        public int TrickId { get; set; }
        public string Card { get; set; }
        public int SortOrder { get; set; }

        public Participant Participant { get; set; }
        public Trick Trick { get; set; }
    }
}
