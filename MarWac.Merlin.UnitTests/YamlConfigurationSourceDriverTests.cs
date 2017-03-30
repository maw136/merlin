using System.IO;
using System.Text;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests
{
    [TestFixture]
    public class YamlConfigurationSourceDriverTests
    {
        [Test]
        public void Read_GivenSingleParamUnderParametersNode_ReadsParameterNameCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds: 15");

            Assert.That(configuration.Parameters[0].Name, Is.EqualTo("callTimeoutSeconds"));
        }

        [Test]
        public void Read_GivenSingleParamUnderParametersNode_ReadsParameterValueCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds: 15");

            Assert.That(configuration.Parameters[0].Value, Is.EqualTo("15"));
        }

        private static Configuration Read(string source)
        {
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = new YamlConfigurationSourceDriver().Read(sourceStream);
            return configuration;
        }
    }
}