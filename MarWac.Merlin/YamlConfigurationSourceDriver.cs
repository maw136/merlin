using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace MarWac.Merlin
{
    /// <summary>
    /// Retrieves/stores configuration from/to YAML sources.
    /// </summary>
    public class YamlConfigurationSourceDriver
    {
        /// <summary>
        /// Retrieves configuration from a YAML source stream.
        /// </summary>
        /// <param name="source">YAML source stream</param>
        /// <returns>Configuration instance filled with data from the YAML source</returns>
        public Configuration Read(Stream source)
        {
            var configuration = new Configuration();

            using (var reader = new StreamReader(source))
            {
                YamlMappingNode root = ReadRoot(reader);
                IEnumerable<YamlMappingNode> parameters = ReadParametersSequence(root);
                FillConfigurationWithParameters(configuration, parameters);
            }

            return configuration;
        }

        private static YamlMappingNode ReadRoot(TextReader reader)
        {
            var yaml = new YamlStream();
            yaml.Load(reader);

            var root = yaml.Documents?.FirstOrDefault()?.RootNode as YamlMappingNode;

            return root;
        }

        private static IEnumerable<YamlMappingNode> ReadParametersSequence(YamlMappingNode root)
        {
            var parametersNode = root?.Children[new YamlScalarNode("parameters")] as YamlSequenceNode;
            var parametersNodeChildrenAsMappings = parametersNode?.Children.OfType<YamlMappingNode>();

            return parametersNodeChildrenAsMappings;
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