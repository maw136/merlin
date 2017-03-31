using System;

namespace MarWac.Merlin
{
    /// <summary>
    /// Environment that can be configured with parameters.
    /// </summary>
    /// <remarks>Name <see cref="Environment"/> is reserved in <see cref="System"/> namespace.</remarks>
    public class ConfigurableEnvironment : IEquatable<ConfigurableEnvironment>
    {
        public ConfigurableEnvironment(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the environment
        /// </summary>
        public string Name { get; }

        public bool Equals(ConfigurableEnvironment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConfigurableEnvironment) obj);
        }

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(ConfigurableEnvironment left, ConfigurableEnvironment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConfigurableEnvironment left, ConfigurableEnvironment right)
        {
            return !Equals(left, right);
        }
    }
}