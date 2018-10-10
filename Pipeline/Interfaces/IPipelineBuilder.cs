namespace Pipeline
{
    using System.Collections.Generic;

    public interface IPipelineBuilder<TInput>
    {
        IEnumerable<IPipelineItem> Items { get; }

        IDataflowPipeline<TInput> Build();
        IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerTransformation;
        IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerTransformation;
        IPipelineBuilder<TInput> LinkTo(string name);
    }
}