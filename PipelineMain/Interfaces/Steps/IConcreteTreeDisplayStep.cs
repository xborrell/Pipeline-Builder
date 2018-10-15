namespace Pipeline
{
    using System;

    public interface IConcreteTreeDisplayStep : ICompilerAction<Tuple<ICompilerOptions, IParseTree>>
    {
    }
}