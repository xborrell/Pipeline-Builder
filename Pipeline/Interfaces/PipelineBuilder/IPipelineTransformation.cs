namespace Pipeline
{
    using System;

    public interface IPipelineTransformation : IPipelineItem
    {
        string Name { get; }
        Type OutputType { get; }

        void SetName(string name);
    }
}