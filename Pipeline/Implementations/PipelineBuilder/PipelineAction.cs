namespace Pipeline
{
    using System;

    public class PipelineAction : PipelineItem, IPipelineAction
    {
        public PipelineAction(Type step, Type inputType) : base(step, inputType) { }
    }
}