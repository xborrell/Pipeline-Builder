namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class ForceClearCacheStep : IAstTransformationStep
    {
        private readonly ILog log;

        public ForceClearCacheStep(ILog log)
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