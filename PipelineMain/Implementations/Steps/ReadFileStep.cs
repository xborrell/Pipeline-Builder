namespace Pipeline
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using log4net;

    public class ReadFileStep : IReadFileStep
    {
        private readonly ILog log;

        public ReadFileStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<string> Execute(ICompilerOptions root)
        {
            log.Info($"Ejecutando {GetType().Name}");

            return Task.FromResult(string.Empty);
        }
    }
}