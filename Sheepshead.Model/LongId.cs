using System;
using System.Collections.Generic;
using System.Linq;


namespace Sheepshead.Models
{
    public class LongId : ILongId
    {
        protected long _id;

        public long Id
        {
            get { return _id; }
            set
            {
                if (_id == 0)
                    _id = value;
                else
                    throw new IdAlreadySetException("Id has already been set.");
            }
        }
    }

    public class IdAlreadySetException: ApplicationException {
        public IdAlreadySetException(string message) : base (message)
        {
        }
    }

    public interface ILongId
    {
        long Id { get; set; }
    }
}