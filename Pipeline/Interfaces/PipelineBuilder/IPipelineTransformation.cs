namespace Pipeline
{
    using System;

    public interface IPipelineTransformation : IPipelineItem
    {
        Type OutputType { get; }
    }
}