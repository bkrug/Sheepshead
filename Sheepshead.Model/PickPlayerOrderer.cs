using Sheepshead.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Model
{
    public interface IPlayerOrderer
    {
        List<IPlayer> PlayersInTurnOrder(List<IPlayer> players, IPlayer startingPlayer);
        List<IPlayer> PlayersWithoutTurn(List<IPlayer> playersInPickOrder, List<IPlayer> playersRefusingPick);
    }

    public class PlayerOrderer : IPlayerOrderer
    {
        public List<IPlayer> PlayersInTurnOrder(List<IPlayer> players, IPlayer startingPlayer)
        {
            var startIndex = players.IndexOf(startingPlayer);
            return players.Skip(startIndex).Union(players.Take(startIndex)).ToList();
        }

        public List<IPlayer> PlayersWithoutTurn(List<IPlayer> playersInPickOrder, List<IPlayer> playersRefusingPick)
        {
            var finishedCount = playersRefusingPick.Count();
            return playersInPickOrder.Skip(finishedCount).ToList();
        }
    }
}