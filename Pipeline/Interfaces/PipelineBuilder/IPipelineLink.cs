namespace Pipeline
{
    using System;

    public interface IPipelineLink 
    {
        bool IsDefault { get; }
        IPipelineSource Source { get; }
        IPipelineTarget Target { get; }
    }
}