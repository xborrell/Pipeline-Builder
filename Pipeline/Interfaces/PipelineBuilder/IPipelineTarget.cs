namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineTarget : IPipelineItem
    {
        IEnumerable<IPipelineLink> InputLinks { get; }
        void AddInputLink(IPipelineLink pipelineLink);
        void RemoveInputLink(IPipelineLink pipelineLink);
    }
}