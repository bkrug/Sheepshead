using System;
using System.Collections.Generic;

namespace Sheepshead.Logic
{
    public static class TypeExtenders
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            var comparer = EqualityComparer<T>.Default;
            var i = 0;
            foreach(T value in list)
            {
                if (comparer.Equals(item, value))
                    return i;
                ++i;
            }
            return -1;
        }
    }
}
