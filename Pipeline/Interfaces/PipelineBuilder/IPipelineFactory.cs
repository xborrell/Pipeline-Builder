namespace Pipeline
{
    using System;

    public interface IPipelineFactory<TInput>
    {
        IPipelineTarget CreateAction<TStep>();
        IPipelineTarget CreateTransformation<TStep>();
        IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target);
        IPipelineJoin CreateJoin();
        IPipelineFork CreateFork();
        IDataflowPipeline<TInput> CreatePipeline();
        TCompilerStep CreateCompilerStep<TCompilerStep>();
    }
}