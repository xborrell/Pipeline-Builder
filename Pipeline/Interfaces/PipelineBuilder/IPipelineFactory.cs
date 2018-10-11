namespace Pipeline
{
    using System;

    public interface IPipelineFactory<TInput>
    {
        IPipelineAction<TStep, TIn> CreateAction<TStep, TIn>() where TStep : ICompilerAction<TIn>;
        IPipelineTransformation<TStep, TIn, TOut> CreateTransformation<TStep, TIn, TOut>() where TStep : ICompilerTransformation<TIn, TOut>;
        IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target);
        IPipelineJoin CreateJoin();
        IPipelineFork CreateFork();
        IDataflowPipeline<TInput> CreatePipeline();
        TCompilerStep CreateCompilerStep<TCompilerStep>();
    }
}