namespace Pipeline
{
    using System;

    public interface IPipelineFactory
    {
        IPipelineTarget CreateStep<TStep>();
        IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target);
        IPipelineJoin CreateJoin(Type source1, Type source2);
        IPipelineFork CreateFork(Type source);
    }
}