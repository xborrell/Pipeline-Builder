namespace Pipeline
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public interface IDataflowPipeline<T>
    {
        IEnumerable<IDataflowBlock> Blocks { get; }
        ExecutionDataflowBlockOptions BlockOptions { get; }
        DataflowLinkOptions LinkOptions { get; }

        void AddBlock(IDataflowBlock block);
        void AddEndStep(IDataflowBlock block);

        void Post(T input);
        void Complete();
        Task Completion { get; }
    }
}