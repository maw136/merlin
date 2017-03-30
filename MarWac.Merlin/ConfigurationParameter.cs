using System;

namespace MarWac.Merlin
{
    /// <summary>
    /// A configuration building block.
    /// </summary>
    public class ConfigurationParameter
    {
        /// <summary>
        /// The name of the configuration parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the configuration parameter
        /// </summary>
        public string Value { get; set; }

        public ConfigurationParameter(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }

            Name = name;
            Value = value;
        }
    }
}