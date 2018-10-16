namespace Pipeline
{
    using System;

    public interface IPipelineOutput<TInput> : IPipelineTarget
    {
        void SetOutput(Action<TInput> holder);
    }
}