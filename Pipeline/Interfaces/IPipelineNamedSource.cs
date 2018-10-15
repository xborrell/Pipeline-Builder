namespace Pipeline
{
    public interface IPipelineNamedSource : IPipelineSource
    {
        string Name { get; }
        void SetName(string name);
    }
}