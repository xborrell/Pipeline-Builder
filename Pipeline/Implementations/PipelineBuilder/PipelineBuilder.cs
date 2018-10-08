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

        public IEnumerable<IPipelineItem> Transformations => transformations;

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

            factory.CreateLink(false, transformation, lastItem);

            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            ResolveJoins();

            foreach (var pipelineItem in transformations)
            {
                CheckPipelineItem((dynamic)pipelineItem);
            }

            return null;
        }

        private void ResolveJoins()
        {
            var joinCandidates = from transformation in transformations.OfType<IPipelineTarget>()
                                 where transformation.InputLinks.Count() > 1
                                 select transformation;
            ;

            foreach (var target in joinCandidates.ToList())
            {
                var types = from link in target.InputLinks select link.Source.OutputType;

                var join = factory.CreateJoin(types.ToArray());

                var targetLinks = new List<IPipelineLink>(target.InputLinks);

                foreach (var link in targetLinks)
                {
                    target.RemoveInputLink(link);

                    var source = link.Source;

                    factory.CreateLink(false, source, join);
                }
                factory.CreateLink(false, join, target);

                transformations.Add(join);
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
                    factory.CreateLink(true, transformation, item);
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

        private void CheckPipelineItem(IPipelineItem item)
        {
            throw new NotImplementedException();
        }

        private void CheckPipelineItem(IPipelineJoin join)
        {
            var numberOfLinks = join.InputLinks.Count();

            if (numberOfLinks != 2)
            {
                throw new InvalidOperationException();
            }

            var source1ToMatch = join.InputLinks.First().Source.OutputType;
            var target1ToMatch = join.InputType;

            CheckTypes(source1ToMatch, target1ToMatch);

            var source2ToMatch = join.InputLinks.Last().Source.OutputType;
            var target2ToMatch = join.InputType2;

            CheckTypes(source2ToMatch, target2ToMatch);
        }

        private void CheckPipelineItem(IPipelineAction action)
        {
            var sourceToMatch = !action.InputLinks.Any()
                ? typeof(TInput)
                : action.InputLinks.First().Source.OutputType;

            var targetToMatch = action.InputType;

            CheckTypes(sourceToMatch, targetToMatch);
        }

        private void CheckPipelineItem(IPipelineTransformation transformation)
        {
            var sourceToMatch = !transformation.InputLinks.Any()
                ? typeof(TInput)
                : transformation.InputLinks.First().Source.OutputType;

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
    }
}