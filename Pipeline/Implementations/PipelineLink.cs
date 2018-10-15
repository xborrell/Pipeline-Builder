namespace Pipeline
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks.Dataflow;

    public class PipelineLink : IPipelineLink
    {
        public bool IsDefault { get; }
        public IPipelineSource Source { get; private set; }
        public IPipelineTarget Target { get; private set; }
        public Type Type { get; private set; }

        public PipelineLink(bool isDefault, IPipelineSource source, IPipelineTarget target)
        {
            IsDefault = isDefault;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));

            target.AddInputLink(this);
            source.AddOutputLink(this);
        }

        public void Remove()
        {
            Target.RemoveInputLink(this);
            Source.RemoveOutputLink(this);
            Source = null;
            Target = null;
        }

        public void MoveSourceTo(IPipelineSource newSource)
        {
            Source.RemoveOutputLink(this);
            newSource.AddOutputLink(this);
            Source = newSource;
        }

        public void MoveTargetTo(IPipelineTarget newTarget)
        {
            Target.RemoveInputLink(this);
            newTarget.AddInputLink(this);
            Target = newTarget;
        }

        public void SetType(Type newType)
        {
            if (newType == null)
            {
                throw new ArgumentNullException(nameof(newType));
            }

            if (Type == null)
            {
                Type = newType;
                return;
            }

            if (Type != newType)
            {
                throw new PipelineBuilderException($"Detected inconsistency. Expected {newType.FullName} but found {Type.FullName}.");
            }
        }

        public void Connect<T>(IDataflowPipeline<T> pipeline)
        {
            var method = GetType().GetMethod("ConnectBlocks", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception("Could not found method 'ConnectBlocks'");
            }

            var methodGeneric = method.MakeGenericMethod(Type, typeof(T));

            methodGeneric.Invoke(this, new object[]{pipeline});
        }

        private void ConnectBlocks<TLink, T>(IDataflowPipeline<T> pipeline)
        {
            var source = Source.GetAsSource<TLink>(this);
            var target = Target.GetAsTarget<TLink>(this);

            source.LinkTo(target, pipeline.LinkOptions);
        }
    }
}