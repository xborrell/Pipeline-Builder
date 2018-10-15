namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class ConcreteTreeDisplayStep : IConcreteTreeDisplayStep
    {
        private readonly ILog log;

        public ConcreteTreeDisplayStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task Execute(Tuple<ICompilerOptions, IParseTree> input)
        {
            log.Info($"Ejecutando {GetType().Name}");
 
            return Task.CompletedTask;
        }
    }
}
