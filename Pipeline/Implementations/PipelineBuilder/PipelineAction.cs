namespace Pipeline
{
    using System;
    using System.Linq;

    public class PipelineAction : IPipelineAction
    {
        public Type Step { get; }
        public Type InputType { get; }

        public PipelineAction(Type step, Type inputType)
        {
            Step = step;
            InputType = inputType;
        }
    }
}