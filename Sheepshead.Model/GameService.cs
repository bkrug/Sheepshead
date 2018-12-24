using System;
using Sheepshead.Model.Models;

namespace Sheepshead.Model
{
    public class GameService : IGameService
    {
        private IOldGameRepository _repository;

        private GameService() { }

        public GameService(IOldGameRepository repository)
        {
            _repository = repository;
        }

        public IGame GetGame(Guid id)
        {
            return _repository.GetById(id);
        }
    }

    public interface IGameService
    {
        IGame GetGame(Guid id);
    }
}