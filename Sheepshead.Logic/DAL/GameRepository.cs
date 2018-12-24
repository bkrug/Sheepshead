using Microsoft.EntityFrameworkCore;
using Sheepshead.Logic.Models;
using System;
using System.Collections.Generic;

namespace Sheepshead.Model.DAL
{
    public class GameRepository : IDisposable
    {
        private SheepsheadContext context;

        public GameRepository(SheepsheadContext context)
        {
            this.context = context;
        }

        public IEnumerable<Game> GetGames()
        {
            return context.Game;
        }

        public Game GetGameById(Guid id)
        {
            return context.Game.Find(id);
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
            newGame.RearrangePlayers();
            context.Game.Add(newGame);
            return newGame;
        }

        //public void InsertGame(Game game)
        //{
        //    context.Games.Add(game);
        //}

        public void DeleteGame(int gameId)
        {
            var game = context.Game.Find(gameId);
            context.Game.Remove(game);
        }

        public void UpdateGame(Game game)
        {
            context.Entry(game).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
