namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public class PipelineTransformation<TStep, TInput, TOutput> : PipelineItem, IPipelineTransformation<TStep, TInput, TOutput> 
        where TStep : ICompilerTransformation<TInput,TOutput>
    {
        private List<IPipelineLink> inputLinks = new List<IPipelineLink>();
        private List<IPipelineLink> outputLinks = new List<IPipelineLink>();

        public IEnumerable<IPipelineLink> InputLinks => inputLinks;
        public IEnumerable<IPipelineLink> OutputLinks => outputLinks;

        public string Name { get; private set; }
        
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
        
        public override string ToString()
        {
            return $"Transformation<{typeof(TStep).Name}>";
        }
 
        public override void ResolveLinkTypes(bool firstItem, Type firstType)
        {
            if (firstItem)
            {
                if (inputLinks.Count > 0)
                {
                    throw new PipelineBuilderException($"The first transformation cannot be connected to any other block.");
                }

                if (firstType != typeof(TInput))
                {
                    throw new PipelineBuilderException($"The type of first transformation must match the pipeline type.");
                }
            }
            else
            {
                if (inputLinks.Count != 1)
                {
                    if (inputLinks.Count == 0)
                    {
                        throw new PipelineBuilderException($"Transformation without input link.");
                    }

                    throw new PipelineBuilderException($"Transformation with many input links.");
                }

                inputLinks[0].SetType(typeof(TInput));
            }

            if (outputLinks.Count != 1)
            {
                if (outputLinks.Count == 0)
                {
                    throw new PipelineBuilderException($"Transformation without output link.");
                }
                throw new PipelineBuilderException($"Transformation with many output links.");
            }

            outputLinks[0].SetType(typeof(TOutput));
        }
 
        public override void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory)
        {
            var step = factory.Resolve<TStep>();
            Block = new TransformBlock<TInput, TOutput>(input => step.Execute(input), pipeline.BlockOptions);
        }

        public ISourceBlock<TOut> GetAsSource<TOut>(IPipelineLink link)
        {
            return (ISourceBlock<TOut>)Block;
        }
 
        public ITargetBlock<TIn> GetAsTarget<TIn>(IPipelineLink link)
        {
            return (ITargetBlock<TIn>)Block;
        }
    }
}