namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class AbstractTreeDisplayStep : IAbstractTreeDisplayStep
    {
        private readonly ILog log;

        public AbstractTreeDisplayStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task Execute(Tuple<ICompilerOptions, IAstRoot> input)
        {
            log.Info($"Ejecutando {GetType().Name}");
 
            return Task.CompletedTask;
        }
    }
}
