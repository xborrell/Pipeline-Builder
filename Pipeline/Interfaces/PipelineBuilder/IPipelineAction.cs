namespace Pipeline
{
    using System;

    public interface IPipelineAction : IPipelineTarget, IPipelineStep
    {
    }
}