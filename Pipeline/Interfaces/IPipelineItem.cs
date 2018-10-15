namespace Pipeline
{
    using System;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public interface IPipelineItem
    {
        IDataflowBlock Block { get; }
        void ResolveLinkTypes(bool firstItem, Type firstType);
        void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory);
    }
}