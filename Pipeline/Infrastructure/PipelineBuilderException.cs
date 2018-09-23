namespace Pipeline
{
    using System;
    using System.Runtime.Serialization;

    public class PipelineBuilderException : Exception
    {
        public PipelineBuilderException()
        {
        }

        public PipelineBuilderException(string message) : base(message)
        {
        }

        public PipelineBuilderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PipelineBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
