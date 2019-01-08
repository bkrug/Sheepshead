using Sheepshead.Logic.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Logic
{
    public class PlayerOrderer
    {
        public static List<IPlayer> PlayersInTurnOrder(List<IPlayer> players, IPlayer startingPlayer)
        {
            var startIndex = players.IndexOf(startingPlayer);
            return players.Skip(startIndex).Union(players.Take(startIndex)).ToList();
        }

        public static List<IPlayer> PlayersWithoutTurn(List<IPlayer> playersInPickOrder, IReadOnlyList<IPlayer> playersRefusingPick)
        {
            var finishedCount = playersRefusingPick.Count();
            return playersInPickOrder.Skip(finishedCount).ToList();
        }
    }
}