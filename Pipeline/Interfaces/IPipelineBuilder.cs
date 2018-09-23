namespace Pipeline
{
    public interface IPipelineBuilder<TInput>
    {
        IDataflowPipeline<TInput> Build();
        IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerTransformation;
        IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerTransformation;
    }
}