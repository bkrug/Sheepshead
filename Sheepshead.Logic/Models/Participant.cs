﻿using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Models
{
    public partial class Participant
    {
        public Participant()
        {
            HandsAsPartner = new HashSet<Hand>();
            HandsAsPicker = new HashSet<Hand>();
            HandsStarted = new HashSet<Hand>();
            PicksRefused = new HashSet<ParticipantRefusingPick>();
            Tricks = new HashSet<Trick>();
            TrickPlays = new HashSet<TrickPlay>();
            Scores = new HashSet<Score>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Cards { get; set; }
        public string Type { get; set; }
        public Guid GameId { get; set; }
        public bool AssignedToClient { get; set; }
        public Guid Guid { get; set; }
        public int SortOrder { get; set; }

        public Game Game { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<Hand> HandsAsPartner { get; set; }
        public ICollection<Hand> HandsAsPicker { get; set; }
        public ICollection<Hand> HandsStarted { get; set; }
        public ICollection<ParticipantRefusingPick> PicksRefused { get; set; }
        public ICollection<Trick> Tricks { get; set; }
        public ICollection<TrickPlay> TrickPlays { get; set; }
    }
}
