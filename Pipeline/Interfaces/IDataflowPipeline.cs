namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public interface IDataflowPipeline
    {
        IEnumerable<IDataflowBlock> Blocks { get; }
        IEnumerable<IDataflowBlock> EndSteps { get; }
        ExecutionDataflowBlockOptions BlockOptions { get; }
        DataflowLinkOptions LinkOptions { get; }

        void AddBlock(IDataflowBlock block);
        void AddEndStep(IDataflowBlock block);

        void Complete();
        Task Completion { get; }
    }

    public interface IDataflowPipeline<in TIn, out TOut> : IDataflowPipeline
    {
        void Post(TIn input);
        void SetOutput(Action<TOut> outputHolder);
    }
}