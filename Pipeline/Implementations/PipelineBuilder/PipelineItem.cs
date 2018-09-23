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
            if(links.Count == 0 && !pipelineLink.IsDefault )
            {
                throw new PipelineBuilderException("The first link must be default");
            }

            if (links.Count > 0 && pipelineLink.IsDefault)
            {
                throw new PipelineBuilderException("Can't add more than one default link");
            }

            if ( links.Any( x => x.IsDefault ) )
            {
                links.RemoveAll(x => x.IsDefault);
            }
            
            links.Add(pipelineLink);
        }
    }
}