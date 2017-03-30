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

        public string Value { get; set; }

        public ConfigurationParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
        }
    }
}