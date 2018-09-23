namespace Pipeline
{
    using System;

    public interface IPipelineFactory
    {
        IPipelineItem Create<TStep>();
    }
}