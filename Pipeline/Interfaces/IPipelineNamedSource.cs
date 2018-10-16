namespace Pipeline
{
    internal interface IPipelineNamedSource : IPipelineSource
    {
        string Name { get; }
        void SetName(string name);
    }
}