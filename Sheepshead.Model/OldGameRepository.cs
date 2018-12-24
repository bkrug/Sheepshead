using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Model.Models;
using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    public class GameDictionary
    {
        private GameDictionary() { }
        public static GameDictionary Instance { get; } = new GameDictionary();
        public IDictionary<Guid, IGame> Dictionary { get; } = new SelfClearingDictionary();
    }

    public class OldGameRepository : BaseRepository<IGame>, IOldGameRepository
    {
        public OldGameRepository(IDictionary<Guid, IGame> gameList) : base(gameList)
        {
        }

        public IGame Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, PartnerMethod partnerMethod, bool leastersGame)
        {
            var participant = new List<Participant>();
            for (var i = 0; i < humanCount; ++i)
                participant.Add(new Participant() { Type = "H" });
            for (var i = 0; i < simpleCount; ++i)
                participant.Add(new Participant() { Type = "S" });
            for (var i = 0; i < intermediateCount; ++i)
                participant.Add(new Participant() { Type = "M" });
            for (var i = 0; i < advancedCount; ++i)
                participant.Add(new Participant() { Type = "I" });
            var newGame = new Game(participant, partnerMethod, leastersGame);
            Save(newGame);
            newGame.RearrangePlayers();
            return newGame;
        }

        public IGame GetGame(Func<IGame, bool> lambda)
        {
            return Items.Select(l => l.Value).ToList().FirstOrDefault(lambda);
        }
    }

    public interface IOldGameRepository : IBaseRepository<IGame>
    {
        IGame Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, PartnerMethod partnerMethod, bool leastersGame);
        IGame GetGame(Func<IGame, bool> lambda);
    }
}