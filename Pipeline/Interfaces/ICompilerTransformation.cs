namespace Pipeline
{
    using System.Threading.Tasks;

    public interface ICompilerTransformation
    {

    }

    public interface ICompilerAction<in TInput> : ICompilerTransformation
    {
        Task Ejecutar(TInput input);
    }

    public interface ICompilerTransformation<in TInput, TOutput> : ICompilerTransformation
    {
        Task<TOutput> Ejecutar(TInput input);
    }
}