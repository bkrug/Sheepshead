using Sheepshead.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model
{
    public class BaseRepository<T>
        : IBaseRepository<T> where T : IGame
    {
        private BaseRepository() { }

        public BaseRepository(IDictionary<Guid, T> datasource)
        {
            Items = datasource;
        }

        protected IDictionary<Guid, T> Items { get; set; }

        protected void Save(T saveThis)
        {
            if (saveThis == null)
            {
                throw new ArgumentNullException(
                     "saveThis", "Argument cannot be null.");
            }

            Items.Add(saveThis.Id, saveThis);
        }

        public T GetById(Guid id)
        {
            if (Items.ContainsKey(id))
                return Items[id];
            return default(T);
        }

        public IList<T> GetAll()
        {
            return Items.Select(i => i.Value).ToList();
        }

        public void Delete(T saveThis)
        {
            Items.Remove(saveThis.Id);
        }
    }

    public interface IBaseRepository<T>
    {
        T GetById(Guid id);
        IList<T> GetAll();
        void Delete(T saveThis);
    }
}