namespace Pipeline
{
    public interface IPipelineAction<TStep, TInput> : IPipelineTarget, IPipelineStep<TStep> where TStep : ICompilerAction<TInput>
    {
    }
}