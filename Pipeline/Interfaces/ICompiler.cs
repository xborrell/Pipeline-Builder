namespace Pipeline
{
    using System.Threading.Tasks;

    public interface ICompiler
    {
        Task Compilar(ICompilerOptions options);
    }
}