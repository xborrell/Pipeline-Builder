namespace Pipeline
{
    using System;
    using System.Linq;

    public class PipelineFactory : IPipelineFactory
    {
        private static Type compilerTransformationGenericType = typeof(ICompilerTransformation<,>);
        private static Type compilerActionGenericType = typeof(ICompilerAction<>);
        private readonly Func<Type, Type, IPipelineAction> actionFactory;
        private readonly Func<Type, Type, Type, IPipelineTransformation> transformationFactory;
        private readonly Func<bool, IPipelineTransformation, IPipelineItem, IPipelineLink> linkFactory;

        public PipelineFactory(
            Func<Type, Type, IPipelineAction> actionFactory, 
            Func<Type, Type, Type, IPipelineTransformation> transformationFactory, 
            Func<bool, IPipelineTransformation, IPipelineItem, IPipelineLink> linkFactory)
        {
            this.actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
            this.transformationFactory = transformationFactory ?? throw new ArgumentNullException(nameof(transformationFactory));
            this.linkFactory = linkFactory ?? throw new ArgumentNullException(nameof(linkFactory));
        }

        public IPipelineLink CreateLink(bool isDefault, IPipelineTransformation source, IPipelineItem target)
        {
            return linkFactory(isDefault, source, target);
        }

        public IPipelineItem CreateStep<TStep>()
        {
            var step = typeof(TStep);
            var interfacesImplemented = step.GetInterfaces();

            var implementedTransformation = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == compilerTransformationGenericType);

            if (implementedTransformation != null)
            {
                var inputType = implementedTransformation.GetGenericArguments()[0];
                var outputType = implementedTransformation.GetGenericArguments()[1];

                return transformationFactory(step, inputType, outputType);
            }

            var implementedAction = interfacesImplemented.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == compilerActionGenericType);

            if (implementedAction != null)
            {
                var inputType = implementedAction.GetGenericArguments()[0];

                return actionFactory(step, inputType);
            }

            throw new PipelineBuilderException($"{step.Name} is not a transformation nor action.");
        }
    }
}