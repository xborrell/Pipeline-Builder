namespace Pipeline
{
    public interface IPipelineTransformation<TStep, TInput, TOutput> : IPipelineNamedSource, IPipelineTarget, IPipelineStep<TStep> 
        where TStep : ICompilerTransformation<TInput,TOutput>
    {
    }
}