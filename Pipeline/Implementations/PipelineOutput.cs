namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public class PipelineOutput<TInput> : PipelineItem, IPipelineOutput<TInput>
    {
        private Action<TInput> holderAction;
        private readonly List<IPipelineLink> inputLinks = new List<IPipelineLink>();

        public IEnumerable<IPipelineLink> InputLinks => inputLinks;

        public void AddInputLink(IPipelineLink pipelineLink)
        {
            if (!inputLinks.Any())
            {
                inputLinks.Add(pipelineLink);
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

            inputLinks.Add(pipelineLink);
        }

        public void RemoveInputLink(IPipelineLink pipelineLink)
        {
            inputLinks.Remove(pipelineLink);
        }

        public override string ToString()
        {
            return "Output";
        }

        public override void ResolveLinkTypes(bool firstItem, Type firstType)
        {
            if (firstItem)
            {
                if (inputLinks.Count > 0)
                {
                    throw new PipelineBuilderException($"The first output cannot be connected to any other block.");
                }

                if (firstType != typeof(TInput))
                {
                    throw new PipelineBuilderException($"The type of first output must match the pipeline type.");
                }
            }
            else
            {
                if (inputLinks.Count != 1)
                {
                    if (inputLinks.Count == 0)
                    {
                        throw new PipelineBuilderException($"Output without input link.");
                    }

                    throw new PipelineBuilderException($"Output with many input links.");
                }

                inputLinks[0].SetType(typeof(TInput));
            }
        }

        public void SetOutput(Action<TInput> holder)
        {
            this.holderAction = holder;
        }

        public override void BuildBlock(IDataflowPipeline pipeline, IIoCAbstractFactory factory)
        {
            var block = new ActionBlock<TInput>(input => holderAction(input), pipeline.BlockOptions);

            AddBlock(pipeline, block);
            pipeline.AddEndStep(block);
        }

        public ITargetBlock<TIn> GetAsTarget<TIn>(IPipelineLink link)
        {
            return (ITargetBlock<TIn>)blocks[0];
        }
    }
}