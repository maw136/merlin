using System.IO;
using System.Text;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    internal static class DriverWrapper
    {
        public static Configuration Read(string source)
        {
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = new Merlin.YamlConfigurationSourceDriver().Read(sourceStream);
            return configuration;
        }
    }
}