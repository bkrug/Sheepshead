﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Hand : IHand
    {
        public IDeck Deck { get; private set; }

        public Hand(IDeck deck)
        {
            Deck = deck;
        }
    }

    public interface IHand
    {
        IDeck Deck { get; }
    }
}