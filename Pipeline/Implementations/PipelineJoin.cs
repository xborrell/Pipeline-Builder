namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public class PipelineJoin : PipelineItem, IPipelineJoin
    {
        private readonly List<IPipelineLink> inputLinks = new List<IPipelineLink>();
        private IPipelineLink outputLink;
        private List<IDataflowBlock> sources;


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

        public override void ResolveLinkTypes(bool firstItem, Type firstType)
        {
            if (firstItem)
            {
                throw new InvalidOperationException("Join block can't be first item");
            }

            if (inputLinks.Count < 1)
            {
                throw new PipelineBuilderException($"Join without input links.");
            }

            if (outputLink == null)
            {
                throw new PipelineBuilderException($"Join without output link.");
            }

            var inputTypes = from link in inputLinks select link.Type;
            var types = inputTypes.ToArray();

            Type precursorType;

            switch (types.Length)
            {
                case 2:
                    precursorType = typeof(Tuple<,>);
                    break;

                case 3:
                    precursorType = typeof(Tuple<,,>);
                    break;

                default:
                    throw new NotImplementedException();
            }

            var outputType = precursorType.MakeGenericType(types);

            outputLink.SetType(outputType);
        }


        public override void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory)
        {
            var types = from link in InputLinks select link.Type;
            var typesList = types.ToList();

            string methodName;
            switch (typesList.Count)
            {
                case 2:
                    methodName = "BuildJoinBlock2";
                    break;

                case 3:
                    methodName = "BuildJoinBlock3";
                    break;

                default:
                    throw new NotImplementedException();
            }
            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception($"Could not found method '{methodName}'");
            }

            typesList.Insert(0, typeof(TPipelineType));
            var methodGeneric = method.MakeGenericMethod(typesList.ToArray());

            methodGeneric.Invoke(this, new[] { pipeline });
        }

        public ISourceBlock<TOut> GetAsSource<TOut>(IPipelineLink link)
        {
            return (ISourceBlock<TOut>)blocks[0];
        }

        public ITargetBlock<TIn> GetAsTarget<TIn>(IPipelineLink link)
        {
            var pos = inputLinks.IndexOf(link);

            return (ITargetBlock<TIn>)sources[pos];
        }

        private void BuildJoinBlock2<TPipelineType, TIn1, TIn2>(IDataflowPipeline<TPipelineType> pipeline)
        {
            var joinBlock = new JoinBlock<TIn1, TIn2>();

            sources = new List<IDataflowBlock>
            {
                joinBlock.Target1,
                joinBlock.Target2
            };

            AddBlock(pipeline, joinBlock);
        }

        private void BuildJoinBlock3<TPipelineType, TIn1, TIn2, TIn3>(IDataflowPipeline<TPipelineType> pipeline)
        {
            var joinBlock = new JoinBlock<TIn1, TIn2, TIn3>();
            sources = new List<IDataflowBlock>
            {
                joinBlock.Target1,
                joinBlock.Target2,
                joinBlock.Target3
            };

            AddBlock(pipeline, joinBlock);
        }
    }
}