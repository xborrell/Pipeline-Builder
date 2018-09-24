﻿namespace Pipeline
{
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public interface IDataflowPipeline<T>
    {
        void MarkAsFirstStep(ITargetBlock<T> block);
        void AddBlock(IDataflowBlock block);
        void AddEndStep(IDataflowBlock block);

        void Post(T input);
        void Complete();
        Task Completion { get; }
    }
}