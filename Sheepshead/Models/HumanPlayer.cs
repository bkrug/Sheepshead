using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class HumanPlayer : Player, IHumanPlayer
    {
    }

    public interface IHumanPlayer
    {
    }
}