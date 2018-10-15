namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class NoVersionGreaterThanModuleRuleStep : IAstValidationStep
    {
        private readonly ILog log;

        public NoVersionGreaterThanModuleRuleStep(ILog log)
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