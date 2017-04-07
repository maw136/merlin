using NUnit.Framework;
using YamlDotNet.Core;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    [TestFixture]
    public class InvalidYamlSemanticsTests
    {
        [Test]
        public void Read_GivenInputWithWrongYamlSyntax_ThrowsSourceReadExceptionWithInnerYamlSemanticErrorException()
        {
            var ex = Assert.Throws<SourceReadException>(() => DriverWrapper.Read(@"---
                parameters:
                  - key
                 - error"));

            Assert.That(ex.Message, Is.EqualTo("Invalid YAML syntax in configuration source provided."));
            Assert.That(ex.InnerException, Is.TypeOf<SemanticErrorException>());
        }

        [Test]
        public void Read_GivenParameterNotUnderParametersSection_ThrowsSourceReadException()
        {
            Assert.Throws<SourceReadException>(() => DriverWrapper.Read(@"
                parameters:
                  - firstParam: 20
                  
                - orphanParam: someValue"));
        }
    }
}