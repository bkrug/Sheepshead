using Microsoft.EntityFrameworkCore;
using Sheepshead.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Logic.DAL
{
    public class ScoreRepository : IDisposable
    {
        private SheepsheadContext context;

        public ScoreRepository(SheepsheadContext context)
        {
            this.context = context;
        }

        public List<Score> GetScoresByHand(int handId)
        {
            return context.Score
                .Include(s => s.Participant)
                .Include(s => s.Hand)
                .Where(s => s.HandId == handId)
                .ToList();
        }

        public Score GetScoreById(int handId, int participantId)
        {
            return context.Score
                .Include(s => s.Participant)
                .Include(s => s.Hand)
                .SingleOrDefault(s => s.HandId == handId && s.ParticipantId == participantId);
        }

        public void Add(Score score)
        {
            context.Score.Add(score);
        }

        public void DeleteScore(int handId, int participantId)
        {
            var score = context.Score.Find(handId, participantId);
            context.Score.Remove(score);
        }

        public void UpdateScore(Score score)
        {
            context.Entry(score).State = EntityState.Modified;
        }

        public void RecordScores(Hand hand)
        {
            var scoresInDb = GetScoresByHand(hand.Id);
            if (!scoresInDb.Any() && hand.IsComplete())
            {
                var handScores = ScoreCalculator.GetScores(hand);
                hand.IGame.Players.ForEach(player =>
                {
                    var score = new Score()
                    {
                        Hand = hand,
                        Participant = player.Participant,
                        Coins = handScores.Coins[player],
                        Points = handScores.Points.ContainsKey(player) ? handScores.Points[player] : 0
                    };
                    hand.ScoreList.Add(score);
                    Add(score);
                });
                Save();
            }
        }

        public List<GameCoins> GameCoins(Guid gameId)
        {
            var gameCoins = context
                .Score
                .Where(s => s.Hand.GameId == gameId)
                .GroupBy(s => s.Participant)
                .OrderBy(g => g.Key.SortOrder)
                .Select(g => new GameCoins
                {
                    Name = g.Key.Name,
                    Coins = g.Sum(c => c.Coins)
                })
                .ToList();
            if (gameCoins.Any())
                return gameCoins;

            var emptyScores = context
                .Participant
                .Where(p => p.GameId == gameId)
                .OrderBy(p => p.SortOrder)
                .Select(p => new GameCoins()
                {
                    Name = p.Name,
                    Coins = 0
                })
                .ToList();
            return emptyScores;
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
