using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;

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

        public IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd)
        {
            var game = _repository.CreateGame(name, players, rnd);
            return game;
        }

        public IGame GetGame(long id)
        {
            return _repository.GetById(id);
        }
    }

    public interface IGameService
    {
        IGame CreateGame(string name, List<IPlayer> players, IRandomWrapper rnd);
        IGame GetGame(long id);
    }
}