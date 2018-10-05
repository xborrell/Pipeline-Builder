namespace Pipeline
{
    public interface IPipelineTransformation : IPipelineSource, IPipelineTarget
    {
        string Name { get; }
        void SetName(string name);
    }
}