using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin.SourceDrivers
{
    public class InvalidExcelConfigurationFormatException : Exception
    {
        public InvalidExcelConfigurationFormatException()
        {
        }

        public InvalidExcelConfigurationFormatException(string message) : base(message)
        {
        }

        public InvalidExcelConfigurationFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidExcelConfigurationFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}