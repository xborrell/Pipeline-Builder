namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineTransformation : IPipelineTransformation
    {
        private List<IPipelineLink> inputLinks = new List<IPipelineLink>();
        private List<IPipelineLink> outputLinks = new List<IPipelineLink>();

        public Type InputType { get; }
        public Type OutputType { get; }
        public Type Step { get; }

        public IEnumerable<IPipelineLink> InputLinks => inputLinks;
        public IEnumerable<IPipelineLink> OutputLinks => outputLinks;

        public string Name { get; private set; }
        
        public PipelineTransformation(Type step, Type inputType, Type outputType)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public void AddInputLink(IPipelineLink pipelineLink)
        {
            if (!inputLinks.Any())
            {
                inputLinks.Add( pipelineLink );
                return;
            }

            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException($"Cannot assign input default link into linked transformation.");
            }

            var first = inputLinks.First();

            if (first.IsDefault)
            {
                inputLinks.Remove(first);
            }

            inputLinks.Add( pipelineLink );
        }

        public void RemoveInputLink(IPipelineLink pipelineLink)
        {
            inputLinks.Remove(pipelineLink);
        }

        public void AddOutputLink(IPipelineLink pipelineLink)
        {
            var outputLinksForTarget = from link in outputLinks where link.Target == pipelineLink.Target select link;
            var linkForTarget = outputLinksForTarget.FirstOrDefault();

            if (linkForTarget == null)
            {
                outputLinks.Add( pipelineLink );
                return;
            }

            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException($"Cannot assign output default link into linked transformation.");
            }

            if (linkForTarget.IsDefault)
            {
                outputLinks.Remove(linkForTarget);
            }

            outputLinks.Add( pipelineLink );
        }

        public void RemoveOutputLink(IPipelineLink pipelineLink)
        {
            outputLinks.Remove(pipelineLink);
        }

        public void SetName(string name)
        {
            Name = name.ToLower();
        }

        private void AddLink( string description, List<IPipelineLink> links, IPipelineLink pipelineLink)
        {
            if (!links.Any())
            {
                links.Add( pipelineLink );
                return;
            }

            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException($"Cannot assign {description} default link into linked transformation.");
            }

            var first = links.First();

            if (first.IsDefault)
            {
                links.Remove(first);
            }

            links.Add( pipelineLink );
        }
    }
}