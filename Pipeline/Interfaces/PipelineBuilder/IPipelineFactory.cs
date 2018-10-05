namespace Pipeline
{
    using System;

    public interface IPipelineFactory
    {
        IPipelineTarget CreateStep<TStep>();
        IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target);
    }
}