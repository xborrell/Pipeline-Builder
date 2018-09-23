namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TypeCollection<T>
        where T : class
    {
        private readonly IEnumerable<Type> listOfTypes;

        public TypeCollection(IEnumerable<Type> listOfTypes)
        {
            this.listOfTypes = listOfTypes ?? new List<Type>();
        }

        public IEnumerable<Type> ListOfTypes => this.listOfTypes;

        public void Execute(Action<T> action)
        {
            foreach (var instanceOfType in this.listOfTypes.Select(type => Activator.CreateInstance(type) as T))
            {
                action(instanceOfType);
            }
        }

        public IEnumerable<T> GetAllInstances()
        {
            return this.listOfTypes.Select(type => Activator.CreateInstance(type) as T).ToList();
        }

        public IEnumerable<T> GetAllInstancesFromIoC(IServiceProvider provider)
        {
            return this.listOfTypes.Select(type => provider.GetService(type) as T).ToList();
        }

        public IEnumerable<T> GetInstancesAndExecuteAction(Action<T> action)
        {
            var instances = new List<T>();
            foreach (var instanceOfType in this.listOfTypes.Select(type => Activator.CreateInstance(type) as T))
            {
                instances.Add(instanceOfType);
                action(instanceOfType);
            }

            return instances;
        }

        public IEnumerable<T> GetInstancesFromIoCAndExecuteAction(Action<T> action, IServiceProvider provider)
        {
            var instances = new List<T>();
            foreach (var instanceOfType in this.listOfTypes.Select(type => provider.GetService(type) as T))
            {
                instances.Add(instanceOfType);
                action(instanceOfType);
            }

            return instances;
        }
    }
}
