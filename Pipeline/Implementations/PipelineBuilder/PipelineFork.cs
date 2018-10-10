namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public class PipelineFork : PipelineItem, IPipelineFork
    {
        private IPipelineLink inputLink;
        private readonly List<IPipelineLink> outputLinks = new List<IPipelineLink>();

        public IEnumerable<IPipelineLink> InputLinks => new []{inputLink};
        public IEnumerable<IPipelineLink> OutputLinks => outputLinks;

        public void AddInputLink(IPipelineLink pipelineLink)
        {
            if (inputLink != null)
            {
                throw new InvalidOperationException("Cannot replace the assigned input link.");
            }

            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException("Cannot assign input default link into fork.");
            }

            inputLink = pipelineLink;
        }

        public void RemoveInputLink(IPipelineLink pipelineLink)
        {
            inputLink = null;
        }
        public void AddOutputLink(IPipelineLink pipelineLink)
        {
            outputLinks.Add( pipelineLink );
        }

        public void RemoveOutputLink(IPipelineLink pipelineLink)
        {
            outputLinks.Remove(pipelineLink);
        }

        public override string ToString()
        {
            return "Fork";
        }
    }
}