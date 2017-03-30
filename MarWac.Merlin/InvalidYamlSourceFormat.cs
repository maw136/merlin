using System;
using System.Runtime.Serialization;

namespace MarWac.Merlin
{
    /// <summary>
    /// Thrown when syntactically valid YAML source contains semantic errors (missing elements, wrong ordering, etc.)
    /// </summary>
    public class InvalidYamlSourceFormat : SourceReadException
    {
        public InvalidYamlSourceFormat()
        {
        }

        public InvalidYamlSourceFormat(string message) : base(message)
        {
        }

        public InvalidYamlSourceFormat(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidYamlSourceFormat(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}