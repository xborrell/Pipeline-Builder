namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;

    public abstract class PipelineItem : IPipelineItem
    {
        public IDataflowBlock Block { get; private set; }

        public void AddBlock(IDataflowBlock block)
        {
            if (Block != null)
            {
                throw new InvalidOperationException();
            }

            Block = block;
        }
    }
}