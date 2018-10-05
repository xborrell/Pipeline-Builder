namespace Pipeline
{
    using System;

    public interface IPipelineSource : IPipelineItem
    {
        Type OutputType { get; }
    }
}