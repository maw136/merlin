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
            const string source = @"---
                parameters:
                    - callTimeoutSeconds: 15";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var driver = new YamlConfigurationSourceDriver();
            var configuration = driver.Read(stream);

            Assert.That(configuration.Parameters[0].Name, Is.EqualTo("callTimeoutSeconds"));
        }
    }
}