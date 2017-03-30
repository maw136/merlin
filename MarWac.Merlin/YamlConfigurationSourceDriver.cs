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
            IEnumerable<YamlMappingNode> parameters = ReadParametersSequence(root);
            FillConfigurationWithParameters(configuration, parameters);
            EnsureNoUnknownSectionProvided(root);

            return configuration;
        }

        private void EnsureNoUnknownSectionProvided(YamlMappingNode root)
        {
            var firstUnknownSection = root?.Children.Keys
                                           .OfType<YamlScalarNode>()
                                           .Where(x => x.Value != ParametersSectionName)
                                           .Select(x => x.Value)
                                           .FirstOrDefault();
            if (firstUnknownSection != null)
            {
                throw new InvalidYamlSourceFormat($"Unknown section `{firstUnknownSection}`.");
            }
        }

        private static YamlMappingNode ReadRoot(Stream source)
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

            var root = yaml.Documents?.FirstOrDefault()?.RootNode as YamlMappingNode;

            return root;
        }

        private static IEnumerable<YamlMappingNode> ReadParametersSequence(YamlMappingNode root)
        {
            var parametersNode = root?.Children[new YamlScalarNode(ParametersSectionName)] as YamlSequenceNode;

            if (parametersNode == null)
            {
                throw new InvalidYamlSourceFormat("Missing `parameters` node.");
            }

            return parametersNode.Children.OfType<YamlMappingNode>();
        }

        private static void FillConfigurationWithParameters(Configuration configuration,
            IEnumerable<YamlMappingNode> parameters)
        {
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
    }
}