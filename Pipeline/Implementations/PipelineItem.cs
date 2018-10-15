namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public abstract class PipelineItem : IPipelineItem
    {
        public IEnumerable<IDataflowBlock> Blocks => blocks;
        protected List<IDataflowBlock> blocks = new List<IDataflowBlock>();

        public abstract void ResolveLinkTypes(bool firstItem, Type firstType);
        public abstract void BuildBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IIoCAbstractFactory factory);

        protected void AddBlock<TPipelineType>(IDataflowPipeline<TPipelineType> pipeline, IDataflowBlock block)
        {
            pipeline.AddBlock(block);
            blocks.Add(block);
        }
    }
}