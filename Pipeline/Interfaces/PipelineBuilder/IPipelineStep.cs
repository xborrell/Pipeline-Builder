namespace Pipeline
{
    using System;

    public interface IPipelineStep : IPipelineItem
    {
        Type Step { get; }
    }
}