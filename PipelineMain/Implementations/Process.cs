namespace Pipeline
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class Process : IDisposable
    {
        private readonly ILog tracer;
        private readonly ICompiler compiler;
        private readonly Func<ICompilerOptions> optionsFactory;

        public Process(ILog tracer, ICompiler compiler, Func<ICompilerOptions> optionsFactory)
        {
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            this.compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
            this.optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
        }

        public async Task<int> Execute(string[] args)
        {
            try
            {
                tracer.Debug("Command line Parsed.");

                var options = optionsFactory();
                options.InputFile = @"C:\Fonts\Newpol\Pipeline\Pipeline\Samples\Sample.txt";
                options.OutputFolder = @"C:\Fonts\Newpol\Pipeline\Pipeline\Samples";
                options.ShowAbstractTree = true;
                options.ShowConcreteTree = true;

                await compiler.Compilar(options).ConfigureAwait(false);
                
                return 0;

            }
            catch (Exception e)
            {
                ShowError(e);
                return 1;
            }
        }

        private void ShowError(Exception e)
        {
            var sb = new StringBuilder();
            var queue = new Queue<Exception>();
            queue.Enqueue(e);

            while (queue.Any())
            {
                var exception = queue.Dequeue();

                if (exception is AggregateException aggregate)
                {
                    foreach (var innerException in aggregate.InnerExceptions)
                    {
                        queue.Enqueue(innerException);
                    }
                }
                else
                {
                    sb.AppendLine(exception.Message);

                    if (exception.InnerException != null)
                    {
                        queue.Enqueue(exception.InnerException);
                    }
                }
            }

            tracer.Error(sb.ToString());
        }

        #region IDisposable implementation
        private bool disposed = false;

        ~Process()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            disposed = true;
        }
        #endregion
    }
}