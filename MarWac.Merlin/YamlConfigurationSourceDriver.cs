using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace MarWac.Merlin
{
    /// <summary>
    /// Retrieves/stores configuration from/to YAML sources.
    /// </summary>
    public class YamlConfigurationSourceDriver
    {
        private const string EnvironmentsSectionName = "environments";
        private const string ParametersSectionName = "parameters";
        private const string ParameterValuePropertyName = "value";
        private const string ParameterDescriptionPropertyName = "description";
        private const string ParameterDefaultValuePropertyName = "default";

        /// <summary>
        /// Retrieves configuration from a YAML source stream.
        /// </summary>
        /// <param name="source">YAML source stream</param>
        /// <returns>Configuration instance filled with data from the YAML source</returns>
        public Configuration Read(Stream source)
        {
            return new Reader(source).Read();
        }

        private class Reader
        {
            private readonly YamlMappingNode _root;
            private readonly Configuration _configuration;

            public Reader(Stream source)
            {
                _configuration = new Configuration();
                _root = ReadRoot(source);
            }

            public Configuration Read()
            {
                BuildUpEnvironments();
                BuildUpParameters();
                EnsureNoUnknownSectionProvided();

                return _configuration;
            }

            private static YamlMappingNode ReadRoot(Stream source)
            {
                var yamlDoc = LoadYamlFromSource(source).Documents?.FirstOrDefault();
                if (yamlDoc == null)
                {
                    throw new InvalidYamlSourceFormatException("Empty YAML source. Cannot read configuration.");
                }

                var rootAsMapping = yamlDoc.RootNode as YamlMappingNode;
                if (rootAsMapping == null)
                {
                    throw new InvalidYamlSourceFormatException("No valid section provided.");
                }

                return rootAsMapping;
            }

            private static YamlStream LoadYamlFromSource(Stream source)
            {
                var yaml = new YamlStream();

                using (var reader = new StreamReader(source))
                {
                    try
                    {
                        yaml.Load(reader);
                    }
                    catch (SemanticErrorException ex)
                    {
                        throw new SourceReadException("Invalid YAML syntax in configuration source provided.", ex);
                    }
                }

                return yaml;
            }

            private IEnumerable<YamlScalarNode> ReadEnvironmentsSequence()
            {
                YamlNode environmentsSectionNode;
                _root.Children.TryGetValue(EnvironmentsSectionName, out environmentsSectionNode);
                YamlSequenceNode environmentsSequence = environmentsSectionNode as YamlSequenceNode;

                return environmentsSequence?.Children.OfType<YamlScalarNode>() ?? Enumerable.Empty<YamlScalarNode>();
            }

            private IEnumerable<YamlMappingNode> ReadParametersSequence()
            {
                YamlNode parametersSectionNode;
                _root.Children.TryGetValue(ParametersSectionName, out parametersSectionNode);
                YamlSequenceNode parametersSequence = parametersSectionNode as YamlSequenceNode;

                if (parametersSequence == null)
                {
                    throw new InvalidYamlSourceFormatException($"Missing `{ParametersSectionName}` section.");
                }

                return parametersSequence.Children.OfType<YamlMappingNode>();
            }

            private void BuildUpEnvironments()
            {
                foreach (YamlScalarNode environmentNode in ReadEnvironmentsSequence())
                {
                    var configurableEnvironment = new ConfigurableEnvironment(environmentNode.Value);

                    if (_configuration.Environments.Contains(configurableEnvironment))
                    {
                        throw new InvalidYamlSourceFormatException($"Environment `{configurableEnvironment.Name}` " +
                                                                    "cannot occur multiple times.");
                    }

                    _configuration.Environments.Add(configurableEnvironment);
                }
            }

            private void BuildUpParameters()
            {
                foreach (YamlMappingNode parameterNode in ReadParametersSequence())
                {
                    _configuration.Parameters.Add(ReadConfigurationParameter(parameterNode));
                }
            }

            private ConfigurationParameter ReadConfigurationParameter(YamlMappingNode parameter)
            {
                KeyValuePair<YamlNode, YamlNode> parameterAssignment = parameter.Children.First();
                string parameterName = parameterAssignment.Key.ToString();
                YamlNode parameterDefinition = parameterAssignment.Value;

                if (parameterDefinition is YamlScalarNode)
                {
                    return ReadSimpleConfigurationParameter(parameterName, parameterDefinition);
                }
                var parameterDefinitionMapping = parameterDefinition as YamlMappingNode;
                if (parameterDefinitionMapping != null)
                {
                    return ReadComplexConfigurationParameter(parameterName, parameterDefinitionMapping);
                }

                throw new InvalidYamlSourceFormatException($"Invalid `{parameterName}` parameter definition.");
            }

            private ConfigurationParameter ReadSimpleConfigurationParameter(string parameterName,
                YamlNode parameterDefinition)
            {
                return new ConfigurationParameter(parameterName, parameterDefinition.ToString());
            }

            private ConfigurationParameter ReadComplexConfigurationParameter(string parameterName, 
                YamlMappingNode parameterDefinition)
            {
                YamlNode descriptionNode;
                parameterDefinition.Children.TryGetValue(ParameterDescriptionPropertyName, out descriptionNode);

                YamlNode values;
                parameterDefinition.Children.TryGetValue(ParameterValuePropertyName, out values);

                if (values is YamlScalarNode)
                {
                    return new ConfigurationParameter(parameterName, values.ToString())
                    {
                        Description = descriptionNode?.ToString()
                    };
                }
                var sequenceOfValuesNode = values as YamlSequenceNode;
                if (sequenceOfValuesNode != null)
                {
                    return ReadMultipleEnvironmentsConfiguredParameter(parameterName, sequenceOfValuesNode, descriptionNode);
                }

                throw new InvalidYamlSourceFormatException($"Invalid value definition for parameter `{parameterName}`.");
            }

            private ConfigurationParameter ReadMultipleEnvironmentsConfiguredParameter(string parameterName, 
                YamlSequenceNode valueNodes, YamlNode descriptionNode)
            {
                string defaultValue = null;
                var valueMapping = new Dictionary<ConfigurableEnvironment, string>();

                foreach (var valueNode in valueNodes.OfType<YamlMappingNode>())
                {
                    MapDefaultAndEnvironmentValues(parameterName, valueNode, valueMapping, out defaultValue, 
                        new HashSet<ConfigurableEnvironment>(_configuration.Environments));
                }

                return new ConfigurationParameter(parameterName, defaultValue, valueMapping)
                {
                    Description = descriptionNode?.ToString()
                };
            }

            private void MapDefaultAndEnvironmentValues(string parameterName, YamlMappingNode valueNode, 
                IDictionary<ConfigurableEnvironment, string> valueMapping, out string defaultValue, 
                ISet<ConfigurableEnvironment> definedEnvironments)
            {
                defaultValue = null;
                KeyValuePair<YamlNode, YamlNode> valueAssignment = valueNode.Children.First();
                var environmentName = valueAssignment.Key.ToString();
                var environment = new ConfigurableEnvironment(environmentName);
                var value = valueAssignment.Value.ToString();

                if (environmentName == ParameterDefaultValuePropertyName)
                {
                    defaultValue = value;
                }
                else
                {
                    if (!definedEnvironments.Contains(environment))
                    {
                        throw new InvalidYamlSourceFormatException(
                            $"Unknown environment `{environmentName}` for which parameter `{parameterName}` is " +
                             "configured.");
                    }
                
                    valueMapping[environment] = value;
                }
            }

            private void EnsureNoUnknownSectionProvided()
            {
                var firstUnknownSection = _root.Children.Keys
                    .OfType<YamlScalarNode>()
                    .Where(x => x.Value != ParametersSectionName && x.Value != EnvironmentsSectionName)
                    .Select(x => x.Value)
                    .FirstOrDefault();

                if (firstUnknownSection != null)
                {
                    throw new InvalidYamlSourceFormatException($"Unknown section `{firstUnknownSection}`.");
                }
            }
        }
    }
}