using System;
using System.Collections.Generic;
using System.Linq;

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

            Validate();
        }

        /// <summary>
        /// All enviornments defined in the configuration
        /// </summary>
        public IReadOnlyCollection<ConfigurableEnvironment> Environments { get; }

        /// <summary>
        /// All parameters defined in the configuration
        /// </summary>
        public IReadOnlyCollection<ConfigurationParameter> Parameters { get; }

        /// <summary>
        /// Throws if there is any configuration issue. Ensures that the configuration instance built is valid.
        /// </summary>
        private void Validate()
        {
            var environmentsSoFar = new HashSet<ConfigurableEnvironment>();
            var parameterNamesSoFar = new HashSet<string>();

            foreach (var environment in Environments)
            {
                if (environmentsSoFar.Contains(environment))
                {
                    throw new InvalidConfigurationException(
                        $"Environment `{environment.Name}` cannot occur multiple times.");
                }
                environmentsSoFar.Add(environment);
            }

            foreach (var parameter in Parameters)
            {
                if (parameterNamesSoFar.Contains(parameter.Name))
                {
                    throw new InvalidConfigurationException(
                        $"Parameter `{parameter.Name}` cannot occur multiple times.");
                }
                parameterNamesSoFar.Add(parameter.Name);

                foreach (var parameterEnvironment in parameter.Values.Keys)
                {
                    if (!environmentsSoFar.Contains(parameterEnvironment))
                    {
                        throw new InvalidConfigurationException(
                            $"Unknown environment `{parameterEnvironment.Name}` for which parameter " +
                            $"`{parameter.Name}` is configured.");
                    }
                }
            }
        }
    }
}