using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin
{
    public class SourceReadException : Exception
    {
        public SourceReadException()
        {
        }

        public SourceReadException(string message) : base(message)
        {
        }

        public SourceReadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SourceReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}