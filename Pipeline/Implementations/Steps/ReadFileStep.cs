namespace Pipeline
{
    using System.IO;
    using System.Threading.Tasks;

    public class ReadFileStep : IReadFileStep
    {
        public async Task<string> Ejecutar(ICompilerOptions options)
        {
            using (var reader = new StreamReader(options.InputFile))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}