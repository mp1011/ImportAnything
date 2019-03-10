using System.Collections.Generic;

namespace ImportAnything.Services.Interfaces
{
    public interface IConsolidator { }

    public interface IConsolidator<T> : IConsolidator
    {
        T Consolidate(IEnumerable<T> entries);
    }
}
