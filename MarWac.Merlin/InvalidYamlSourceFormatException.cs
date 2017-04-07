using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin
{
    /// <summary>
    /// Thrown when syntactically valid YAML source contains semantic errors (missing elements, wrong ordering, etc.)
    /// </summary
    public class InvalidYamlSourceFormatException : Exception
    {
        public InvalidYamlSourceFormatException()
        {
        }

        public InvalidYamlSourceFormatException(string message) : base(message)
        {
        }

        public InvalidYamlSourceFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidYamlSourceFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}