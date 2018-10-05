namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineItem
    {
        IEnumerable<IPipelineLink> Links { get; }
        void AddLink(IPipelineLink pipelineLink);
    }
}