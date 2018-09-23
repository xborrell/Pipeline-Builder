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

        public PipelineFactory(Func<Type, Type, IPipelineAction> actionFactory, Func<Type, Type, Type, IPipelineTransformation> transformationFactory)
        {
            this.actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
            this.transformationFactory = transformationFactory ?? throw new ArgumentNullException(nameof(transformationFactory));
        }

        public IPipelineItem Create<TStep>()
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
                var inputType = implementedTransformation.GetGenericArguments()[0];

                return actionFactory(step, inputType);
            }

            throw new NotImplementedException();
        }
    }
}