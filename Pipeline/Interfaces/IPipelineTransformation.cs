namespace Pipeline
{
    public interface IPipelineTransformation<TStep, TInput, TOutput> : IPipelineNamedSource, IPipelineTarget, IPipelineStep<TStep> 
        where TStep : class, ICompilerTransformation<TInput,TOutput>
    {
    }

    public interface IPipelineTransformation<TStep, TInput> : IPipelineNamedSource, IPipelineTarget, IPipelineStep<TStep> 
        where TStep : class, ICompilerTransformation<TInput,TInput>
    {
    }
}