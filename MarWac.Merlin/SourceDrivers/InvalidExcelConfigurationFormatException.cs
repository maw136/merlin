using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// Represents errors that occur during Excel XML source reading.
    /// </summary>
    [Serializable]
    public class InvalidExcelConfigurationFormatException : Exception
    {
        /// <inheritdoc cref="Exception"/>
        public InvalidExcelConfigurationFormatException()
        {
        }

        /// <inheritdoc cref="Exception"/>
        public InvalidExcelConfigurationFormatException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception"/>
        public InvalidExcelConfigurationFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc cref="Exception"/>
        protected InvalidExcelConfigurationFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}