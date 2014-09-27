using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public IGame CreateGame(string name, List<IPlayer> players)
        {
            var game = _repository.CreateGame(name, players);
            return game;
        }

        public IGame GetGame(long id)
        {
            return _repository.GetById(id);
        }
    }

    public interface IGameService
    {
        IGame CreateGame(string name, List<IPlayer> players);
        IGame GetGame(long id);
    }
}