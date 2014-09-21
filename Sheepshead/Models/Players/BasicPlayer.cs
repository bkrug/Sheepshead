using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class BasicPlayer : ComputerPlayer
    {
        public override ICard GetMove(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public override bool WillPick(ITrick trick)
        {
            throw new NotImplementedException();
        }
    }
}