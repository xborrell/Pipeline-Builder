namespace Pipeline
{
    using System;

    public class PipelineTransformation : IPipelineTransformation
    {
        private static Type compilerTransformationGenericType = typeof(ICompilerTransformation<,>);
        private static Type compilerActionGenericType = typeof(ICompilerAction<>);

        public Type Step { get; }
        public Type InputType { get; }
        public Type OutputType { get; }

        public PipelineTransformation(Type step, Type inputType, Type outputType)
        {
            Step = step;
            InputType = inputType;
            OutputType = outputType;
        }
    }
}