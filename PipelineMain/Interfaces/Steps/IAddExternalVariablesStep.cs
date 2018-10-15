namespace Pipeline
{
    using System;

    public interface IAddExternalVariablesStep : ICompilerTransformation<Tuple<ICompilerOptions, IAstRoot>, IAstRoot>
    {
    }
}