namespace Pipeline
{
    using System;
    using System.Reflection;

    public static class FindTypes
    {
        public static AssemblyNavigator InAssemblies(Assembly[] assemblies)
        {
            return new AssemblyNavigator(assemblies);
        }

        public static AssemblyNavigator InAssembly(Assembly assembly)
        {
            return new AssemblyNavigator(assembly);
        }

        public static AssemblyNavigator InDomain(AppDomain domain)
        {
            return new AssemblyNavigator(domain);
        }
    }
}
