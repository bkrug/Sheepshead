using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Models
{
    public class User : LongId, IUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public interface IUser : ILongId
    {
        string Name { get; set; }
        string Password { get; set; }
        string Email { get; set; }
    }
}