using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sheepshead.Models;

namespace Sheepshead.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create (string username, string password, string email)
        {
            var user = new User() { Name = username, Password = password, Email = email };
            var repository = new UserRepository(UserDictionary.Instance.Dictionary);
            repository.Save(user);
            return RedirectToAction("Index", "Game");
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var repository = new UserRepository(UserDictionary.Instance.Dictionary);
            var user = repository.GetById(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(long id, string password, string email)
        {
            var repository = new UserRepository(UserDictionary.Instance.Dictionary);
            var user = repository.GetById(id);
            user.Password = password;
            user.Email = email;
            repository.Save(user);
            return RedirectToAction("Index", "Game");
        }
    }
}
