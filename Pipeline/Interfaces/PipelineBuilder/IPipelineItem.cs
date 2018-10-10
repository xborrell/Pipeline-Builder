namespace Pipeline
{
    using System.Threading.Tasks.Dataflow;

    public interface IPipelineItem
    {
        IDataflowBlock Block { get; }
        void AddBlock(IDataflowBlock block);
    }
}