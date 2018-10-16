namespace Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Autofac;
    using log4net;
    using TASuite.Commons.Crosscutting;

    public class Compiler : ICompiler
    {
        private readonly IIoCAbstractFactory factory;
        public IScriptRoot Root { get; private set; }

        public Compiler(IIoCAbstractFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task Compilar(ICompilerOptions options)
        {
            var pipeline = BuildPipeline();

            pipeline.ToString();


            pipeline.Post(options);
            pipeline.Complete();

            await pipeline.Completion;
        }

        private IDataflowPipeline<ICompilerOptions> BuildPipeline()
        {
            var builder = PipelineBuilder<ICompilerOptions, IScriptRoot>.Create(factory)
                .AddTransformation<IValidationStep>("parametersSource")
                .AddAction<IDisplayParametersStep>()
                .AddTransformation<IReadFileStep>()
                .AddTransformation<IConcreteTreeBuildStep>("concrete")
                .AddAction<IConcreteTreeDisplayStep>().LinkTo("parametersSource").LinkTo("concrete")
                .AddTransformation<IAbstractTreeBuildStep>()
                .AddTransformation<IAstValidationStep>("validation")
                .AddTransformation<IAddExternalVariablesStep>().LinkTo("parametersSource").LinkTo("validation")
                .AddTransformation<IResolveVariablesRuleStep>()
                .AddTransformation<IAstTransformationStep>()
                .AddTransformation<IAstSortStep>("AstSource")
                .AddAction<IAbstractTreeDisplayStep>().LinkTo("parametersSource").LinkTo("AstSource")
                .AddTransformation<IMacTreeBuildStep>()
                .OutputTo(x => Root = x)
                ;

            try
            {
                var pipeline = builder.Build();
                Console.WriteLine(builder.ToString());

                return pipeline;
            }
            catch (Exception)
            {
                Console.WriteLine(builder.ToString());
                throw;
            }
        }
    }
}
