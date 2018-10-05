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

            var lastLink = lastItem.Links.FirstOrDefault();
            if (lastLink == null)
            {
                throw new InvalidOperationException("Missing Default link");
            }

            if (lastLink.IsDefault)
            {
                lastItem.RemoveLink(lastLink);
                lastItem.AddLink(factory.CreateLink(false, transformation, lastItem));

                return this;
            }

            var join = factory.CreateJoin(lastLink.Source.OutputType, transformation.OutputType );

            join.AddLink(factory.CreateLink(false, join, lastItem));
            join.AddLink(factory.CreateLink(false, lastLink.Source, join));
            lastItem.RemoveLink(lastLink);
            join.AddLink(factory.CreateLink(false, transformation, join));

            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            foreach (var pipelineItem in transformations)
            {
                CheckPipelineItem((dynamic)pipelineItem);
            }

            return null;
        }

        private void CheckPipelineItem(IPipelineItem item)
        {
            throw new NotImplementedException();
        }

        private void CheckPipelineItem(IPipelineJoin join)
        {
            var numberOfLinks = join.Links.Count();

            if (numberOfLinks != 2)
            {
                throw new InvalidOperationException();
            }
            
            var source1ToMatch = join.Links.First().Source.OutputType;
            var target1ToMatch = join.InputType;

            CheckTypes(source1ToMatch, target1ToMatch);

            var source2ToMatch = join.Links.First().Source.OutputType;
            var target2ToMatch = join.InputType2;

            CheckTypes(source2ToMatch, target2ToMatch);
        }

        private void CheckPipelineItem(IPipelineAction action)
        {
            var sourceToMatch = !action.Links.Any()
                ? typeof(TInput)
                : action.Links.First().Source.OutputType;

            var targetToMatch = action.InputType;

            CheckTypes(sourceToMatch, targetToMatch);
        }

        private void CheckPipelineItem(IPipelineTransformation transformation)
        {

            var sourceToMatch = !transformation.Links.Any()
                ? typeof(TInput)
                : transformation.Links.First().Source.OutputType;

            var targetToMatch = transformation.InputType;

            CheckTypes(sourceToMatch, targetToMatch);
        }

        private void CheckTypes(Type sourceToMatch, Type targetToMatch)
        {
            if (sourceToMatch != targetToMatch)
            {
                throw new PipelineBuilderException($"Expects input type {sourceToMatch.FullName} but found {targetToMatch.FullName}.");
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