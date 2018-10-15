using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Infrastructure
{
    using Autofac;
    using TASuite.Commons.Crosscutting;

    public class IoCAbstractFactory : IIoCAbstractFactory
    {
        private readonly ILifetimeScope scope;

        public IoCAbstractFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public T Resolve<T>()
        {
            return scope.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return scope.Resolve<IEnumerable<T>>();
        }
    }
}
