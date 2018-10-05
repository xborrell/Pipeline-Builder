namespace Pipeline
{
    using Autofac;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineBuilder<TInput> : IPipelineBuilder<TInput>
    {
        private readonly IPipelineFactory factory;
        private readonly List<IPipelineItem> transformations;
        private IPipelineTarget lastItem;

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

            AddTarget(pipelineItem);

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

            AddTarget(pipelineItem);

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
                CheckSourceType(pipelineItem as IPipelineTarget);
            }

            return null;
        }

        private void CheckSourceType(IPipelineTarget pipelineTarget)
        {
            if (pipelineTarget == null)
            {
                return;
            }

            Type sourceToMatch;
            var numberOfLinks = pipelineTarget.Links.Count();

            if (numberOfLinks == 0)
            {
                sourceToMatch = typeof(TInput);
            } else if (numberOfLinks == 1)
            {
                var link = pipelineTarget.Links.First();

                sourceToMatch = link.Source.OutputType;
            } else
            {
                var parameters = pipelineTarget.Links.Select(link => link.Source.OutputType).ToArray();

                Type precursorType;

                switch (parameters.Length)
                {
                    case 2: 
                        precursorType = typeof(Tuple<,>);
                        break;
                    case 3: 
                        precursorType = typeof(Tuple<,,>);
                        break;
                    case 4: 
                        precursorType = typeof(Tuple<,,,>);
                        break;
                    default : 
                        throw new NotImplementedException();
                }

                sourceToMatch = precursorType.MakeGenericType(parameters);
            }

            if (pipelineTarget.InputType != sourceToMatch)
            {
                throw new PipelineBuilderException($"Expects input type {sourceToMatch.FullName} but found {pipelineTarget.InputType.FullName}.");
            }
        }

        private void LinkByDefault(IPipelineTarget item)
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

        private void AddTarget(IPipelineTarget pipelineItem)
        {
            transformations.Add(pipelineItem);
            LinkByDefault(pipelineItem);
            lastItem = pipelineItem;
        }
    }
}