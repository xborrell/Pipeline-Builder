namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    internal class DataflowPipeline<TIn> : IDataflowPipeline<TIn>
    {
        private ITargetBlock<TIn> firstStep;
        private readonly List<IDataflowBlock> steps;
        private readonly List<IDataflowBlock> lastSteps;

        public ExecutionDataflowBlockOptions BlockOptions { get; }
        public DataflowLinkOptions LinkOptions { get; private set; }
        public IEnumerable<IDataflowBlock> Blocks => steps;
        public IEnumerable<IDataflowBlock> EndSteps => lastSteps;

        public DataflowPipeline()
        {
            steps = new List<IDataflowBlock>();
            lastSteps = new List<IDataflowBlock>();
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            BlockOptions = new ExecutionDataflowBlockOptions
            {
                CancellationToken = token
            };

            LinkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };
        }

        public void AddBlock(IDataflowBlock block)
        {
            if (steps.Count == 0)
            {
                firstStep = (ITargetBlock<TIn>)block;
            }

            steps.Add(block);
        }

        public void AddEndStep(IDataflowBlock block)
        {
            lastSteps.Add(block);
        }

        public void Post(TIn input)
        {
            firstStep.Post(input);
        }

        public void Complete()
        {
            firstStep.Complete();
        }

        public Task Completion => Task.WhenAll(lastSteps.Select(x => x.Completion));
    }
}