using MarWac.Merlin.UnitTests.Utils;
using NUnit.Framework;
using YamlDotNet.Core;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver.ReadingTests
{
    [TestFixture]
    public class InvalidYamlStandardFormatTests
    {
        [Test]
        public void Read_GivenInputWithWrongYamlSyntax_ThrowsSourceReadExceptionWithInnerYamlSemanticErrorException()
        {
            var ex = Assert.Throws<SourceReadException>(() => DriverWrapper.ReadYaml(@"---
                parameters:
                  - key
                 - error"));

            Assert.That(ex.Message, Is.EqualTo("Invalid YAML syntax in configuration source provided."));
            Assert.That(ex.InnerException, Is.TypeOf<SemanticErrorException>());
        }

        [Test]
        public void Read_GivenParameterNotUnderParametersSection_Throws()
        {
            Assert.Throws<SourceReadException>(() => DriverWrapper.ReadYaml(@"
                parameters:
                  - firstParam: 20
                  
                - orphanParam: someValue"));
        }
    }
}