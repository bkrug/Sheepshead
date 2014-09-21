using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public abstract class ComputerPlayer : Player
    {
        public abstract ICard GetMove(ITrick trick);

        public abstract bool WillPick(ITrick trick);
    }
}