namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public class PipelineFork : IPipelineFork
    {
        private IPipelineLink inputLink;
        private readonly List<IPipelineLink> outputLinks = new List<IPipelineLink>();

        public Type InputType { get; }
        public Type OutputType { get; }
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
            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException("Cannot assign output default link into fork.");
            }

            outputLinks.Add( pipelineLink );
        }

        public void RemoveOutputLink(IPipelineLink pipelineLink)
        {
            outputLinks.Remove(pipelineLink);
        }

        public PipelineFork(Type source)
        {
            this.InputType = source ?? throw new ArgumentNullException(nameof(source));
            this.OutputType = source;
        }
    }
}