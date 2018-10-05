namespace Pipeline.Unit.Tests
{
    using System;
    using Pipeline;

    internal interface IRejectableTransformation : ICompilerTransformation { }

    internal interface IIntAction : ICompilerAction<int> { }

    internal interface IStringAction : ICompilerAction<string> { }

    internal interface ITupla2Action : ICompilerAction<Tuple<int, string>> { }

    internal interface IIntTransformation : ICompilerTransformation<int, int> { }

    internal interface IStringTransformation : ICompilerTransformation<string, string> { }

    internal interface IIntToStringTransformation : ICompilerTransformation<int, string> { }
}
