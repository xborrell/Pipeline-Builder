namespace Pipeline
{
    using Autofac;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;

    public class PipelineBuilder<TInput> : IPipelineBuilder<TInput>
    {
        private static readonly Type CompilerActionGenericType = typeof(ICompilerAction<>);
        private static readonly Type CompilerTransformationGenericType = typeof(ICompilerTransformation<,>);

        private readonly IIoCAbstractFactory factory;
        private readonly List<IPipelineItem> pipelineItems;
        private IPipelineTarget lastItem;

        public IEnumerable<IPipelineItem> Items => pipelineItems;

        public static PipelineBuilder<TInput> Create(IIoCAbstractFactory factory)
        {
            return new PipelineBuilder<TInput>(factory);
        }

        protected PipelineBuilder(IIoCAbstractFactory factory)
        {
            pipelineItems = new List<IPipelineItem>();
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerStep
        {
            var stepType = typeof(TStep);

            var interfacesImplemented = stepType.GetInterfaces();

            var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CompilerActionGenericType);

            if (implementedTransformation == null)
            {
                throw new PipelineBuilderException($"{stepType.Name} is not an action.");
            }

            var inputType = implementedTransformation.GetGenericArguments()[0];

            var method = GetType().GetMethod("BuildAction", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'BuildAction'");
            }

            var methodGeneric = method.MakeGenericMethod(stepType, inputType);

            methodGeneric.Invoke(this, new object[0]);

            return this;
        }

        public IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerStep
        {
            var stepType = typeof(TStep);

            var interfacesImplemented = stepType.GetInterfaces();

            var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CompilerTransformationGenericType);

            if (implementedTransformation == null)
            {
                throw new PipelineBuilderException($"{stepType.Name} is not a transformation.");
            }

            var inputType = implementedTransformation.GetGenericArguments()[0];
            var outputType = implementedTransformation.GetGenericArguments()[1];

            string methodName = "BuildTransformation1";
            var typeParameters = new List<Type>() { stepType, inputType };

            if (inputType != outputType)
            {
                typeParameters.Add(outputType);
                methodName = "BuildTransformation2";
            }


            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception($"Could not found method '{methodName}'");
            }


            var methodGeneric = method.MakeGenericMethod(typeParameters.ToArray());

            methodGeneric.Invoke(this, new object[] { name });

            return this;
        }

        private void BuildAction<TStep, TIn>() where TStep : ICompilerAction<TIn>
        {
            var action = new PipelineAction<TStep, TIn>();

            AddTarget(action);
        }

        private void BuildTransformation1<TStep, TIn>(string name) where TStep : class, ICompilerTransformation<TIn, TIn>
        {
            var transformation = new PipelineTransformation<TStep, TIn>();

            AddTarget(transformation);

            if (!string.IsNullOrWhiteSpace(name))
            {
                transformation.SetName(name);
            }
        }

        private void BuildTransformation2<TStep, TIn, TOut>(string name) where TStep : class, ICompilerTransformation<TIn, TOut>
        {
            var transformation = new PipelineTransformation<TStep, TIn, TOut>();

            AddTarget(transformation);

            if (!string.IsNullOrWhiteSpace(name))
            {
                transformation.SetName(name);
            }
        }

        public IPipelineBuilder<TInput> LinkTo(string name)
        {
            name = name.ToLower();

            var transformation = pipelineItems.OfType<IPipelineNamedSource>().First(t => t.Name == name);

            new PipelineLink(false, transformation, lastItem);

            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            ResolveForks();
            ResolveJoins();
            CheckTypes();

            var pipeline = new DataflowPipeline<TInput>();

            BuildBlocks(pipeline);
            BuildConnections(pipeline);

            return pipeline;
        }

        private void ResolveJoins()
        {
            var joinCandidates = from transformation in pipelineItems.OfType<IPipelineTarget>()
                                 where transformation.InputLinks.Count() > 1
                                 select transformation;
            ;

            foreach (var target in joinCandidates.ToList())
            {
                var join = new PipelineJoin();

                foreach (var link in target.InputLinks.ToList())
                {
                    link.MoveTargetTo(join);
                }

                new PipelineLink(false, join, target);

                pipelineItems.Add(join);
            }
        }

        private void ResolveForks()
        {
            var forkCandidates = from transformation in pipelineItems.OfType<IPipelineSource>()
                                 where transformation.OutputLinks.Count() > 1
                                 select transformation;
            ;

            foreach (var source in forkCandidates.ToList())
            {
                var fork = new PipelineFork();

                foreach (var link in source.OutputLinks.ToList())
                {
                    link.MoveSourceTo(fork);
                }

                new PipelineLink(false, source, fork);

                pipelineItems.Add(fork);
            }
        }

        private void CheckTypes()
        {
            var firstItem = pipelineItems.First();

            foreach (var pipelineItem in Items)
            {
                pipelineItem.ResolveLinkTypes(pipelineItem == firstItem, typeof(TInput));
            }
        }

        private void BuildBlocks(IDataflowPipeline<TInput> pipeline)
        {
            foreach (var pipelineItem in Items)
            {
                pipelineItem.BuildBlock(pipeline, factory);
            }
        }

        private void BuildConnections(IDataflowPipeline<TInput> pipeline)
        {
            var links = Items.OfType<IPipelineSource>().SelectMany(source => source.OutputLinks)
                .Union(
                        Items.OfType<IPipelineTarget>().SelectMany(target => target.InputLinks)
                );

            foreach (var link in links)
            {
                link.Connect(pipeline);
            }
        }

        private void LinkByDefault(IPipelineTarget item)
        {
            var pos = pipelineItems.IndexOf(item);

            while (pos > 0)
            {
                pos--;

                if (pipelineItems[pos] is IPipelineSource transformation)
                {
                    new PipelineLink(true, transformation, item);
                    break;
                }
            }
        }

        private void AddTarget(IPipelineTarget pipelineItem)
        {
            pipelineItems.Add(pipelineItem);
            LinkByDefault(pipelineItem);
            lastItem = pipelineItem;
        }

        public override string ToString()
        {
            List<IPipelineItem> items = new List<IPipelineItem>();
            items.AddRange(Items);

            StringBuilder sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.AppendLine(itemName(items, item));
                var s = LinkNames(items, item as IPipelineSource);

                if (s != String.Empty)
                {
                    sb.AppendLine(s);
                }
            }

            return sb.ToString();
        }

        private string itemName(List<IPipelineItem> items, IPipelineItem item)
        {
            return $"[{items.IndexOf(item)}] {item}";
        }

        private string LinkNames(List<IPipelineItem> items, IPipelineSource source)
        {
            if (source == null) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var link in source.OutputLinks)
            {
                sb.AppendLine($"    --> [{items.IndexOf(link.Target)}]");
            }
            return sb.ToString().TrimEnd();
        }
    }
}