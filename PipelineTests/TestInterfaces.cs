namespace Pipeline.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;

    public  interface IRejectableTransformation : ICompilerTransformation { }

    public  interface IIntAction : ICompilerAction<int> { }

    public  interface IStringAction : ICompilerAction<string> { }

    public  interface ITupla2Action : ICompilerAction<Tuple<int, string>> { }

    public  interface IIntTransformation : ICompilerTransformation<int, int> { }

    public  interface IStringTransformation : ICompilerTransformation<string, string> { }

    public  interface IIntToStringTransformation : ICompilerTransformation<int, string> { }

    public interface ITuplaChained2 : ICompilerAction<Tuple<Tuple<int, string>, int>> { }
}
