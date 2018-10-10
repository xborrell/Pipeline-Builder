namespace Pipeline
{
    public interface IPipelineTransformation : IPipelineSource, IPipelineTarget, IPipelineStep
    {
        string Name { get; }
        void SetName(string name);
    }
}