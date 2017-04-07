using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin
{
    /// <summary>
    /// Thrown when configuration domain expressed with YAML source is invalid (e.g missing or disorder of expected 
    /// YAML node names/sequences/etc.). In this case YAML source is valid according to the YAML format standard.
    /// </summary>
    public class InvalidYamlConfigurationFormatException : SourceReadException
    {
        public InvalidYamlConfigurationFormatException()
        {
        }

        public InvalidYamlConfigurationFormatException(string message) : base(message)
        {
        }

        public InvalidYamlConfigurationFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidYamlConfigurationFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}