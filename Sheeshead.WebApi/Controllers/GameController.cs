using System;
using System.Web.Http;
using System.Linq;
using Sheepshead.Model;
using Sheepshead.Models;
using Sheepshead.Models.Players.Stats;
using Sheepshead.Models.Wrappers;

namespace Sheeshead.WebApi.Controllers
{
    public class GameController : ApiController
    {
        private static IRandomWrapper _rnd = new RandomWrapper();

        //[HttpPost]
        //public string RequestPlayerKey(string oldKey)
        //{
        //    if (!string.IsNullOrEmpty(oldKey) && PlayerDictionary.PlayerFound(new Guid(oldKey)))
        //        return oldKey;
        //    return PlayerDictionary.GeneratePlayerKey();
        //}

        ////After adding this method, javascript calls to game/validatePlayerKey would
        ////route to game/create.  I could have continued to learn more about WebApi to 
        ////figure this out, but the state of this program is already such that we 
        ////need two website running, which is weird.
        //[HttpPost]
        //public bool ValidatePlayerKey(int someNumber)
        //{
        //    var key = Request.Headers.GetValues("Player-Key").FirstOrDefault();
        //    return PlayerDictionary.PlayerFound(new Guid(key));
        //}

        [HttpPost]
        public IHttpActionResult Create(GameStartModel model)
        {
            var repository = new GameRepository(GameDictionary.Instance.Dictionary);
            var newGame = repository.CreateGame(model, _rnd, new LearningHelperFactory());
            repository.Save(newGame);
            newGame.RearrangePlayers();
            return Json(new { success = true });
        }
    }
}
