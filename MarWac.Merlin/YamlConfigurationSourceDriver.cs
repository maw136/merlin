using System.IO;

namespace MarWac.Merlin
{
    public class YamlConfigurationSourceDriver
    {
        public Configuration Read(Stream source)
        {
            return new Configuration();
        }
    }
}