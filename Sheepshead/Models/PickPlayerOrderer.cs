using Sheepshead.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public interface IPickPlayerOrderer
    {
        List<IPlayer> PlayersInPickOrder(List<IPlayer> players, IPlayer startingPlayer);
        List<IPlayer> PlayersWithoutPickTurn(List<IPlayer> playersInPickOrder, List<IPlayer> playersRefusingPick);
    }

    public class PickPlayerOrderer : IPickPlayerOrderer
    {
        public List<IPlayer> PlayersInPickOrder(List<IPlayer> players, IPlayer startingPlayer)
        {
            var startIndex = players.IndexOf(startingPlayer);
            return players.Skip(startIndex).Union(players.Take(startIndex)).ToList();
        }

        public List<IPlayer> PlayersWithoutPickTurn(List<IPlayer> playersInPickOrder, List<IPlayer> playersRefusingPick)
        {
            var finishedCount = playersRefusingPick.Count();
            return playersInPickOrder.Skip(finishedCount).ToList();
        }
    }
}