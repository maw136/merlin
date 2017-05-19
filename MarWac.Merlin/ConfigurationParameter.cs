using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        /// The default value of the configuration parameter
        /// </summary>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// The description of the configuration parameter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Represents values assignment per environments
        /// </summary>
        public IReadOnlyDictionary<ConfigurableEnvironment, string> Values { get; }

        /// <summary>
        /// Answers the question whether a given value is unknown (not defined, no value, not applicable, etc.)
        /// </summary>
        /// <param name="value">The value to test whether is known or not</param>
        /// <returns><c>true</c> if the value is unknown.</returns>
        public static bool IsValueUnknown(string value) => string.IsNullOrEmpty(value);

        public ConfigurationParameter(string name, string defaultValue,
            IDictionary<ConfigurableEnvironment, string> values = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            DefaultValue = defaultValue;
            Values = new ReadOnlyDictionary<ConfigurableEnvironment, string>(
                values ?? new Dictionary<ConfigurableEnvironment, string>());
        }
    }
}