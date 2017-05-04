using System.IO;
using System.Text;
using MarWac.Merlin.SourceDrivers;

namespace MarWac.Merlin.UnitTests.Utils
{
    /// <summary>
    /// A facade over various drivers employed in unit tests as subjects under test.
    /// </summary>
    internal static class DriverWrapper
    {
        public static Configuration ReadYaml(string source) => 
            Read(source, new SourceDrivers.YamlConfigurationSourceDriver());

        public static Configuration ReadExcel(string source) => 
            Read(source, new SourceDrivers.ExcelConfigurationSourceDriver());

        public static string WriteYaml(Configuration configuration) =>
            Write(configuration, new SourceDrivers.YamlConfigurationSourceDriver());

        public static string WriteExcel(Configuration configuration) => 
            Write(configuration, new SourceDrivers.ExcelConfigurationSourceDriver());

        private static Configuration Read(string source, ConfigurationSourceDriver driver)
        {
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = driver.Read(sourceStream);
            return configuration;
        }

        private static string Write(Configuration configuration, ConfigurationSourceDriver driver)
        {
            using (var stream = new MemoryStream())
            {
                driver.Write(stream, configuration);
                stream.Position = 0L;
                var streamReader = new StreamReader(stream, Encoding.UTF8);

                return streamReader.ReadToEnd();
            }
        }
    }
}