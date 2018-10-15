namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Autofac;
    using TASuite.Commons.Crosscutting;

    public class Compiler : ICompiler
    {
        private readonly IIoCAbstractFactory factory;

        public Compiler(IIoCAbstractFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public Task Compilar(ICompilerOptions options)
        {
            var pipeline = BuildPipeline();

            pipeline.ToString();


            pipeline.Post(options);
            pipeline.Complete();

            return pipeline.Completion;
        }

        private IDataflowPipeline<ICompilerOptions> BuildPipeline()
        {
            return PipelineBuilder<ICompilerOptions>.Create(factory)
                .AddTransformation<IValidationStep>("parametersSource")
                .AddAction<IDisplayParametersStep>()
                .AddTransformation<IReadFileStep>()
                .AddTransformation<IConcreteTreeBuildStep>()
                .AddAction<IConcreteTreeDisplayStep>()
                .AddTransformation<IAbstractTreeBuildStep>()
                .AddTransformation<IAstValidationStep>("validation")
                .AddTransformation<IAddExternalVariablesStep>().LinkTo("validation").LinkTo("parametersSource")
                .AddTransformation<IResolveVariablesRuleStep>()
                .AddTransformation<IAstTransformationStep>()
                .AddTransformation<IAstSortStep>("AstSource")
                .AddTransformation<IAbstractTreeDisplayStep>().LinkTo("AstSource").LinkTo("parametersSource")
                .AddTransformation<IMacTreeBuildStep>()
                .Build();
        }
    }
}
