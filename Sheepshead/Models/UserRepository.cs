using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace Sheepshead.Models
{
    public class UserDictionary
    {
        private static UserDictionary _instance = new UserDictionary();
        private Dictionary<long, IUser> _dictionary = new Dictionary<long, IUser>();

        private UserDictionary() { }

        public static UserDictionary Instance {
            get { return _instance; }
        }

        public Dictionary<long, IUser> Dictionary { get { return _dictionary; } }
    }

    public class UserRepository : BaseRepository<IUser>, IUserRepository
    {
        public UserRepository(Dictionary<long, IUser> gameList) : base(gameList)
        {
        }

        public IUser CreateUser(string name)
        {
            var user = new User() { Name = name };
            return user;
        }

        public IUser GetUser(Func<IUser, bool> lambda)
        {
            return Items.Select(l => l.Value).ToList().FirstOrDefault(lambda);
        }
    }

    public interface IUserRepository : IBaseRepository<IUser>
    {
        IUser CreateUser(string name);
        IUser GetUser(Func<IUser, bool> lambda);
    }
}