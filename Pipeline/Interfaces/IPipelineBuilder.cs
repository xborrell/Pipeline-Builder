namespace Pipeline
{
    using System.Collections.Generic;

    public interface IPipelineBuilder<TInput>
    {
        IEnumerable<IPipelineItem> Items { get; }
        IDataflowPipeline<TInput> Build();
        IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerStep;
        IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerStep;
        IPipelineBuilder<TInput> LinkTo(string name);
    }
}