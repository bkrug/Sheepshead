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

        public IGame CreateGame(string name, int numPlayers, int humanPlayers)
        {
            var game = _repository.CreateGame(name);
            game.MaxPlayers = numPlayers;
            game.MaxHumanPlayers = humanPlayers;
            return game;
        }

        public IGame GetGame(long id)
        {
            return _repository.GetById(id);
        }
    }

    public interface IGameService
    {
        IGame CreateGame(string name, int numPlayers, int humanPlayers);
        IGame GetGame(long id);
    }
}