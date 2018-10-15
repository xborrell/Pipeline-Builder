namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public class PipelineAction<TStep, TInput> : PipelineItem, IPipelineAction<TStep, TInput> where TStep : ICompilerAction<TInput>
    {
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
            return $"Action<{typeof(TStep).Name}>";
        }

        public override void ResolveLinkTypes(bool firstItem, Type firstType)
        {
            if (firstItem)
            {
                if (inputLinks.Count > 0)
                {
                    throw new PipelineBuilderException($"The first action cannot be connected to any other block.");
                }

                if (firstType != typeof(TInput))
                {
                    throw new PipelineBuilderException($"The type of first action must match the pipeline type.");
                }
            }
            else
            {
                if (inputLinks.Count != 1)
                {
                    if (inputLinks.Count == 0)
                    {
                        throw new PipelineBuilderException($"Action without input link.");
                    }

                    throw new PipelineBuilderException($"Action with many input links.");
                }

                inputLinks[0].SetType(typeof(TInput));
            }
        }

        public override void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory)
        {
            var step = factory.Resolve<TStep>();
            Block = new ActionBlock<TInput>(input => step.Execute(input), pipeline.BlockOptions);

            pipeline.AddEndStep(Block);
        }
 
        public ITargetBlock<TIn> GetAsTarget<TIn>(IPipelineLink link)
        {
            return (ITargetBlock<TIn>)Block;
        }
    }
}