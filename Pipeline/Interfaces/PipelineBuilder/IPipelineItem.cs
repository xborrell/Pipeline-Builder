namespace Pipeline
{
    using System;
    using System.Linq;

    public interface IPipelineItem
    {
        Type InputType { get; }
    }
}