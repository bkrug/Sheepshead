using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players
{
/*
Heuristic For Whether to play the card = 
	GamesWon% + (TricksWon% / (2 ^ (abs(GamesWon% - 50) / 25) * 2) - 25
*/

    public class LearningPlayer : ComputerPlayer
    {
        public override ICard GetMove(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public override bool WillPick(IDeck deck)
        {
            throw new NotImplementedException();
        }

        protected override List<ICard> DropCardsForPickInternal(IDeck deck)
        {
            throw new NotImplementedException();
        }
    }
}