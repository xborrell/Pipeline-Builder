namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class AssemblyNavigator
    {
        private readonly Assembly[] assemblies;

        public AssemblyNavigator(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            this.assemblies = new[] { assembly };
        }

        public AssemblyNavigator(Assembly[] assemblies)
        {
            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public AssemblyNavigator(AppDomain domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            this.assemblies = domain.GetAssemblies();
        }

        public TypeCollection<T> Implementing<T>() where T : class
        {
            var typeImplement = typeof(T);
            var listOfTypesImplementingInterface =
                (from type in this.GetTypes() where typeImplement.IsAssignableFrom(type) && type.IsClass select type)
                    .ToList();

            if (!listOfTypesImplementingInterface.Any() && typeImplement.IsGenericType)
            {
                listOfTypesImplementingInterface = (from type in this.GetTypes()
                                                    where
                                                        type.IsClass
                                                        && ((type.IsGenericType
                                                             && typeImplement.IsAssignableFrom(
                                                                 type.GetGenericTypeDefinition()))
                                                            || type.GetInterfaces()
                                                                   .Any(
                                                                       type1 =>
                                                                       type1.IsGenericType
                                                                       && typeImplement.IsAssignableFrom(
                                                                           type1.GetGenericTypeDefinition())))
                                                    select type).ToList();
            }

            return new TypeCollection<T>(listOfTypesImplementingInterface);
        }

        public TypeCollection<T> InterfacesImplementing<T>() where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Invalid argument, should be a interface", nameof(T));
            }

            var typeImplement = typeof(T);
            var listOfTypesImplementingInterface =
                (from type in this.GetTypes() where typeImplement.IsAssignableFrom(type) && type.IsInterface select type)
                    .ToList();

            if (!listOfTypesImplementingInterface.Any() && typeImplement.IsGenericType)
            {
                listOfTypesImplementingInterface = (from type in this.GetTypes()
                                                    where
                                                        type.IsInterface
                                                        && ((type.IsGenericType
                                                             && typeImplement.IsAssignableFrom(
                                                                 type.GetGenericTypeDefinition()))
                                                            || type.GetInterfaces()
                                                                   .Any(
                                                                       type1 =>
                                                                       type1.IsGenericType
                                                                       && typeImplement.IsAssignableFrom(
                                                                           type1.GetGenericTypeDefinition())))
                                                    select type).ToList();
            }

            return new TypeCollection<T>(listOfTypesImplementingInterface);
        }

        private IEnumerable<Type> GetTypes()
        {
            var types = new List<Type>();
            try
            {
                foreach (var assembly in this.assemblies)
                {
                    types.AddRange(assembly.GetTypes());
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                if (ex.LoaderExceptions == null || !ex.LoaderExceptions.Any())
                {
                    throw;
                }

                // Will composite message to see all not loaded exceptions.
                var sb = new StringBuilder();
                sb.Append($"{ex.Message} all next assemblies has not loaded :");
                foreach (var exl in ex.LoaderExceptions)
                {
                    sb.AppendLine(exl.Message);
                }

                throw new ReflectionTypeLoadException(ex.Types, ex.LoaderExceptions, sb.ToString());
            }

            return types;
        }
    }
}
