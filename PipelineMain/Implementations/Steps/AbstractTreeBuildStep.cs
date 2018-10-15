namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class AbstractTreeBuildStep : IAbstractTreeBuildStep
    {
        private readonly ILog log;

        public AbstractTreeBuildStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IAstRoot> Execute(IParseTree input)
        {
            log.Info($"Ejecutando {GetType().Name}");
 
            return Task.FromResult((IAstRoot)null);
        }
    }
}
