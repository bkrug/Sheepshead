using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Models
{
    public class BaseRepository<T>
        : IBaseRepository<T> where T : ILongId
    {
        private int m_currentIdValue = 0;

        private BaseRepository() { }

        public BaseRepository(Dictionary<long, T> datasource)
        {
            Items = datasource;
        }

        protected Dictionary<long, T> Items { get; set; }

        public void Save(T saveThis)
        {
            if (saveThis == null)
            {
                throw new ArgumentNullException(
                     "saveThis", "Argument cannot be null.");
            }

            if (saveThis.Id == 0)
            {
                saveThis.Id = ++m_currentIdValue;
            }

            if (Items.Keys.Contains(saveThis.Id) == false)
            {
                Items.Add(saveThis.Id, saveThis);
            }
        }

        public T GetById(long id)
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
        void Save(T saveThis);
        T GetById(long id);
        IList<T> GetAll();
        void Delete(T saveThis);
    }
}