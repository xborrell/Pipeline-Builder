namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class Compiler : ICompiler
    {
        private readonly IPipelineBuilder<ICompilerOptions> pipelineBuilder;

        public Compiler(
            IPipelineBuilder<ICompilerOptions> pipelineBuilder
        ) {
            this.pipelineBuilder = pipelineBuilder ?? throw new ArgumentNullException(nameof(pipelineBuilder));
        }

        public Task Compilar(ICompilerOptions options)
        {
            var pipeline = BuildPipeline();

            pipeline.Post(options);
            pipeline.Complete();

            return pipeline.Completion;
        }

        private IDataflowPipeline<ICompilerOptions> BuildPipeline()
        {
            return pipelineBuilder
                .AddTransformation<IValidationStep>()
                .AddAction<IDisplayParametersStep>()
                .Build();

            //cancellationTokenSource = new CancellationTokenSource();
            //var token = cancellationTokenSource.Token;

            //var blockOptions = new ExecutionDataflowBlockOptions { CancellationToken = token };
            //var linkOptions = new DataflowLinkOptions().PropagateCompletion(true);
            //var endBlocks = new List<IDataflowBlock>();

            //var validation = new TransformBlock<ICompilerOptions, ICompilerOptions>(options => validationStep.Ejecutar(options), blockOptions);
            //var displayParameters = new ActionBlock<ICompilerOptions>(options => displayOptionsStep.Ejecutar(options), blockOptions);
            //var readFile = new TransformBlock<ICompilerOptions, string>(options => readFileStep.Ejecutar(options), blockOptions);



            //var compilerOptionsFork = new BroadcastBlock<ICompilerOptions>(options => options); // no need to clone because nobody modifies it
            //validation.LinkTo(compilerOptionsFork, linkOptions);

            //DisplayOptionsStep = factory.Create<IDisplayParametersStep>();
            //var printOptions = new ActionBlock<ICompilerOptions>(options => DisplayOptionsStep.Ejecutar(options), blockOptions);
            //compilerOptionsFork.LinkTo(printOptions, linkOptions);
            //endBlocks.Add(printOptions);

            //ReadFileStep = factory.Create<IReadFileStep>();
            //var readFile = new TransformBlock<ICompilerOptions, string>(options => ReadFileStep.Ejecutar(options), blockOptions);
            //compilerOptionsFork.LinkTo(readFile, linkOptions);

            //CstBuilderStep = factory.Create<IConcreteTreeBuildStep>();
            //var cstBuilder = new TransformBlock<string, IParseTree>(content => CstBuilderStep.Ejecutar(content), blockOptions);
            //readFile.LinkTo(cstBuilder, linkOptions);

            //var cstFork = new BroadcastBlock<IParseTree>(options => options); // no need to clone because nobody modifies it
            //cstBuilder.LinkTo(cstFork, linkOptions);

            //var cstDisplayJoin = new JoinBlock<ICompilerOptions, IParseTree>();
            //compilerOptionsFork.LinkTo(cstDisplayJoin.Target1, linkOptions);
            //cstFork.LinkTo(cstDisplayJoin.Target2, linkOptions);

            //CstDisplayStep = factory.Create<IConcreteTreeDisplayStep>();
            //var cstDisplay = new TransformBlock<Tuple<ICompilerOptions, IParseTree>, string>(content => CstDisplayStep.Ejecutar(content), blockOptions);
            //cstDisplayJoin.LinkTo(cstDisplay, linkOptions);

            //AstBuilderStep = factory.Create<IAbstractTreeBuildStep>();
            //var astBuilder = new TransformBlock<IParseTree, IAstRoot>(tree => AstBuilderStep.Ejecutar(tree), blockOptions);
            //cstFork.LinkTo(astBuilder, linkOptions);

            //var validationStep = LinkStep<IAstValidationStep>(astBuilder, validationRules, blockOptions, linkOptions);

            //var addVariablesJoin = new JoinBlock<ICompilerOptions, IAstRoot>();
            //compilerOptionsFork.LinkTo(addVariablesJoin.Target1, linkOptions);
            //validationStep.LinkTo(addVariablesJoin.Target2, linkOptions);

            //AddExternalVariablesStep = factory.Create<IAddExternalVariablesStep>();
            //var addExternalVariables = new TransformBlock<Tuple<ICompilerOptions, IAstRoot>, IAstRoot>(tree => AddExternalVariablesStep.Ejecutar(tree), blockOptions);
            //addVariablesJoin.LinkTo(addExternalVariables, linkOptions);

            //ResolveVariablesStep = factory.Create<IResolveVariablesRuleStep>();
            //var resolveVariables = new TransformBlock<IAstRoot, IAstRoot>(tree => ResolveVariablesStep.Ejecutar(tree), blockOptions);
            //addExternalVariables.LinkTo(resolveVariables, linkOptions);

            //var transformationStep = LinkStep<IAstTransformationStep>(resolveVariables, transformationRules, blockOptions, linkOptions);

            //SortVersionsStep = factory.Create<IAstSortStep>();
            //var sortVersions = new TransformBlock<IAstRoot, IAstRoot>(tree => SortVersionsStep.Ejecutar(tree), blockOptions);
            //transformationStep.LinkTo(sortVersions, linkOptions);

            //var astTreeFork = new BroadcastBlock<IAstRoot>(options => options); // no need to clone because nobody modifies it past this point
            //sortVersions.LinkTo(astTreeFork, linkOptions);

            //var astDisplayJoin = new JoinBlock<ICompilerOptions, IAstRoot>();
            //compilerOptionsFork.LinkTo(astDisplayJoin.Target1, linkOptions);
            //astTreeFork.LinkTo(astDisplayJoin.Target2, linkOptions);

            //AstDisplayStep = factory.Create<IAbstractTreeDisplayStep>();
            //var astDisplay = new TransformBlock<Tuple<ICompilerOptions, IAstRoot>, string>(content => AstDisplayStep.Ejecutar(content), blockOptions);
            //astDisplayJoin.LinkTo(astDisplay, linkOptions);

            //MacBuilderStep = factory.Create<IMacTreeBuildStep>();
            //var macBuilder = new TransformBlock<IAstRoot, IScriptRoot>(content => MacBuilderStep.Ejecutar(content), blockOptions);
            //astTreeFork.LinkTo(macBuilder, linkOptions);

            //var terminal = new ActionBlock<string>(macTree => Console.WriteLine("Final Step"), blockOptions);
            ////macBuilder.LinkTo(terminal, linkOptions);
            ////endBlocks.Add(terminal);

            //var dataflow = new DataflowFactory().FromPropagator(validation)
            //    .LinkToTarget(displayParameters)
            //    .Create();

            //return (validation, new IDataflowBlock[] { displayParameters, terminal });
        }

        //private ISourceBlock<IAstRoot> LinkStep<T>(ISourceBlock<IAstRoot> source, ICollection<T> compilerSteps,  ExecutionDataflowBlockOptions blockOptions, DataflowLinkOptions linkOptions) 
        //    where T : ICompilerTransformation<IAstRoot, IAstRoot>
        //{
        //    var stepRules = factory.Create<IEnumerable<T>>();

        //    foreach (var step in stepRules)
        //    {
        //        var block = new TransformBlock<IAstRoot, IAstRoot>(ast => step.Ejecutar(ast), blockOptions);
        //        ITargetBlock<IAstRoot> target = block;
        //        source.LinkTo(target, linkOptions);
        //        compilerSteps.Add(step);

        //        source = block;
        //    }

        //    return source;
        //}
    }
}
