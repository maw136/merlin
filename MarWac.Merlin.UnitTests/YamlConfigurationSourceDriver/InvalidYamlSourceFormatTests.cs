using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    [TestFixture]
    public class InvalidYamlSourceFormatTests
    {
        [Test]
        public void Read_GivenInputWithNoValidSection_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                  - paramKey"));

            Assert.That(ex.Message, Is.EqualTo("No valid section provided."));
        }

        [Test]
        public void Read_GivenInputWithoutParametersSection_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"
                awkwardSection:
                  - paramKey"));

            Assert.That(ex.Message, Is.EqualTo("Missing `parameters` section."));
        }

        [Test]
        public void Read_GivenParameterUnderDifferentSection_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"
                parameters:
                  - firstParam: 20
                
                someOtherSection:
                  - renegadeParam: someValue"));

            Assert.That(ex.Message, Is.EqualTo("Unknown section `someOtherSection`."));
        }

        [Test]
        public void Read_GivenEmptySource_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(string.Empty));

            Assert.That(ex.Message, Is.EqualTo("Empty YAML source. Cannot read configuration."));
        }

        [Test]
        public void Read_GivenParameterWithMalformedValueNode_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        value:
                          default: 15"));

            Assert.That(ex.Message, Is.EqualTo("Invalid value definition for parameter `callTimeoutSeconds`."));
        }

        [Test]
        public void Read_GivenParameterWithMalformedProperties_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        - value: 15"));

            Assert.That(ex.Message, Is.EqualTo("Invalid `callTimeoutSeconds` parameter definition."));
        }

        [Test]
        public void Read_GivenParameterValueDefinedForUnknownEnvironment_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                environments:
                    - local

                parameters:
                    - callTimeoutSeconds:
                        value: 
                            - test: 10"));

            Assert.That(ex.Message, Is.EqualTo(
                "Unknown environment `test` for which parameter `callTimeoutSeconds` is configured."));
        }

        [Test]
        public void Read_GivenDoubledEnvironments_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                environments:
                    - local
                    - local

                parameters:
                    - threadLimit: 20"));

            Assert.That(ex.Message, Is.EqualTo("Environment `local` cannot occur multiple times."));
        }

        [Test]
        public void Read_GivenDoubledParameters_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                parameters:
                    - threadLimit: 20
                    - threadLimit: 10"));

            Assert.That(ex.Message, Is.EqualTo("Parameter `threadLimit` cannot occur multiple times."));
        }

        [Test]
        public void Read_GivenEnvironmentNamedDefault_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => DriverWrapper.Read(@"---
                environments:
                    - default
                
                parameters:
                    - threadLimit: 10"));

            Assert.That(ex.Message, Is.EqualTo("`default` name is prohibited for environment name."));
        }
    }
}