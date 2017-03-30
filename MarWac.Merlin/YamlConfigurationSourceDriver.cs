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
            YamlNode parametersNode;
            root.Children.TryGetValue(new YamlScalarNode(ParametersSectionName), out parametersNode);
            YamlSequenceNode parametersSequence = parametersNode as YamlSequenceNode;

            if (parametersSequence == null)
            {
                throw new InvalidYamlSourceFormatException("Missing `parameters` section.");
            }

            return parametersSequence.Children.OfType<YamlMappingNode>();
        }

        private static void BuildUpParameters(YamlMappingNode root, Configuration configuration)
        {
            IEnumerable<YamlMappingNode> parameters = ReadParametersSequence(root);

            if (parameters == null)
            {
                return;
            }

            foreach (YamlMappingNode parameter in parameters)
            {
                KeyValuePair<YamlNode, YamlNode> parameterAssignment = parameter.Children.First();
                string parameterName = parameterAssignment.Key.ToString();
                string parameterValue = parameterAssignment.Value.ToString();

                configuration.Parameters.Add(new ConfigurationParameter(parameterName, parameterValue));
            }
        }

        private void EnsureNoUnknownSectionProvided(YamlMappingNode root)
        {
            var firstUnknownSection = root.Children.Keys
                                          .OfType<YamlScalarNode>()
                                          .Where(x => x.Value != ParametersSectionName)
                                          .Select(x => x.Value)
                                          .FirstOrDefault();
            if (firstUnknownSection != null)
            {
                throw new InvalidYamlSourceFormatException($"Unknown section `{firstUnknownSection}`.");
            }
        }
    }
}