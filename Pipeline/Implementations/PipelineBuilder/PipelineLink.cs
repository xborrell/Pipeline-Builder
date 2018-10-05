namespace Pipeline
{
    using System;

    public class PipelineLink : IPipelineLink
    {
        public bool IsDefault { get; }
        public IPipelineSource Source { get; }
        public IPipelineTarget Target { get; }

        public PipelineLink( bool isDefault, IPipelineSource source, IPipelineTarget target)
        {
            IsDefault = isDefault;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}