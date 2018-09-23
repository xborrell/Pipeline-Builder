namespace Pipeline
{
    using Autofac;

    public interface IIoCRegister
    {
        void RegisterDependencies(ContainerBuilder builder);
    }
}
