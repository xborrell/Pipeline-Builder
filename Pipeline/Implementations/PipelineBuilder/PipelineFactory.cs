namespace Pipeline
{
    using System;
    using System.Linq;

    public class PipelineFactory : IPipelineFactory
    {
        private static readonly Type CompilerTransformationGenericType = typeof(ICompilerTransformation<,>);
        private static readonly Type CompilerActionGenericType = typeof(ICompilerAction<>);
        private readonly Func<Type, Type, IPipelineAction> actionFactory;
        private readonly Func<Type, Type, Type, IPipelineTransformation> transformationFactory;
        private readonly Func<bool, IPipelineSource, IPipelineTarget, IPipelineLink> linkFactory;
        private readonly Func<Type, Type, IPipelineJoin> joinFactory;

        public PipelineFactory(
            Func<Type, Type, IPipelineAction> actionFactory, 
            Func<Type, Type, Type, IPipelineTransformation> transformationFactory, 
            Func<bool, IPipelineSource, IPipelineTarget, IPipelineLink> linkFactory, 
            Func<Type, Type, IPipelineJoin> joinFactory)
        {
            this.actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
            this.transformationFactory = transformationFactory ?? throw new ArgumentNullException(nameof(transformationFactory));
            this.linkFactory = linkFactory ?? throw new ArgumentNullException(nameof(linkFactory));
            this.joinFactory = joinFactory ?? throw new ArgumentNullException(nameof(joinFactory));
        }

        public IPipelineLink CreateLink(bool isDefault, IPipelineSource source, IPipelineTarget target)
        {
            return linkFactory(isDefault, source, target);
        }

        public IPipelineJoin CreateJoin(Type source1, Type source2)
        {
            return joinFactory(source1, source2);
        }

        public IPipelineTarget CreateStep<TStep>()
        {
            var step = typeof(TStep);
            var interfacesImplemented = step.GetInterfaces();

            var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CompilerTransformationGenericType);

            if (implementedTransformation != null)
            {
                var inputType = implementedTransformation.GetGenericArguments()[0];
                var outputType = implementedTransformation.GetGenericArguments()[1];

                return transformationFactory(step, inputType, outputType);
            }

            var implementedAction = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CompilerActionGenericType);

            if (implementedAction != null)
            {
                var inputType = implementedAction.GetGenericArguments()[0];

                return actionFactory(step, inputType);
            }

            throw new PipelineBuilderException($"{step.Name} is not a transformation nor action.");
        }
    }
}