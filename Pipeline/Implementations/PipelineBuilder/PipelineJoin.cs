namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineJoin : PipelineItem, IPipelineJoin
    {
        private readonly List<IPipelineLink> inputLinks = new List<IPipelineLink>();
        private IPipelineLink outputLink;

        public IEnumerable<IPipelineLink> InputLinks => inputLinks;
        public IEnumerable<IPipelineLink> OutputLinks => new[] { outputLink };

        public void AddInputLink(IPipelineLink pipelineLink)
        {
            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException("Cannot assign input default link into join.");
            }


            inputLinks.Add(pipelineLink);
        }

        public void RemoveInputLink(IPipelineLink pipelineLink)
        {
            inputLinks.Remove(pipelineLink);
        }

        public void AddOutputLink(IPipelineLink pipelineLink)
        {
            if (outputLink != null)
            {
                throw new InvalidOperationException("Cannot replace the assigned output link.");
            }

            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException("Cannot assign output default link into join.");
            }

            outputLink = pipelineLink;
        }

        public void RemoveOutputLink(IPipelineLink pipelineLink)
        {
            outputLink = null;
        }

        public override string ToString()
        {
            return "Join";
        }

    }
}