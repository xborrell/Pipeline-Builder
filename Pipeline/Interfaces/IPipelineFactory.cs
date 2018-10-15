namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineFactory<TInput>
    {
        IEnumerable<IPipelineTransformation<TStep, TIn, TOut>> CreateTransformation<TStep, TIn, TOut>() where TStep : ICompilerTransformation<TIn, TOut>;
        IPipelineAction<TStep, TIn> CreateAction<TStep, TIn>() where TStep : ICompilerAction<TIn>;
        IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target);
        IPipelineJoin CreateJoin();
        IPipelineFork CreateFork();
        IDataflowPipeline<TInput> CreatePipeline();
        TCompilerStep CreateCompilerStep<TCompilerStep>();
    }
}