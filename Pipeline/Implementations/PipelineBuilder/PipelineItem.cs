namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class PipelineItem : IPipelineItem
    {
        public Type Step { get; }
        public Type InputType { get; }

        public IEnumerable<IPipelineLink> Links => links;
        private List<IPipelineLink> links = new List<IPipelineLink>();

        protected PipelineItem(Type step, Type inputType)
        {
            Step = step;
            InputType = inputType;
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