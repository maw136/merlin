using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// Retrieves/stores configuration from/to YAML sources.
    /// </summary>
    public class YamlConfigurationSourceDriver : ConfigurationSourceDriver
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
        /// <exception cref="SourceReadException">Thrown if the source breaks YAML syntax.</exception>
        /// <exception cref="InvalidYamlConfigurationFormatException">Thrown if YAML source does not contain expected
        /// content format.</exception>
        public override Configuration Read(Stream source)
        {
            return new Reader(source).Read();
        }

        /// <summary>
        /// Stores the configuration to the YAML output stream.
        /// </summary>
        /// <param name="output">An output stream to store the configuration</param>
        /// <param name="configuration">Configuration instance to be stored to the stream</param>
        public override void Write(Stream output, Configuration configuration)
        {
            new Writer(configuration).Write(output);
        }

        private class Reader
        {
            private readonly YamlMappingNode _root;
            private readonly ISet<ConfigurableEnvironment> _environmentsSet;
            private readonly IList<ConfigurationParameter> _parametersList;

            public Reader(Stream source)
            {
                _root = ReadRoot(source);
                _environmentsSet = new HashSet<ConfigurableEnvironment>();
                _parametersList = new List<ConfigurationParameter>();
            }

            public Configuration Read()
            {
                BuildUpEnvironments();
                BuildUpParameters();
                EnsureNoUnknownSectionProvided();

                return new Configuration(_parametersList, _environmentsSet);
            }

            private static YamlMappingNode ReadRoot(Stream source)
            {
                var yamlDoc = LoadYamlFromSource(source).Documents?.FirstOrDefault();
                if (yamlDoc == null)
                {
                    throw new InvalidYamlConfigurationFormatException("Empty YAML source. Cannot read configuration.");
                }

                var rootAsMapping = yamlDoc.RootNode as YamlMappingNode;
                if (rootAsMapping == null)
                {
                    throw new InvalidYamlConfigurationFormatException("No valid section provided.");
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
                    throw new InvalidYamlConfigurationFormatException($"Missing `{ParametersSectionName}` section.");
                }

                return parametersSequence.Children.OfType<YamlMappingNode>();
            }

            private void BuildUpEnvironments()
            {
                foreach (YamlScalarNode environmentNode in ReadEnvironmentsSequence())
                {
                    var configurableEnvironment = new ConfigurableEnvironment(environmentNode.Value);

                    if (configurableEnvironment.Name == ParameterDefaultValuePropertyName)
                    {
                        throw new InvalidYamlConfigurationFormatException(
                            $"`{ParameterDefaultValuePropertyName}` name is prohibited for environment name.");
                    }

                    _environmentsSet.Add(configurableEnvironment);
                }
            }

            private void BuildUpParameters()
            {
                foreach (YamlMappingNode parameterNode in ReadParametersSequence())
                {
                    _parametersList.Add(ReadConfigurationParameter(parameterNode));
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

                throw new InvalidYamlConfigurationFormatException($"Invalid `{parameterName}` parameter definition.");
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
                    return ReadMultipleEnvironmentsConfiguredParameter(parameterName, sequenceOfValuesNode,
                        descriptionNode);
                }

                throw new InvalidYamlConfigurationFormatException(
                    $"Invalid value definition for parameter `{parameterName}`.");
            }

            private ConfigurationParameter ReadMultipleEnvironmentsConfiguredParameter(string parameterName,
                YamlSequenceNode valueNodes, YamlNode descriptionNode)
            {
                string defaultValue = null;
                var valueMapping = new Dictionary<ConfigurableEnvironment, string>();

                foreach (var valueNode in valueNodes.OfType<YamlMappingNode>())
                {
                    MapDefaultAndEnvironmentValues(valueNode, valueMapping, out defaultValue);
                }

                return new ConfigurationParameter(parameterName, defaultValue, valueMapping)
                {
                    Description = descriptionNode?.ToString()
                };
            }

            private void MapDefaultAndEnvironmentValues(YamlMappingNode valueNode,
                IDictionary<ConfigurableEnvironment, string> valueMapping, out string defaultValue)
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
                    throw new InvalidYamlConfigurationFormatException($"Unknown section `{firstUnknownSection}`.");
                }
            }
        }

        private class Writer
        {
            private readonly Configuration _configuration;

            public Writer(Configuration configuration)
            {
                _configuration = configuration;
            }

            public void Write(Stream output)
            {
                using (var writer = new StreamWriter(output, Encoding.UTF8, bufferSize: 512, leaveOpen: true))
                {
                    new Serializer().Serialize(writer, new
                    {
                        environments = _configuration.Environments.Any() 
                            ? _configuration.Environments.Select(e => e.Name)
                            : null,
                        parameters = _configuration.Parameters.Select(MapParamToDictionaryForSerialization)
                    });

                    writer.Flush();   
                }
            }

            private Dictionary<string, object> MapParamToDictionaryForSerialization(ConfigurationParameter parameter)
            {
                return new Dictionary<string, object>
                {
                    {parameter.Name, MapParamProperties(parameter)}
                };
            }

            private object MapParamProperties(ConfigurationParameter parameter)
            {
                if (IsParameterDegenerated(parameter))
                {
                    return parameter.DefaultValue;
                }

                var regularMapping = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(parameter.Description))
                {
                    regularMapping.Add("description", parameter.Description);
                }

                regularMapping.Add("value", MapParamValues(parameter));

                return regularMapping;
            }

            private static object MapParamValues(ConfigurationParameter parameter)
            {
                if (!parameter.Values.Any())
                {
                    return parameter.DefaultValue;
                }

                var values = new List<object>();

                foreach (var environment in parameter.Values.Keys.OrderBy(env => env.Name))
                {
                    values.Add(new Dictionary<string, string>
                    {
                        {environment.Name, parameter.Values[environment]}
                    });    
                }

                if (!string.IsNullOrEmpty(parameter.DefaultValue))
                {
                    values.Add(new Dictionary<string, string>
                    {
                        {"default", parameter.DefaultValue}
                    });
                }

                return values;
            }

            private static bool IsParameterDegenerated(ConfigurationParameter parameter)
            {
                return string.IsNullOrEmpty(parameter.Description) && !parameter.Values.Any();
            }
        }
    }
}