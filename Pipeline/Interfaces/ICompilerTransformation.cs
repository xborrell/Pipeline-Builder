namespace Pipeline
{
    using System.Threading.Tasks;

    public interface ICompilerStep
    {

    }

    public interface ICompilerAction<in TInput> : ICompilerStep
    {
        Task Execute(TInput input);
    }

    public interface ICompilerTransformation<in TInput, TOutput> : ICompilerStep
    {
        Task<TOutput> Execute(TInput input);
    }
}