namespace Pipeline
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using log4net;

    public class ValidationStep : IValidationStep
    {
        private readonly ILog log;

        public ValidationStep(ILog tracer)
        {
            this.log = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public Task<ICompilerOptions> Execute(ICompilerOptions options)
        {
            if (!File.Exists(options.InputFile))
            {
                throw new NPUpdaterException($"file {options.InputFile} not found.");
            }

            if (!Directory.Exists(options.OutputFolder))
            {
                throw new NPUpdaterException($"folder {options.OutputFolder} not found");
            }

            log.Info($"Ejecutando {GetType().Name}");

            return Task.FromResult(options);
        }
    }
}