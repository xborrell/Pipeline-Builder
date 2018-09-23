namespace Pipeline
{
    using System.Threading.Tasks;

    public interface IDataflowPipeline<T>
    {
        void Post(T input);
        void Complete();
        Task Completion { get; }
    }
}