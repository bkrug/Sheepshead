using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model.Models
{
    //public class EntityBoundList<TI, TC> : CollectionBase, IList<TI> where TC : TI
    //{
    //    private ICollection<TC> _mainCollection;

    //    public EntityBoundList(ICollection<TC> mainCollection) : base(mainCollection.Count)
    //    {
    //        foreach (var item in mainCollection)
    //            List.Add(item);
    //        _mainCollection = mainCollection;
    //    }

    //    public TI this[int index] {
    //        get => (TI)List[index];
    //        set { throw new NotImplementedException(); }
    //    }

    //    //public int Count => _mainCollection.Count;

    //    public bool IsReadOnly => _mainCollection.IsReadOnly;

    //    public void Add(TI item)
    //    {
    //        List.Add((TC)item);
    //    }

    //    //public void Clear()
    //    //{
    //    //    _mainCollection.Clear();
    //    //}

    //    public bool Contains(TI item)
    //    {
    //        return _mainCollection.Contains((TC)item);
    //    }

    //    public void CopyTo(TI[] array, int arrayIndex)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    //public new IEnumerator<TI> GetEnumerator()
    //    //{
    //    //    return new Enumerator<TI>(List);
    //    //}

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    public int IndexOf(TI item)
    //    {
    //        return List.IndexOf((TC)item);
    //    }

    //    public void Insert(int index, TI item)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Remove(TI item)
    //    {
    //        List.Remove((TC)item);
    //        return true;
    //    }

    //    //public void RemoveAt(int index)
    //    //{
    //    //    _mainCollection.Remove(_mainCollection.ElementAt(index));
    //    //}

    //    protected override void OnInsert(int index, Object value)
    //    {
    //        _mainCollection.Add((TC)value);
    //    }

    //    protected override void OnRemove(int index, Object value)
    //    {
    //        _mainCollection.Remove((TC)value);
    //    }

    //    protected override void OnSet(int index, Object oldValue, Object newValue)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override void OnValidate(Object value)
    //    {
    //        if (value.GetType() != typeof(System.Int16))
    //            throw new ArgumentException("value must be of type Int16.", "value");
    //    }
    //}

    //public class EntityEnumerator<TI, TC> : IEnumerator<TI> where TC : TI
    //{
    //    private IList<TC> _mainList;

    //    int position = -1;

    //    public EntityEnumerator(ICollection<TC> mainList)
    //    {
    //        _mainList = (IList<TC>)mainList.ToList();
    //    }

    //    object IEnumerator.Current => Current;

    //    public TI Current
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return _mainList[position];
    //            }
    //            catch (IndexOutOfRangeException)
    //            {
    //                throw new InvalidOperationException();
    //            }
    //        }
    //    }

    //    public void Dispose()
    //    {
    //    }

    //    public bool MoveNext()
    //    {
    //        position++;
    //        return (position < _mainList.Count);
    //    }

    //    public void Reset()
    //    {
    //        position = -1;
    //    }
    //}

    ////
    //// Summary:
    ////     Represents a collection of objects that can be individually accessed by index.
    ////
    //// Type parameters:
    ////   T:
    ////     The type of elements in the list.
    //public interface IList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    //{
    //    //
    //    // Summary:
    //    //     Gets or sets the element at the specified index.
    //    //
    //    // Parameters:
    //    //   index:
    //    //     The zero-based index of the element to get or set.
    //    //
    //    // Returns:
    //    //     The element at the specified index.
    //    //
    //    // Exceptions:
    //    //   T:System.ArgumentOutOfRangeException:
    //    //     index is not a valid index in the System.Collections.Generic.IList`1.
    //    //
    //    //   T:System.NotSupportedException:
    //    //     The property is set and the System.Collections.Generic.IList`1 is read-only.
    //    T this[int index] { get; set; }

    //    //
    //    // Summary:
    //    //     Determines the index of a specific item in the System.Collections.Generic.IList`1.
    //    //
    //    // Parameters:
    //    //   item:
    //    //     The object to locate in the System.Collections.Generic.IList`1.
    //    //
    //    // Returns:
    //    //     The index of item if found in the list; otherwise, -1.
    //    int IndexOf(T item);
    //    //
    //    // Summary:
    //    //     Inserts an item to the System.Collections.Generic.IList`1 at the specified index.
    //    //
    //    // Parameters:
    //    //   index:
    //    //     The zero-based index at which item should be inserted.
    //    //
    //    //   item:
    //    //     The object to insert into the System.Collections.Generic.IList`1.
    //    //
    //    // Exceptions:
    //    //   T:System.ArgumentOutOfRangeException:
    //    //     index is not a valid index in the System.Collections.Generic.IList`1.
    //    //
    //    //   T:System.NotSupportedException:
    //    //     The System.Collections.Generic.IList`1 is read-only.
    //    void Insert(int index, T item);
    //    //
    //    // Summary:
    //    //     Removes the System.Collections.Generic.IList`1 item at the specified index.
    //    //
    //    // Parameters:
    //    //   index:
    //    //     The zero-based index of the item to remove.
    //    //
    //    // Exceptions:
    //    //   T:System.ArgumentOutOfRangeException:
    //    //     index is not a valid index in the System.Collections.Generic.IList`1.
    //    //
    //    //   T:System.NotSupportedException:
    //    //     The System.Collections.Generic.IList`1 is read-only.
    //    void RemoveAt(int index);


    //}
}