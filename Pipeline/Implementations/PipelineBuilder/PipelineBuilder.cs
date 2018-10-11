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

    public class PipelineBuilder<TInput> : IPipelineBuilder<TInput>
    {
        private static readonly Type CompilerActionGenericType = typeof(ICompilerAction<>);
        private static readonly Type CompilerTransformationGenericType = typeof(ICompilerTransformation<,>);

        private readonly IPipelineFactory<TInput> factory;
        private readonly List<IPipelineItem> pipelineItems;
        private IPipelineTarget lastItem;

        public IEnumerable<IPipelineItem> Items => pipelineItems;

        public PipelineBuilder(IPipelineFactory<TInput> factory)
        {
            pipelineItems = new List<IPipelineItem>();
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IPipelineBuilder<TInput> AddAction<TStep>() where TStep : ICompilerTransformation
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

        public IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerTransformation
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

            var method = GetType().GetMethod("BuildTransformation", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'BuildTransformation'");
            }

            var methodGeneric = method.MakeGenericMethod(stepType, inputType, outputType);

            methodGeneric.Invoke(this, new object[] { name });

            return this;
        }

        private void BuildAction<TStep, TIn>() where TStep : ICompilerAction<TIn>
        {
            var transformation = factory.CreateAction<TStep, TIn>();

            AddTarget(transformation);
        }

        private void BuildTransformation<TStep, TIn, TOut>(string name) where TStep : ICompilerTransformation<TIn, TOut>
        {
            var transformation = factory.CreateTransformation<TStep, TIn, TOut>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                transformation.SetName(name);
            }

            AddTarget(transformation);
        }

        public IPipelineBuilder<TInput> LinkTo(string name)
        {
            name = name.ToLower();

            var transformation = pipelineItems.OfType<IPipelineNamedSource>().First(t => t.Name == name);

            factory.CreateLink(false, transformation, lastItem);

            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            ResolveForks();
            ResolveJoins();
            CheckTypes();

            var pipeline = factory.CreatePipeline();

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
                var join = factory.CreateJoin();

                foreach (var link in target.InputLinks.ToList())
                {
                    link.MoveTargetTo(join);
                }

                factory.CreateLink(false, join, target);

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
                var fork = factory.CreateFork();

                foreach (var link in source.OutputLinks.ToList())
                {
                    link.MoveSourceTo(fork);
                }

                factory.CreateLink(false, source, fork);

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
            
            var firstItem = pipelineItems.First();
            pipeline.MarkAsFirstStep((ITargetBlock<TInput>)firstItem.Block);
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
                    factory.CreateLink(true, transformation, item);
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

        //private void BuildConnections(IDataflowPipeline<TInput> pipeline)
        //{
        //    var method = GetType().GetMethod("LinkBlocks", BindingFlags.NonPublic | BindingFlags.Instance);

        //    if (method == null)
        //    {
        //        throw new Exception("Could not found method 'LinkBlocks'");
        //    }

        //    var inputLinks = Items.OfType<IPipelineTarget>().SelectMany(item => item.InputLinks);

        //    foreach (var link in inputLinks)
        //    {
        //        var linkType = link.Type;
        //        var methodGeneric = method.MakeGenericMethod(linkType);

        //        methodGeneric.Invoke(this, new object[] { pipeline, link });
        //    }
        //}

        //private void LinkBlocks<TLink>(IDataflowPipeline<TInput> pipeline, IPipelineLink link)
        //{
        //    var source = link.Source.Block as ISourceBlock<TLink>;

        //    var targetBlock = link.Target;

        //    ITargetBlock<TLink> target;

        //    if (targetBlock is IPipelineJoin targetJoin)
        //    {
        //        var links = new List<IPipelineLink>();
        //        links.AddRange(targetJoin.InputLinks);

        //        var pos = links.IndexOf(link);

        //        throw new NotImplementedException();
        //    }
        //    else
        //    {
        //        target = targetBlock.Block as ITargetBlock<TLink>;
        //    }


        //    source.LinkTo(target, pipeline.LinkOptions);
        //}


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