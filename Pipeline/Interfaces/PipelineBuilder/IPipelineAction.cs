namespace Pipeline
{
    using System;

    public interface IPipelineAction<TStep, TInput> : IPipelineTarget, IPipelineStep<TStep> where TStep : ICompilerAction<TInput>
    {
    }
}