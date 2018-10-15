namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class ConcreteTreeBuildStep : IConcreteTreeBuildStep
    {
        private readonly ILog log;

        public ConcreteTreeBuildStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IParseTree> Execute(string input)
        {
            log.Info($"Ejecutando {GetType().Name}");
 
            return Task.FromResult((IParseTree)null);
        }
    }
}
