using System;

namespace MarWac.Merlin
{
    public class ConfigurationParameter
    {
        public string Name { get; }

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