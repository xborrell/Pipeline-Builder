namespace Pipeline
{
    using log4net;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DisplayParametersStep : IDisplayParametersStep
    {
        private readonly ILog tracer;

        public DisplayParametersStep(ILog tracer)
        {
            this.tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public Task Ejecutar(ICompilerOptions options)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"-------------- Settings ---------------");
            sb.AppendLine($"   Compile and execute {options.InputFile}");
            sb.AppendLine($"   Output Folder for auxiliary files = {options.OutputFolder}");
            sb.AppendLine($"   Write Concrete Syntax Tree (cst)  = {options.ShowConcreteTree}");
            sb.AppendLine($"   Write Abstract Syntax Tree (ast)  = {options.ShowAbstractTree}");

            var txt = sb.ToString();

            tracer.Info(txt);

            return Task.CompletedTask;
        }
    }
}