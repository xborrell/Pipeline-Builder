namespace Pipeline
{
    using System;

    public interface IPipelineJoin : IPipelineSource, IPipelineTarget
    {
        Type InputType2 { get; }
    }
}