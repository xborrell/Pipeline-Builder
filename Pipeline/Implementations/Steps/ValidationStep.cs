namespace Pipeline
{
    using log4net;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class ValidationStep : IValidationStep
    {
        private readonly ILog tracer;

        public ValidationStep(ILog tracer)
        {
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public Task<ICompilerOptions> Ejecutar(ICompilerOptions options)
        {
            if (!File.Exists(options.InputFile))
            {
                throw new Exception($"file {options.InputFile} not found.");
            }

            if (!Directory.Exists(options.OutputFolder))
            {
                throw new Exception($"folder {options.OutputFolder} not found");
            }

            tracer.Debug("Compiler parameters validated.");

            return Task.FromResult(options);
        }
    }
}