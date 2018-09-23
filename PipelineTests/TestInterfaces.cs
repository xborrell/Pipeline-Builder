namespace Pipeline.Unit.Tests
{
    using Pipeline;

    internal interface IRejectableTransformation : ICompilerTransformation { }

    internal interface IIntAction : ICompilerAction<int> { }

    internal interface IStringAction : ICompilerAction<string> { }

    internal interface IIntTransformation : ICompilerTransformation<int, int> { }

    internal interface IIntToStringTransformation : ICompilerTransformation<int, string> { }
}
