namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineJoin : IPipelineJoin
    {
        private List<IPipelineLink> inputLinks = new List<IPipelineLink>();
        private IPipelineLink outputLink;

        public Type InputType { get; }
        public Type InputType2 { get; }
        public Type OutputType { get; }


        public IEnumerable<IPipelineLink> InputLinks => inputLinks;
        public IEnumerable<IPipelineLink> OutputLinks => new[] { outputLink };


        public PipelineJoin(Type source1, Type source2)
        {
            this.InputType = source1 ?? throw new ArgumentNullException(nameof(source1));
            this.InputType2 = source2 ?? throw new ArgumentNullException(nameof(source2));

            var precursorType = typeof(Tuple<,>);
            OutputType = precursorType.MakeGenericType(source1, source2);
        }

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
    }
}