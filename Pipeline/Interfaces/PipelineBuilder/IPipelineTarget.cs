namespace Pipeline
{
    using System;
    using System.Collections.Generic;

    public interface IPipelineTarget : IPipelineItem
    {
        Type InputType { get; }
    }
}