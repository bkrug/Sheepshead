using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Player : IPlayer
    {
        public string Name { get { return String.Empty; } }
    }

    public interface IPlayer
    {
        string Name { get; }
    }
}