namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineJoin : IPipelineJoin
    {
        public Type InputType { get; }
        public Type InputType2 { get; }
        public Type OutputType { get; }

        public IEnumerable<IPipelineLink> Links => links;
        private List<IPipelineLink> links = new List<IPipelineLink>();

        public PipelineJoin(Type source1, Type source2)
        {
            this.InputType = source1 ?? throw new ArgumentNullException(nameof(source1));
            this.InputType2 = source2 ?? throw new ArgumentNullException(nameof(source2));

            var precursorType = typeof(Tuple<,>);
            OutputType = precursorType.MakeGenericType(source1, source2);
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