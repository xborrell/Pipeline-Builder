namespace Pipeline
{
    using System;

    public class PipelineTransformation : PipelineItem, IPipelineTransformation
    {
        public Type OutputType { get; }

        public string Name { get; private set; }

        public PipelineTransformation(Type step, Type inputType, Type outputType) : base(step, inputType)
        {
            OutputType = outputType;
        }

        public void SetName(string name)
        {
            Name = name.ToLower();
        }
    }
}