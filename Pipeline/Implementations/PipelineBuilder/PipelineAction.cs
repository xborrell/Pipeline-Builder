namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineAction : PipelineItem, IPipelineAction
    {
        private readonly List<IPipelineLink> inputLinks = new List<IPipelineLink>();

        public Type Step { get; }

        public IEnumerable<IPipelineLink> InputLinks => inputLinks;

        public void AddInputLink(IPipelineLink pipelineLink)
        {
            if (!inputLinks.Any())
            {
                inputLinks.Add( pipelineLink );
                return;
            }

            if (pipelineLink.IsDefault)
            {
                throw new InvalidOperationException("Cannot assign input default link into linked transformation.");
            }

            var first = inputLinks.First();

            if (first.IsDefault)
            {
                first.Remove();
            }

            inputLinks.Add( pipelineLink );
        }

        public void RemoveInputLink(IPipelineLink pipelineLink)
        {
            inputLinks.Remove(pipelineLink);
        }
        
        public PipelineAction(Type step)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
        }

        public override string ToString()
        {
            return $"Action<{Step.Name}>";
        }
    }
}