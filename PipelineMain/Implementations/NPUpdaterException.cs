namespace Pipeline
{
    using System;
    using System.Runtime.Serialization;

    public class NPUpdaterException : Exception
    {
        public NPUpdaterException()
        {
        }

        public NPUpdaterException(string message) : base(message)
        {
        }

        public NPUpdaterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NPUpdaterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
