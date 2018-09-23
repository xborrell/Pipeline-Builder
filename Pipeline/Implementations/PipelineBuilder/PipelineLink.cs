namespace Pipeline
{
    using System;

    public class PipelineLink : IPipelineLink
    {
        public bool IsDefault { get; }
        public IPipelineTransformation Source { get; }
        public IPipelineItem Target { get; }

        public PipelineLink( bool isDefault, IPipelineTransformation source, IPipelineItem target)
        {
            IsDefault = isDefault;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}