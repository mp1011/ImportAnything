using ImportAnything.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace ImportAnything.Services.Consolidators
{
    public class ArrayConsolidator<T, E> : IConsolidator<T>
        where T : class, IEnumerable<E>
    {
        T IConsolidator<T>.Consolidate(IEnumerable<T> entries)
        {
            List<E> items = new List<E>();

            foreach (var list in entries)
                items.AddRange(list);


            T ret = default(T);

            //todo - inelegant
            if (typeof(T).IsArray)
                ret = items.ToArray() as T;
            else
                throw new NotImplementedException("Other enumerable types not yet supported");

            return ret;
        }
    }
}
