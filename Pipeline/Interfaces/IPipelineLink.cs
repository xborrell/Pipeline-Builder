﻿namespace Pipeline
{
    using System;

    internal interface IPipelineLink 
    {
        bool IsDefault { get; }
        IPipelineSource Source { get; }
        IPipelineTarget Target { get; }
        Type Type { get; }

        void Remove();
        void MoveSourceTo(IPipelineSource newSource);
        void MoveTargetTo(IPipelineTarget newTarget);
        void SetType(Type newType);
        void Connect(IDataflowPipeline pipeline);
    }
}