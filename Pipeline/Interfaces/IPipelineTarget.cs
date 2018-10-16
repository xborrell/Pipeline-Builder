namespace Pipeline
{
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;

    internal interface IPipelineTarget : IPipelineItem
    {
        IEnumerable<IPipelineLink> InputLinks { get; }
        void AddInputLink(IPipelineLink pipelineLink);
        void RemoveInputLink(IPipelineLink pipelineLink);
        ITargetBlock<TInput> GetAsTarget<TInput>(IPipelineLink link);
    }
}