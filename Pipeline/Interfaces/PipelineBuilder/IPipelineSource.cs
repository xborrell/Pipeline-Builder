namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineSource : IPipelineItem
    {
        Type OutputType { get; }

        IEnumerable<IPipelineLink> OutputLinks { get; }
        void AddOutputLink(IPipelineLink pipelineLink);
        void RemoveOutputLink(IPipelineLink pipelineLink);
    }
}