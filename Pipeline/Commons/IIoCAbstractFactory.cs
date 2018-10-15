namespace TASuite.Commons.Crosscutting
{
    using System;
    using System.Collections.Generic;

    public interface IIoCAbstractFactory
    {
        bool IsRoot { get; }

        T Resolve<T>();
        IEnumerable<T> ResolveAll<T>() where T : class;
    }
}
