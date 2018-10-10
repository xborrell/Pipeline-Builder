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
            var pipelineItem = factory.CreateAction<TStep>();

            if (!(pipelineItem is IPipelineAction))
            {
                throw new PipelineBuilderException("The action must implement ICompilerAction<InputType>.");
            }

            AddTarget(pipelineItem);

            return this;
        }

        public IPipelineBuilder<TInput> AddTransformation<TStep>(string name = "") where TStep : ICompilerTransformation
        {
            var pipelineItem = factory.CreateTransformation<TStep>();

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

            var transformation = pipelineItems.OfType<IPipelineTransformation>().First(t => t.Name == name);

            factory.CreateLink(false, transformation, lastItem);

            return this;
        }

        public IDataflowPipeline<TInput> Build()
        {
            ResolveJoins();
            ResolveForks();
            CheckTypes();

            var pipeline = factory.CreatePipeline();

            BuildActions(pipeline);
            BuildTransformations(pipeline);
            BuildForks(pipeline);
            BuildJoins(pipeline);
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
            ResolveLinkTypesFromActions();
            ResolveLinkTypesFromTransformations();
            ResolveLinkTypesFromForks();
            ResolveLinkTypesFromJoins();
        }

        private void LinkByDefault(IPipelineTarget item)
        {
            var pos = pipelineItems.IndexOf(item);

            while (pos > 0)
            {
                pos--;

                if (pipelineItems[pos] is IPipelineTransformation transformation)
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

        private void ResolveLinkTypesFromActions()
        {
            var compilerTransformationGenericType = typeof(ICompilerAction<>);

            var actions = pipelineItems.OfType<IPipelineAction>();

            foreach (var action in actions)
            {
                var stepType = action.Step;
                var interfacesImplemented = stepType.GetInterfaces();

                var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == compilerTransformationGenericType);
                var inputType = implementedTransformation.GetGenericArguments()[0];

                if (action == pipelineItems.First())
                {
                    if (inputType != typeof(TInput))
                    {
                        throw new PipelineBuilderException($"The type of first action must match the pipeline type.");
                    }

                    continue;
                }

                var inputLinks = new List<IPipelineLink>();
                inputLinks.AddRange(action.InputLinks);

                if (inputLinks.Count != 1)
                {
                    if (inputLinks.Count == 0)
                    {
                        throw new PipelineBuilderException($"Action without input link.");
                    }
                    throw new PipelineBuilderException($"Action with many input links.");
                }

                var inputLink = inputLinks[0];

                SetTypeToLink(inputLink, inputType);
            }
        }

        private void ResolveLinkTypesFromTransformations()
        {
            var compilerTransformationGenericType = typeof(ICompilerTransformation<,>);

            var transformations = pipelineItems.OfType<IPipelineTransformation>();

            foreach (var transformation in transformations)
            {
                var stepType = transformation.Step;
                var interfacesImplemented = stepType.GetInterfaces();

                var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == compilerTransformationGenericType);
                var inputType = implementedTransformation.GetGenericArguments()[0];

                if (transformation == pipelineItems.First())
                {
                    if (inputType != typeof(TInput))
                    {
                        throw new PipelineBuilderException($"The type of first action must match the pipeline type.");
                    }
                }
                else
                {
                    var inputLinks = new List<IPipelineLink>();
                    inputLinks.AddRange(transformation.InputLinks);

                    if (inputLinks.Count != 1)
                    {
                        if (inputLinks.Count == 0)
                        {
                            throw new PipelineBuilderException($"Transformation without input link.");
                        }

                        throw new PipelineBuilderException($"Transformation with many input links.");
                    }

                    var inputLink = inputLinks[0];

                    SetTypeToLink(inputLink, inputType);
                }

                var outputLinks = new List<IPipelineLink>();
                outputLinks.AddRange(transformation.OutputLinks);

                if (outputLinks.Count != 1)
                {
                    if (outputLinks.Count == 0)
                    {
                        throw new PipelineBuilderException($"Transformation without output link.");
                    }
                    throw new PipelineBuilderException($"Transformation with many output links.");
                }

                var outputLink = outputLinks[0];

                var outputType = implementedTransformation.GetGenericArguments()[1];

                SetTypeToLink(outputLink, outputType);
            }
        }

        private void ResolveLinkTypesFromForks()
        {
            var forks = pipelineItems.OfType<IPipelineFork>();

            foreach (var fork in forks)
            {
                var inputLink = CheckExpectedLinks(fork.InputLinks, 1, 1).First();
                var outputLinks = CheckExpectedLinks(fork.OutputLinks, 1, -1);

                foreach (var link in outputLinks)
                {
                    SetTypeToLink(link, inputLink.Type);
                }
            }
        }

        private void ResolveLinkTypesFromJoins()
        {
            var joins = pipelineItems.OfType<IPipelineJoin>();

            foreach (var join in joins)
            {
                var inputLinks = CheckExpectedLinks(join.InputLinks, 1, -1);

                var inputTypes = from link in inputLinks select link.Type;
                var types = inputTypes.ToArray();

                Type precursorType;

                switch (types.Length)
                {
                    case 2:
                        precursorType = typeof(Tuple<,>);
                        break;

                    case 3:
                        precursorType = typeof(Tuple<,,>);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                var outputType = precursorType.MakeGenericType(types);

                var outputLink = CheckExpectedLinks(join.OutputLinks, 1, 1).First();
                SetTypeToLink(outputLink, outputType);
            }
        }

        private void BuildActions(IDataflowPipeline<TInput> pipeline)
        {
            var compilerTransformationGenericType = typeof(ICompilerAction<>);

            var method = GetType().GetMethod("BuildAction", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'BuildAction'");
            }

            var actions = pipelineItems.OfType<IPipelineAction>();

            foreach (var action in actions)
            {
                var stepType = action.Step;
                var interfacesImplemented = stepType.GetInterfaces();

                var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == compilerTransformationGenericType);
                var inputType = implementedTransformation.GetGenericArguments()[0];


                var methodGeneric = method.MakeGenericMethod(stepType, inputType);

                var actionBlock = (IDataflowBlock)methodGeneric.Invoke(this, new object[] { pipeline });

                pipeline.AddBlock(actionBlock);
                action.AddBlock(actionBlock);
                pipeline.AddEndStep(actionBlock);

                if (action == pipelineItems.First())
                {
                    pipeline.MarkAsFirstStep((ITargetBlock<TInput>)actionBlock);
                }
            }
        }

        private void BuildTransformations(IDataflowPipeline<TInput> pipeline)
        {
            var compilerTransformationGenericType = typeof(ICompilerTransformation<,>);

            var method = GetType().GetMethod("BuildTransformation", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'BuildTransformation'");
            }

            var transformations = pipelineItems.OfType<IPipelineTransformation>();

            foreach (var transformation in transformations)
            {
                var stepType = transformation.Step;
                var interfacesImplemented = stepType.GetInterfaces();

                var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == compilerTransformationGenericType);
                var inputType = implementedTransformation.GetGenericArguments()[0];
                var outputType = implementedTransformation.GetGenericArguments()[1];

                var methodGeneric = method.MakeGenericMethod(stepType, inputType, outputType);

                var transformationBlock = (IDataflowBlock)methodGeneric.Invoke(this, new object[] { pipeline });

                pipeline.AddBlock(transformationBlock);
                transformation.AddBlock(transformationBlock);

                if (transformation == pipelineItems.First())
                {
                    pipeline.MarkAsFirstStep((ITargetBlock<TInput>)transformationBlock);
                }
            }
        }

        private void BuildForks(IDataflowPipeline<TInput> pipeline)
        {
            var method = GetType().GetMethod("BuildFork", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'BuildFork'");
            }

            var forks = pipelineItems.OfType<IPipelineFork>();

            foreach (var fork in forks)
            {
                var inputType = fork.InputLinks.First().Type;
                var methodGeneric = method.MakeGenericMethod(inputType);

                var forkBlock = (IDataflowBlock)methodGeneric.Invoke(this, new object[] { pipeline });

                pipeline.AddBlock(forkBlock);
                fork.AddBlock(forkBlock);
            }
        }

        private void BuildJoins(IDataflowPipeline<TInput> pipeline)
        {
            var joins = pipelineItems.OfType<IPipelineJoin>();

            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "BuildJoin").ToList();

            var join2Method = methods.FirstOrDefault(x => x.IsGenericMethod && x.GetGenericArguments().Length == 2);
            if (join2Method == null)
            {
                throw new Exception("Could not found method 'BuildJoin - 2'");
            }

            var join3Method = methods.FirstOrDefault(x => x.IsGenericMethod && x.GetGenericArguments().Length == 3);
            if (join3Method == null)
            {
                throw new Exception("Could not found method 'BuildJoin - 3'");
            }

            foreach (var join in joins)
            {
                var types = from link in @join.InputLinks select link.Type;
                var typesArray = types.ToArray();

                MethodInfo methodGeneric;
                switch (typesArray.Length)
                {
                    case 2:
                        methodGeneric = join2Method.MakeGenericMethod(types.ToArray());
                        break;

                    case 3:
                        methodGeneric = join3Method.MakeGenericMethod(types.ToArray());
                        break;

                    default:
                        throw new NotImplementedException();
                }

                var joinkBlock = (IDataflowBlock)methodGeneric.Invoke(this, new object[] { pipeline });

                pipeline.AddBlock(joinkBlock);
                join.AddBlock(joinkBlock);
            }
        }

        private void BuildConnections(IDataflowPipeline<TInput> pipeline)
        {
            var method = GetType().GetMethod("LinkBlocks", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'LinkBlocks'");
            }

            var inputLinks = Items.OfType<IPipelineTarget>().SelectMany(item => item.InputLinks);

            foreach (var link in inputLinks)
            {
                var linkType = link.Type;
                var methodGeneric = method.MakeGenericMethod(linkType);

                methodGeneric.Invoke(this, new object[] { pipeline, link });
            }
        }

        private ActionBlock<TInputAction> BuildAction<TStep, TInputAction>(IDataflowPipeline<TInput> pipeline) where TStep : ICompilerAction<TInputAction>
        {
            var step = factory.CreateCompilerStep<TStep>();
            return new ActionBlock<TInputAction>(options => step.Ejecutar(options), pipeline.BlockOptions);
        }

        private TransformBlock<TInputTransformation, TOutputTransformation> BuildTransformation<TStep, TInputTransformation, TOutputTransformation>(IDataflowPipeline<TInput> pipeline)
            where TStep : ICompilerTransformation<TInputTransformation, TOutputTransformation>
        {
            var step = factory.CreateCompilerStep<TStep>();
            return new TransformBlock<TInputTransformation, TOutputTransformation>(options => step.Ejecutar(options), pipeline.BlockOptions);
        }

        private BroadcastBlock<TInputFork> BuildFork<TInputFork>(IDataflowPipeline<TInput> pipeline)
        {
            return new BroadcastBlock<TInputFork>(options => options, pipeline.BlockOptions);
        }

        private JoinBlock<TInput1, TInput2> BuildJoin<TInput1, TInput2>(IDataflowPipeline<TInput> pipeline)
        {
            return new JoinBlock<TInput1, TInput2>();
        }

        private JoinBlock<TInput1, TInput2, TInput3> BuildJoin<TInput1, TInput2, TInput3>(IDataflowPipeline<TInput> pipeline)
        {
            return new JoinBlock<TInput1, TInput2, TInput3>();
        }

        private void LinkBlocks<TLink>(IDataflowPipeline<TInput> pipeline, IPipelineLink link)
        {
            var source = link.Source.Block as ISourceBlock<TLink>;
            var target = link.Target.Block as ITargetBlock<TLink>;

            source.LinkTo(target, pipeline.LinkOptions);
        }

        private void SetTypeToLink(IPipelineLink link, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (link.Type == null)
            {
                link.Type = type;
                return;
            }

            if (link.Type != type)
            {
                throw new PipelineBuilderException($"Detected inconsistency. Expected {type.FullName} but found {link.Type.FullName}.");
            }
        }

        private List<IPipelineLink> CheckExpectedLinks(IEnumerable<IPipelineLink> linksToCheck, int minimumLinks, int maximumLinks)
        {
            var links = new List<IPipelineLink>();
            links.AddRange(linksToCheck);

            if (links.Count < minimumLinks)
            {
                throw new PipelineBuilderException($"Too few links.");
            }

            if (maximumLinks > 0 && links.Count > maximumLinks)
            {
                throw new PipelineBuilderException($"Too many links.");
            }

            return links;
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