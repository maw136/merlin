using System;

namespace MarWac.Merlin
{
    /// <summary>
    /// Environment that can be configured with parameters.
    /// </summary>
    /// <remarks>Name <see cref="Environment"/> is reserved in <see cref="System"/> namespace.</remarks>
    public class ConfigurableEnvironment
    {
        public ConfigurableEnvironment(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the environment
        /// </summary>
        public string Name { get; }
    }
}