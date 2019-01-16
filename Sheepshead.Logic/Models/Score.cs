using System;
using System.Collections.Generic;
using System.Text;

namespace Sheepshead.Logic.Models
{
    public class Score
    {
        public int Coins { get; set; }
        public int Points { get; set; }
        public int HandId { get; set; }
        public int ParticipantId { get; set; }

        public Hand Hand { get; set; }
        public Participant Participant { get; set; }
    }
}
