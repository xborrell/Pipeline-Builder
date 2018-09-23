namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineItem
    {
        Type InputType { get; }
        IEnumerable<IPipelineLink> Links { get; }
        void AddLink(IPipelineLink pipelineLink);
    }
}