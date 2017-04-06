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
        private const string ParametersSectionName = "parameters";
        private const string ParameterValuePropertyName = "value";
        private const string ParameterDescriptionPropertyName = "description";
        private const string ParameterDefaultValuePropertyName = "default";
        private const string EnvironmentsSectionName = "environments";

        /// <summary>
        /// Retrieves configuration from a YAML source stream.
        /// </summary>
        /// <param name="source">YAML source stream</param>
        /// <returns>Configuration instance filled with data from the YAML source</returns>
        public Configuration Read(Stream source)
        {
            var configuration = new Configuration();

            YamlMappingNode root = ReadRoot(source);
            BuildUpParameters(root, configuration);
            EnsureNoUnknownSectionProvided(root);

            return configuration;
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

        private static IEnumerable<YamlMappingNode> ReadParametersSequence(YamlMappingNode root)
        {
            YamlNode parametersSectionNode;
            root.Children.TryGetValue(ParametersSectionName, out parametersSectionNode);
            YamlSequenceNode parametersSequence = parametersSectionNode as YamlSequenceNode;

            if (parametersSequence == null)
            {
                throw new InvalidYamlSourceFormatException($"Missing `{ParametersSectionName}` section.");
            }

            return parametersSequence.Children.OfType<YamlMappingNode>();
        }

        private static void BuildUpParameters(YamlMappingNode root, Configuration configuration)
        {
            foreach (YamlMappingNode parameterNode in ReadParametersSequence(root))
            {
                configuration.Parameters.Add(ReadConfigurationParameter(parameterNode));
            }
        }

        private static ConfigurationParameter ReadConfigurationParameter(YamlMappingNode parameter)
        {
            KeyValuePair<YamlNode, YamlNode> parameterAssignment = parameter.Children.First();
            string parameterName = parameterAssignment.Key.ToString();
            YamlNode parameterDefinition = parameterAssignment.Value;

            if (parameterDefinition is YamlScalarNode)
            {
                return ReadSimpleConfigurationParameter(parameterName, parameterDefinition);
            }
            if (parameterDefinition is YamlMappingNode)
            {
                return ReadComplexConfigurationParameter(parameterName, (YamlMappingNode) parameterDefinition);
            }

            throw new InvalidYamlSourceFormatException($"Invalid `{parameterName}` parameter definition.");
        }

        private static ConfigurationParameter ReadSimpleConfigurationParameter(string parameterName,
            YamlNode parameterDefinition)
        {
            return new ConfigurationParameter(parameterName, parameterDefinition.ToString());
        }

        private static ConfigurationParameter ReadComplexConfigurationParameter(string parameterName,
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

            throw new InvalidYamlSourceFormatException($"Invalid value definition for parameter `{parameterName}`.");
        }

        private static ConfigurationParameter ReadMultipleEnvironmentsConfiguredParameter(string parameterName,
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

        private static void MapDefaultAndEnvironmentValues(YamlMappingNode valueNode, 
            IDictionary<ConfigurableEnvironment, string> valueMapping, out string defaultValue)
        {
            defaultValue = null;
            KeyValuePair<YamlNode, YamlNode> valueAssignment = valueNode.Children.First();
            var environmentName = valueAssignment.Key.ToString();
            var value = valueAssignment.Value.ToString();

            if (environmentName == ParameterDefaultValuePropertyName)
            {
                defaultValue = value;
            }
            else
            {
                valueMapping[new ConfigurableEnvironment(environmentName)] = value; // TODO: check if known
            }
        }

        private void EnsureNoUnknownSectionProvided(YamlMappingNode root)
        {
            var firstUnknownSection = root.Children.Keys
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