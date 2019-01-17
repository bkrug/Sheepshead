using Microsoft.EntityFrameworkCore;
using Sheepshead.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Logic.DAL
{
    public class GameRepository : IDisposable
    {
        private SheepsheadContext context;

        public GameRepository(SheepsheadContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get game instance including the most recent two hands.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Game GetGameById(Guid id)
        {
            var game = context.Game
                .Include(g => g.Participants)
                .SingleOrDefault(g => g.Id == id);

            var hands = context.Hand
                .Include(h => h.StartingParticipant)
                .Include(h => h.ParticipantsRefusingPick)
                .Include(h => h.PickerParticipant)
                .Include(h => h.PartnerParticipant)
                .Include(h => h.Tricks)
                    .ThenInclude(t => t.TrickPlays)
                .Include(h => h.Tricks)
                    .ThenInclude(t => t.StartingParticipant)
                .Where(h => h.GameId == id)
                .OrderByDescending(h => h.Id)
                .Take(2)
                .OrderBy(h => h.Id)
                .ToList();

            hands.ForEach(h => h.Game = game);
            game.Hands = hands;
            return game;
        }

        public IGame Create(int humanCount, int simpleCount, int intermediateCount, int advancedCount, PartnerMethod partnerMethod, bool leastersGame)
        {
            var participants = new List<Participant>();
            for (var i = 0; i < humanCount; ++i)
                participants.Add(new Participant() { Type = Participant.TYPE_HUMAN });
            for (var i = 0; i < simpleCount; ++i)
                participants.Add(new Participant() { Type = Participant.TYPE_SIMPLE });
            for (var i = 0; i < intermediateCount; ++i)
                participants.Add(new Participant() { Type = Participant.TYPE_INTERMEDIATE });
            for (var i = 0; i < advancedCount; ++i)
                participants.Add(new Participant() { Type = Participant.TYPE_ADVANCED });
            var newGame = new Game(participants, partnerMethod, leastersGame);
            newGame.RearrangePlayers();
            context.Game.Add(newGame);
            return newGame;
        }

        public void DeleteGame(int gameId)
        {
            var game = context.Game.Find(gameId);
            context.Game.Remove(game);
        }

        public void UpdateGame(Game game)
        {
            game.LastModifiedTime = DateTime.Now;
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
