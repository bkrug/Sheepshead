using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class HumanPlayer : Player, IHumanPlayer
    {
        private IUser _user;

        private HumanPlayer() { }
        public HumanPlayer(IUser user) {
            _user = user;
        }

        public string Name
        {
            get { return _user.Name; }
        }
    }

    public interface IHumanPlayer : IPlayer
    {
    }
}