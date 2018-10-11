namespace Pipeline
{
    using System;

    public interface IPipelineLink 
    {
        bool IsDefault { get; }
        IPipelineSource Source { get; }
        IPipelineTarget Target { get; }
        Type Type { get; }

        void Remove();
        void MoveSourceTo(IPipelineSource newSource);
        void MoveTargetTo(IPipelineTarget newTarget);
        void SetType(Type newType);
        void Connect<T>(IDataflowPipeline<T> pipeline);
    }
}