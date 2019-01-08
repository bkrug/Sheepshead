using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Hand
    {
        public Hand()
        {
            Coins = new HashSet<Coin>();
            ParticipantsRefusingPick = new HashSet<ParticipantRefusingPick>();
            Points = new HashSet<Point>();
            Tricks = new HashSet<Trick>();
        }

        public int Id { get; set; }
        public string BlindCards { get; set; }
        public string BuriedCards { get; set; }
        public string PartnerCard { get; set; }
        public Guid GameId { get; set; }
        public int? PartnerParticipantId { get; set; }
        public int? PickerParticipantId { get; set; }
        public int StartingParticipantId { get; set; }
        public bool PickPhaseComplete { get; private set; }
        public int SortOrder { get; set; }

        public Game Game { get; set; }
        public Participant PartnerParticipant { get; set; }
        public Participant PickerParticipant { get; set; }
        public Participant StartingParticipant { get; set; }
        public ICollection<Coin> Coins { get; set; }
        public ICollection<ParticipantRefusingPick> ParticipantsRefusingPick { get; set; }
        public ICollection<Point> Points { get; set; }
        public ICollection<Trick> Tricks { get; set; }
    }
}
