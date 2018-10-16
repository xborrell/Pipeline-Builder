namespace Pipeline
{
    internal interface IPipelineAction<TStep, TInput> : IPipelineTarget, IPipelineStep<TStep> where TStep : ICompilerAction<TInput>
    {
    }
}