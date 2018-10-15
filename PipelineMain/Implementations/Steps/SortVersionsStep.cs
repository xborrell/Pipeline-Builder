namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class SortVersionsStep : IAstSortStep
    {
        private readonly ILog log;

        public SortVersionsStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IAstRoot> Execute(IAstRoot root)
        {
            log.Info($"Ejecutando {GetType().Name}");

            return Task.FromResult(root);
        }
    }
}