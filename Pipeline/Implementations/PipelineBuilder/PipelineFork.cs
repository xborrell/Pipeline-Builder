namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public class PipelineFork : IPipelineFork
    {
        public Type InputType { get; }
        public Type OutputType { get; }

        public IEnumerable<IPipelineLink> Links => links;
        private List<IPipelineLink> links = new List<IPipelineLink>();

        public PipelineFork(Type source)
        {
            this.InputType = source ?? throw new ArgumentNullException(nameof(source));
            this.OutputType = source;
        }

        public void AddLink(IPipelineLink pipelineLink)
        {
            links.Add(pipelineLink);
        }

        public void RemoveLink(IPipelineLink pipelineLink)
        {
            links.Remove(pipelineLink);
        }
    }
}