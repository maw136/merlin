using System;

namespace MarWac.Merlin
{
    /// <summary>
    /// Environment that can be configured with parameters.
    /// </summary>
    /// <remarks>Name <see cref="Environment"/> is reserved in <see cref="System"/> namespace.</remarks>
    public class ConfigurableEnvironment : IEquatable<ConfigurableEnvironment>
    {
        /// <summary>
        /// Creates an environment with given name.
        /// </summary>
        /// <param name="name">The name of the environment</param>
        public ConfigurableEnvironment(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the environment
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Checks if this instance is equal to the <paramref name="other"/> environment, that is if both environments
        /// have same names.
        /// </summary>
        /// <param name="other">The environment which this instance is compared with</param>
        /// <returns>True if this environment equals the other environment</returns>
        public bool Equals(ConfigurableEnvironment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        /// <inheritdoc cref="object"/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConfigurableEnvironment) obj);
        }

        /// <inheritdoc cref="object"/>
        public override int GetHashCode() => Name.GetHashCode();

        /// <summary>
        /// Performs equality check on two value objects of this class type
        /// </summary>
        /// <param name="left">Left value object to compare</param>
        /// <param name="right">Right value object to compare</param>
        /// <returns>True if two given value objects are equal</returns>
        public static bool operator ==(ConfigurableEnvironment left, ConfigurableEnvironment right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Performs non-equality check on two value objects of this class type
        /// </summary>
        /// <param name="left">Left value object to compare</param>
        /// <param name="right">Right value object to compare</param>
        /// <returns>True if two given value objects are not equal</returns>
        public static bool operator !=(ConfigurableEnvironment left, ConfigurableEnvironment right)
        {
            return !Equals(left, right);
        }
    }
}