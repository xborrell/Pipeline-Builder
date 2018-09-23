namespace Pipeline
{
    using Autofac;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineBuilder<TInput> : IPipelineBuilder<TInput>
    {
        private readonly IPipelineFactory factory;
        private List<IPipelineItem> transformations;
        private IPipelineItem lastItem;

        public PipelineBuilder(IPipelineFactory factory)
        {
            transformations = new List<IPipelineItem>();
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerTransformation
        {
            var pipelineItem = factory.CreateStep<TStep>();

            if (!(pipelineItem is IPipelineAction))
            {
                throw new PipelineBuilderException("The action must implement ICompilerAction<InputType>.");
            }

            AddPipelineItem(pipelineItem);

            return this;
        }

        public IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerTransformation
        {
            var pipelineItem = factory.CreateStep<TStep>();

            if (!(pipelineItem is IPipelineTransformation transformation))
            {
                throw new PipelineBuilderException("The action must implement ICompilerTransformation<InputType, OutputType>.");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                transformation.SetName(name);
            }

            AddPipelineItem(pipelineItem);

            return this;
        }

        public IPipelineBuilder<TInput> LinkTo(string name)
        {
            name = name.ToLower();

            var transformation = transformations.OfType<IPipelineTransformation>().First(t => t.Name == name);

            lastItem.AddLink(factory.CreateLink(false, transformation, lastItem));


            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            foreach (var pipelineItem in transformations)
            {
                CheckInputType(pipelineItem);
            }

            return null;
        }

        private void CheckInputType(IPipelineItem pipelineItem)
        {
            Type sourceToMatch;
            var numberOfLinks = pipelineItem.Links.Count();

            if (numberOfLinks == 0)
            {
                sourceToMatch = typeof(TInput);
            } else if (numberOfLinks == 1)
            {
                var link = pipelineItem.Links.First();

                sourceToMatch = link.Source.OutputType;
            } else
            {
                throw new NotImplementedException();
            }

            if (pipelineItem.InputType != sourceToMatch)
            {
                throw new PipelineBuilderException($"Expects input type {sourceToMatch.Name} but found {pipelineItem.InputType.Name}.");
            }
        }

        private void LinkByDefault(IPipelineItem item)
        {
            var pos = transformations.IndexOf(item);

            while (pos > 0)
            {
                pos--;

                if (transformations[pos] is IPipelineTransformation transformation)
                {
                    item.AddLink(factory.CreateLink(true, transformation, item));
                    break;
                }
            }
        }

        private void AddPipelineItem(IPipelineItem pipelineItem)
        {
            transformations.Add(pipelineItem);
            LinkByDefault(pipelineItem);
            lastItem = pipelineItem;
        }
    }
}