using System;
using System.Collections.Generic;
using System.Linq;

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