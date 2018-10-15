namespace Pipeline
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    public class AddExternalVariablesStep : IAddExternalVariablesStep
    {
        private readonly ILog log;

        public AddExternalVariablesStep(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IAstRoot> Execute(Tuple<ICompilerOptions, IAstRoot> input)
        {
            log.Info($"Ejecutando {GetType().Name}");
 
            return Task.FromResult(input.Item2);
        }
    }
}