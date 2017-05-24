using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// General exception thrown while reading configuration sources. Could be caused by many cases, e.g. stream
    /// exception or source format exception (when no applicable specific exception has been envisioned).
    /// </summary>
    public class SourceReadException : Exception
    {
        /// <inheritdoc cref="Exception"/>
        public SourceReadException()
        {
        }

        /// <inheritdoc cref="Exception"/>
        public SourceReadException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception"/>
        public SourceReadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc cref="Exception"/>
        protected SourceReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}