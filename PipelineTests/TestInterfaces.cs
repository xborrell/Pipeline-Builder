namespace Pipeline.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;

    internal  interface IRejectableTransformation : ICompilerStep { }

    internal  interface IIntAction : ICompilerAction<int> { }

    internal  interface IStringAction : ICompilerAction<string> { }

    internal  interface ITupla2Action : ICompilerAction<Tuple<int, string>> { }

    internal  interface IIntTransformation : ICompilerTransformation<int, int> { }

    internal  interface IStringTransformation : ICompilerTransformation<string, string> { }

    internal  interface IIntToStringTransformation : ICompilerTransformation<int, string> { }

    internal interface ITuplaChained2 : ICompilerAction<Tuple<Tuple<int, string>, int>> { }
}
