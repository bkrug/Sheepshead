using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models.Players
{
    public class HumanPlayer : Player, IHumanPlayer
    {
        private IUser _user;

        private HumanPlayer() { }
        public HumanPlayer(IUser user) {
            _user = user;
        }

        public override string Name
        {
            get { return _user.Name; }
        }
    }

    public interface IHumanPlayer : IPlayer
    {
    }
}