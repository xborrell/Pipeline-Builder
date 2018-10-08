namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineTarget : IPipelineItem
    {
        Type InputType { get; }

        IEnumerable<IPipelineLink> InputLinks { get; }
        void AddInputLink(IPipelineLink pipelineLink);
        void RemoveInputLink(IPipelineLink pipelineLink);
    }
}