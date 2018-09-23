namespace Pipeline
{
    using System;

    public interface IPipelineFactory
    {
        IPipelineItem CreateStep<TStep>();
        IPipelineLink CreateLink(bool isDefault, IPipelineTransformation source, IPipelineItem target);
    }
}