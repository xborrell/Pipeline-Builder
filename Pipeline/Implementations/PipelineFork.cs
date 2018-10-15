namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public class PipelineFork : PipelineItem, IPipelineFork
    {
        private IPipelineLink inputLink;
        private readonly List<IPipelineLink> outputLinks = new List<IPipelineLink>();

        public IEnumerable<IPipelineLink> InputLinks => new[] { inputLink };
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
            outputLinks.Add(pipelineLink);
        }

        public void RemoveOutputLink(IPipelineLink pipelineLink)
        {
            outputLinks.Remove(pipelineLink);
        }

        public override string ToString()
        {
            return "Fork";
        }

        public override void ResolveLinkTypes(bool firstItem, Type firstType)
        {
            if (firstItem)
            {
                throw new InvalidOperationException("Fork block can't be first item");
            }

            if (inputLink == null)
            {
                throw new PipelineBuilderException($"Fork without input link.");
            }

            if (outputLinks.Count < 1)
            {
                throw new PipelineBuilderException($"Fork without output links.");
            }

            foreach (var link in outputLinks)
            {
                link.SetType(inputLink.Type);
            }
        }
 
        public override void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory)
        {
            var method = GetType().GetMethod("BuildForkBlock", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'BuildForkBlock'");
            }

            var methodGeneric = method.MakeGenericMethod(inputLink.Type);

            methodGeneric.Invoke(this, new object[]{pipeline.BlockOptions});
        }
 
        private void BuildForkBlock<TIn>(ExecutionDataflowBlockOptions options)
        {
            Block = new BroadcastBlock<TIn>(input => input, options);
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