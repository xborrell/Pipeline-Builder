namespace Pipeline
{
    using System;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public abstract class PipelineItem : IPipelineItem
    {
        public IDataflowBlock Block { get; protected set; }
        public abstract void ResolveLinkTypes(bool firstItem, Type firstType);
        public abstract void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory);
    }
}