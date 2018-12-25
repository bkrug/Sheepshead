using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Participant
    {
        public Participant()
        {
            Coin = new HashSet<Coin>();
            HandPartnerParticipant = new HashSet<Hand>();
            HandPickerParticipant = new HashSet<Hand>();
            HandStartingParticipant = new HashSet<Hand>();
            ParticipantRefusingPick = new HashSet<ParticipantRefusingPick>();
            Point = new HashSet<Point>();
            Trick = new HashSet<Trick>();
            TrickPlay = new HashSet<TrickPlay>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Cards { get; set; }
        public string Type { get; set; }
        public Guid GameId { get; set; }
        public bool AssignedToClient { get; set; }
        public Guid Guid { get; set; }

        public Game Game { get; set; }
        public ICollection<Coin> Coin { get; set; }
        public ICollection<Hand> HandPartnerParticipant { get; set; }
        public ICollection<Hand> HandPickerParticipant { get; set; }
        public ICollection<Hand> HandStartingParticipant { get; set; }
        public ICollection<ParticipantRefusingPick> ParticipantRefusingPick { get; set; }
        public ICollection<Point> Point { get; set; }
        public ICollection<Trick> Trick { get; set; }
        public ICollection<TrickPlay> TrickPlay { get; set; }
    }
}
