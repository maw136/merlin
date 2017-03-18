using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace MarWac.Merlin
{
    public class YamlConfigurationSourceDriver
    {
        public Configuration Read(Stream source)
        {
            var configuration = new Configuration();
            
            using (var reader = new StreamReader(source))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                var root = yaml.Documents?.FirstOrDefault()?.RootNode as YamlMappingNode;
                var parameters = (root?.Children[new YamlScalarNode("parameters")] as YamlSequenceNode)?
                                       .Children.OfType<YamlMappingNode>();

                if (parameters != null)
                {
                    foreach (YamlMappingNode parameter in parameters)
                    {
                        KeyValuePair<YamlNode, YamlNode> parameterAssignment = parameter.Children.First();
                        string parameterName = parameterAssignment.Key.ToString();

                        configuration.Parameters.Add(new ConfigurationParameter(parameterName));
                    }
                }
            }

            return configuration;
        }
    }
}