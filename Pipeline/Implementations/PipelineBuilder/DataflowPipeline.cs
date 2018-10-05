namespace Pipeline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class DataflowPipeline<T> : IDataflowPipeline<T>
    {
        private ITargetBlock<T> firstStep;
        private readonly List<IDataflowBlock> steps;
        private readonly List<IDataflowBlock> lastSteps;

        public ExecutionDataflowBlockOptions BlockOptions { get; }
        public DataflowLinkOptions LinkOptions { get; private set; }

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

        public void MarkAsFirstStep(ITargetBlock<T> block)
        {
            firstStep = block;
        }

        public void AddBlock(IDataflowBlock block)
        {
            steps.Add(block);
        }

        public void AddEndStep(IDataflowBlock block)
        {
            lastSteps.Add(block);
        }

        public void Post(T input)
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