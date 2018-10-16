namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public interface IPipelineItem
    {
        IEnumerable<IDataflowBlock> Blocks { get; }

        void ResolveLinkTypes(bool firstItem, Type firstType);
        void BuildBlock(IDataflowPipeline pipeline, IIoCAbstractFactory factory);
    }
}