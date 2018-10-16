namespace Pipeline
{
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;

    internal interface IPipelineSource : IPipelineItem
    {
        IEnumerable<IPipelineLink> OutputLinks { get; }
        void AddOutputLink(IPipelineLink pipelineLink);
        void RemoveOutputLink(IPipelineLink pipelineLink);
        ISourceBlock<TOutput> GetAsSource<TOutput>(IPipelineLink link);
    }
}