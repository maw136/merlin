﻿using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    [TestFixture]
    public class InvalidYamlConfigurationFormatTests
    {
        [Test]
        public void Read_GivenInputWithNoValidSection_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(@"---
                  - paramKey"));

            Assert.That(ex.Message, Is.EqualTo("No valid section provided."));
        }

        [Test]
        public void Read_GivenInputWithoutParametersSection_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(@"
                awkwardSection:
                  - paramKey"));

            Assert.That(ex.Message, Is.EqualTo("Missing `parameters` section."));
        }

        [Test]
        public void Read_GivenParameterUnderDifferentSection_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(@"
                parameters:
                  - firstParam: 20
                
                someOtherSection:
                  - renegadeParam: someValue"));

            Assert.That(ex.Message, Is.EqualTo("Unknown section `someOtherSection`."));
        }

        [Test]
        public void Read_GivenEmptySource_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(string.Empty));

            Assert.That(ex.Message, Is.EqualTo("Empty YAML source. Cannot read configuration."));
        }

        [Test]
        public void Read_GivenParameterWithMalformedValueNode_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        value:
                          default: 15"));

            Assert.That(ex.Message, Is.EqualTo("Invalid value definition for parameter `callTimeoutSeconds`."));
        }

        [Test]
        public void Read_GivenParameterWithMalformedProperties_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        - value: 15"));

            Assert.That(ex.Message, Is.EqualTo("Invalid `callTimeoutSeconds` parameter definition."));
        }

        [Test]
        public void Read_GivenEnvironmentNamedDefault_Throws()
        {
            var ex = Assert.Throws<InvalidYamlConfigurationFormatException>(() => DriverWrapper.Read(@"---
                environments:
                    - default
                
                parameters:
                    - threadLimit: 10"));

            Assert.That(ex.Message, Is.EqualTo("`default` name is prohibited for environment name."));
        }
    }
}