namespace Pipeline
{
    public interface IPipelineBuilder<TInput>
    {
        IDataflowPipeline<TInput> Build();
        IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerStep;
        IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerStep;
        IPipelineBuilder<TInput> LinkTo(string name);
    }
}