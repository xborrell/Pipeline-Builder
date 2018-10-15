namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using log4net;

    public class MoveNoneBlockContentToSuiteBlockStep : IAstTransformationStep
    {
        private readonly ILog log;

        public MoveNoneBlockContentToSuiteBlockStep(ILog log)
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