namespace Pipeline
{
    using System;

    internal interface IPipelineOutput<TInput> : IPipelineTarget
    {
        void SetOutput(Action<TInput> holder);
    }
}