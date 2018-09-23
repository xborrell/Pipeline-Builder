namespace Pipeline
{
    using Autofac;
    using log4net.Config;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        public static Task<int> Main(string[] args)
        {
            XmlConfigurator.Configure();

            var builder = new ContainerBuilder();

            PreloadUnusedAssemblies();
            FindTypes.InAssemblies(AppDomain.CurrentDomain.GetAssemblies()).Implementing<IIoCRegister>().Execute(register => register.RegisterDependencies(builder));

            using (var container = builder.Build())
            {
                using (var context = container.BeginLifetimeScope())
                {
                    using (var process = context.Resolve<Process>())
                    {
                        return process.Execute(args);
                    }
                }
            }
        }

        private static void PreloadUnusedAssemblies()
        {
            var appDomain = AppDomain.CurrentDomain;
            var loaded = appDomain.GetAssemblies();
            var directory = new DirectoryInfo(appDomain.SetupInformation.ApplicationBase);
            var assemblies = directory.GetFiles("*.dll");
            if (!assemblies.Any())
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                if (!loaded.Any(a => !a.IsDynamic && a.Location.Contains(assembly.Name)))
                {
                    appDomain.Load(AssemblyName.GetAssemblyName(assembly.FullName));
                }
            }
        }

        public static void RegisterIoCBootstrappersfromAssemblies(ContainerBuilder builder, params string[] assemblyNames)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var assemblies = assemblyNames.Select(assemblyPath => new AssemblyName(assemblyPath)).Select(Assembly.Load);
            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            if (!enumerable.Any())
            {
                return;
            }

            FindTypes.InAssemblies(enumerable.ToArray()).Implementing<IIoCRegister>().Execute(register => register.RegisterDependencies(builder));
        }
    }
}
