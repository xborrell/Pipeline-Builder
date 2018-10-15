namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class MacTreeBuildStep : IMacTreeBuildStep
    {
        private readonly ILog log;

        public MacTreeBuildStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IScriptRoot> Execute(IAstRoot root)
        {
            log.Info($"Ejecutando {GetType().Name}");

            return Task.FromResult((IScriptRoot)null);
        }
    }
}
