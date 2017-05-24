using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin
{
    /// <summary>
    /// Thrown when domain rules of configuration are infringed.
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        /// <inheritdoc cref="Exception"/>
        public InvalidConfigurationException()
        {
        }

        /// <inheritdoc cref="Exception"/>
        public InvalidConfigurationException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception"/>
        public InvalidConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc cref="Exception"/>
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}