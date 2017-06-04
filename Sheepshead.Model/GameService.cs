using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Models
{
    public class GameService : IGameService
    {
        private IGameRepository _repository;

        private GameService() { }

        public GameService(IGameRepository repository)
        {
            _repository = repository;
        }

        public IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd, ILearningHelperFactory factory)
        {
            var game = _repository.CreateGame(name, players, rnd, factory);
            return game;
        }

        public IGame GetGame(long id)
        {
            return _repository.GetById(id);
        }
    }

    public interface IGameService
    {
        IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd, ILearningHelperFactory factory);
        IGame GetGame(long id);
    }
}