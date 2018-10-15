namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class MacTreeHolderStep : IMacTreeHolderStep
    {
        private readonly ILog log;

        public MacTreeHolderStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task Execute(IScriptRoot root)
        {
            log.Info($"Ejecutando {GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
