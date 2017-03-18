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

            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = new YamlConfigurationSourceDriver().Read(sourceStream);

            Assert.That(configuration.Parameters[0].Name, Is.EqualTo("callTimeoutSeconds"));
        }
    }
}