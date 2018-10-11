namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;

    public interface IPipelineTarget : IPipelineItem
    {
        IEnumerable<IPipelineLink> InputLinks { get; }
        void AddInputLink(IPipelineLink pipelineLink);
        void RemoveInputLink(IPipelineLink pipelineLink);
        ITargetBlock<TInput> GetAsTarget<TInput>(IPipelineLink link);
    }
}