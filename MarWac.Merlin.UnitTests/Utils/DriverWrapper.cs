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
        public static ConfigSettings ReadYaml(string source) => 
            Read(source, new SourceDrivers.YamlConfigurationSourceDriver());

        public static ConfigSettings ReadExcel(string source) => 
            Read(source, new SourceDrivers.ExcelConfigurationSourceDriver());

        public static string WriteYaml(ConfigSettings configSettings) =>
            Write(configSettings, new SourceDrivers.YamlConfigurationSourceDriver());

        public static string WriteExcel(ConfigSettings configSettings) => 
            Write(configSettings, new SourceDrivers.ExcelConfigurationSourceDriver());

        private static ConfigSettings Read(string source, ConfigurationSourceDriver driver)
        {
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = driver.Read(sourceStream);
            return configuration;
        }

        private static string Write(ConfigSettings configSettings, ConfigurationSourceDriver driver)
        {
            using (var stream = new MemoryStream())
            {
                driver.Write(stream, configSettings);
                stream.Position = 0L;
                var streamReader = new StreamReader(stream, Encoding.UTF8);

                return streamReader.ReadToEnd();
            }
        }
    }
}