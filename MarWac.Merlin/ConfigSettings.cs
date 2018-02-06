using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MarWac.Merlin
{
    /// <summary>
    /// Represents configuration of some entity/system. Instances once constructed are immutable.
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>
        /// Creates configuration defined with parameters.
        /// </summary>
        /// <param name="parameters">Parameters which constitute configuration</param>
        /// <param name="environments">Optional environments in which parameters may have defined specific values</param>
        public ConfigSettings(IEnumerable<ConfigurationParameter> parameters,
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

            new Validator(Environments, Parameters).Validate();
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
        private class Validator
        {
            private const string DuplicationErrorMessageFormat = "{0} `{1}` cannot occur multiple times.";

            private readonly IEnumerable<ConfigurableEnvironment> _environments;
            private readonly IEnumerable<ConfigurationParameter> _parameters;
            private readonly HashSet<string> _parameterNamesSoFar;
            private readonly ISet<ConfigurableEnvironment> _environmentsSoFar;

            public Validator(IEnumerable<ConfigurableEnvironment> environments,
                IEnumerable<ConfigurationParameter> parameters)
            {
                _environments = environments;
                _parameters = parameters;
                _environmentsSoFar = new HashSet<ConfigurableEnvironment>();
                _parameterNamesSoFar = new HashSet<string>();
            }

            public void Validate()
            {
                ValidateDuplicates(_environmentsSoFar, _environments,
                     env => string.Format(CultureInfo.InvariantCulture, DuplicationErrorMessageFormat, "Environment", env.Name));

                ValidateDuplicates(_parameterNamesSoFar, _parameters.Select(p => p.Name),
                     paramName => string.Format(CultureInfo.InvariantCulture, DuplicationErrorMessageFormat, "Parameter", paramName));

                ValidateIfParametersConfiguredForKnownEnvironments();
            }

            private void ValidateIfParametersConfiguredForKnownEnvironments()
            {
                foreach (var configurationParameter in _parameters)
                {
                    foreach (var parameterEnvironment in configurationParameter.Values.Keys)
                    {
                        if (!_environmentsSoFar.Contains(parameterEnvironment))
                        {
                            throw new InvalidConfigurationException(
                                $"Unknown environment `{parameterEnvironment.Name}` for which parameter " +
                                $"`{configurationParameter.Name}` is configured.");
                        }
                    }
                }
            }

            private static void ValidateDuplicates<T>(
                ISet<T> itemsSoFar, IEnumerable<T> items, Func<T, string> errorMessageGenerator)
                where T : IEquatable<T>
            {
                foreach (var item in items)
                {
                    if (itemsSoFar.Contains(item))
                    {
                        throw new InvalidConfigurationException(errorMessageGenerator(item));
                    }
                    itemsSoFar.Add(item);
                }
            }
        }
    }
}