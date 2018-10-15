namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class DisableCommandsRuleStep : IAstTransformationStep
    {
        private readonly ILog log;

        public DisableCommandsRuleStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IAstRoot> Execute(IAstRoot input)
        {
            log.Info($"Ejecutando {GetType().Name}");
 
            return Task.FromResult(input);
        }
    }
}