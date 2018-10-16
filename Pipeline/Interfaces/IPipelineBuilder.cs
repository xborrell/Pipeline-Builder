namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineBuilder<in TInput, out TOutput>
    {
        IEnumerable<IPipelineItem> Items { get; }
        IDataflowPipeline<TInput, TOutput> Build();
        IPipelineBuilder<TInput, TOutput> AddTransformation<TStep>(string name = "") where TStep : ICompilerStep;
        IPipelineBuilder<TInput, TOutput> AddAction<TStep>() where TStep : ICompilerStep;
        IPipelineBuilder<TInput, TOutput> LinkTo(string name);
        IPipelineBuilder<TInput, TOutput> OutputTo(Action<TOutput> holder);
    }
}