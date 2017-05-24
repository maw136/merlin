using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// Thrown when configuration domain expressed with YAML source is invalid (e.g missing or disorder of expected 
    /// YAML node names/sequences/etc.). In this case YAML source is valid according to the YAML format standard.
    /// </summary>
    public class InvalidYamlConfigurationFormatException : SourceReadException
    {
        /// <inheritdoc cref="Exception"/>
        public InvalidYamlConfigurationFormatException()
        {
        }

        /// <inheritdoc cref="Exception"/>
        public InvalidYamlConfigurationFormatException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception"/>
        public InvalidYamlConfigurationFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc cref="Exception"/>
        protected InvalidYamlConfigurationFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}