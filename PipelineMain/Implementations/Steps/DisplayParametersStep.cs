namespace Pipeline
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using log4net;

    public class DisplayParametersStep : IDisplayParametersStep
    {
        private readonly ILog log;

        public DisplayParametersStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task Execute(ICompilerOptions options)
        {
            log.Info($"Ejecutando {GetType().Name}");

            return Task.CompletedTask;
        }
    }
}