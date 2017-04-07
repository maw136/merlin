using System;
using System.Collections.Generic;

namespace MarWac.Merlin
{
    /// <summary>
    /// Represents configuration of some entity/system. Instances once constructed are immutable.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Creates configuration defined with parameters.
        /// </summary>
        /// <param name="parameters">Parameters which constitute configuration</param>
        /// <param name="environments">Optional environments in which parameters may have defined specific values</param>
        public Configuration(IEnumerable<ConfigurationParameter> parameters,
            IEnumerable<ConfigurableEnvironment> environments = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Parameters = new List<ConfigurationParameter>(parameters);
            Environments = environments != null
                ? new List<ConfigurableEnvironment>(environments)
                : new List<ConfigurableEnvironment>();
        }

        /// <summary>
        /// All enviornments defined in the configuration
        /// </summary>
        public IReadOnlyCollection<ConfigurableEnvironment> Environments { get; }

        /// <summary>
        /// All parameters defined in the configuration
        /// </summary>
        public IReadOnlyCollection<ConfigurationParameter> Parameters { get; }
    }
}