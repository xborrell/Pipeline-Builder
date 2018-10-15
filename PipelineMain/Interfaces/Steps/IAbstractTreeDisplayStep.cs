namespace Pipeline
{
    using System;

    public interface IAbstractTreeDisplayStep : ICompilerAction<Tuple<ICompilerOptions, IAstRoot>>
    {
    }
}