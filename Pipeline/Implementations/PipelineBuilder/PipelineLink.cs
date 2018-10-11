namespace Pipeline
{
    using System;

    public class PipelineLink : IPipelineLink
    {
        public bool IsDefault { get; }
        public IPipelineSource Source { get; private set; }
        public IPipelineTarget Target { get; private set;}
        public Type Type { get; private set; }

        public PipelineLink( bool isDefault, IPipelineSource source, IPipelineTarget target)
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
    }
}