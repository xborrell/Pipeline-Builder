namespace Pipeline
{
    public interface IAbstractTreeBuildStep : ICompilerTransformation<IParseTree, IAstRoot>
    {
    }
}