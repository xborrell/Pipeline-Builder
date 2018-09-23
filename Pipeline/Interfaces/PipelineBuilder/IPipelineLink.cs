namespace Pipeline
{
    using System;

    public interface IPipelineLink 
    {
        bool IsDefault { get; }
        IPipelineTransformation Source { get; }
        IPipelineItem Target { get; }
    }
}