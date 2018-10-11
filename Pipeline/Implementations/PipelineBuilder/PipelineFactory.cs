﻿namespace Pipeline
{
    using System;
    using System.Linq;
    using Autofac;

    public class PipelineFactory<TInput> : IPipelineFactory<TInput>
    {
        private readonly ILifetimeScope scope;
        private static readonly Type CompilerTransformationGenericType = typeof(ICompilerTransformation<,>);
        private static readonly Type CompilerActionGenericType = typeof(ICompilerAction<>);

        public PipelineFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target)
        {
            return scope.Resolve<IPipelineLink>(
                new PositionalParameter(0, isDefault),
                new PositionalParameter(1, source),
                new PositionalParameter(2, target)
            );
        }

        public IPipelineJoin CreateJoin()
        {
            return scope.Resolve<IPipelineJoin>();
        }

        public IPipelineFork CreateFork()
        {
            return scope.Resolve<IPipelineFork>();
        }

        public IDataflowPipeline<TInput> CreatePipeline()
        {
            return scope.Resolve<IDataflowPipeline<TInput>>();
        }

        public TCompilerStep CreateCompilerStep<TCompilerStep>()
        {
            return scope.Resolve<TCompilerStep>();
        }

        public IPipelineAction<TStep, TIn> CreateAction<TStep, TIn>() where TStep : ICompilerAction<TIn>
        {
            var step = typeof(TStep);

            var interfacesImplemented = step.GetInterfaces();

            var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CompilerActionGenericType);

            if (implementedTransformation == null)
            {
                throw new PipelineBuilderException($"{step.Name} is not an action.");
            }

            return scope.Resolve<IPipelineAction<TStep, TIn>>();
        }

        public IPipelineTransformation<TStep, TIn, TOut> CreateTransformation<TStep, TIn, TOut>() where TStep : ICompilerTransformation<TIn, TOut>
        {
            var step = typeof(TStep);
            var interfacesImplemented = step.GetInterfaces();

            var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CompilerTransformationGenericType);

            if (implementedTransformation == null)
            {
                throw new PipelineBuilderException($"{step.Name} is not a transformation.");
            }

            return scope.Resolve<IPipelineTransformation<TStep, TIn, TOut>>();
        }
    }
}