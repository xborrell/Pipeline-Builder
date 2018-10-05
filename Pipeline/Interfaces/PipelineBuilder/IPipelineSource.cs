namespace Pipeline
{
    using System;

    public interface IPipelineSource : IPipelineItem
    {
        string Name { get; }
        Type OutputType { get; }

        void SetName(string name);
    }
}