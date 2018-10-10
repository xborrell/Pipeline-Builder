namespace Pipeline
{
    using System.Collections.Generic;

    public interface IPipelineSource : IPipelineItem
    {
        IEnumerable<IPipelineLink> OutputLinks { get; }
        void AddOutputLink(IPipelineLink pipelineLink);
        void RemoveOutputLink(IPipelineLink pipelineLink);
    }
}