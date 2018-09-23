namespace Pipeline
{
    using Autofac;
    using System;
    using System.Collections.Generic;

    public class PipelineBuilder<TInput> : IPipelineBuilder<TInput>
    {
        private readonly IPipelineFactory factory;
        private List<IPipelineItem> transformations;

        public PipelineBuilder(IPipelineFactory factory)
        {
            transformations = new List<IPipelineItem>();
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerTransformation
        {
            var pipelineItem = factory.Create<TStep>();

            if (!(pipelineItem is IPipelineAction))
            {
                throw new PipelineBuilderException("The action must implement ICompilerAction<InputType>.");
            }

            transformations.Add(pipelineItem);

            return this;
        }

        public IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerTransformation
        {
            var pipelineItem = factory.Create<TStep>();

            if (!(pipelineItem is IPipelineTransformation transformation))
            {
                throw new PipelineBuilderException("The action must implement ICompilerTransformation<InputType, OutputType>.");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                //transformation.Name
            }

            transformations.Add(pipelineItem);

            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            IPipelineTransformation linkedItem = null;

            foreach (var pipelineItem in transformations)
            {
                CheckInputType(linkedItem, pipelineItem);
                linkedItem = pipelineItem as IPipelineTransformation;
            }

            return null;
        }

        private void CheckInputType(IPipelineTransformation linkedItem, IPipelineItem pipelineItem)
        {
            var outputLastType = GetLastOutputType(linkedItem);

            if (pipelineItem.InputType != outputLastType)
            {
                throw new PipelineBuilderException($"Expects input type {outputLastType.Name} but found {pipelineItem.InputType.Name}.");
            }
        }

        private Type GetLastOutputType(IPipelineTransformation linkedItem)
        {
            return linkedItem == null
                ? typeof(TInput)
                : linkedItem.OutputType
                ;
        }
    }
}