﻿using System.IO;
using System.Text;
using NUnit.Framework;
using YamlDotNet.Core;

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

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsFirstParamNameCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters[0].Name, Is.EqualTo("callTimeoutSeconds"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsFirstParamValueCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters[0].Value, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsSecondParamNameCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters[1].Name, Is.EqualTo("importLocation"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsSecondParamValueCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters[1].Value, Is.EqualTo("//share/imports/"));
        }

        [Test]
        public void Read_GivenInputWithWrongYamlSyntax_ThrowsSourceReadExceptionWithInnerYamlSemanticErrorException()
        {
            var ex = Assert.Throws<SourceReadException>(() => Read(@"---
                parameters:
                  - key
                 - error"));

            Assert.That(ex.Message, Is.EqualTo("Invalid YAML syntax in configuration source provided."));
            Assert.That(ex.InnerException, Is.TypeOf<SemanticErrorException>());
        }

        [Test]
        public void Read_GivenInputWithNoValidSection_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"---
                  - paramKey"));

            Assert.That(ex.Message, Is.EqualTo("No valid section provided."));
        }

        [Test]
        public void Read_GivenInputWithoutParametersSection_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"
                awkwardSection:
                  - paramKey"));

            Assert.That(ex.Message, Is.EqualTo("Missing `parameters` section."));
        }

        [Test]
        public void Read_GivenParameterNotUnderParametersSection_ThrowsSourceReadException()
        {
            Assert.Throws<SourceReadException>(() => Read(@"
                parameters:
                  - firstParam: 20
                  
                - orphanParam: someValue"));
        }

        [Test]
        public void Read_GivenParameterUnderDifferentSection_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"
                parameters:
                  - firstParam: 20
                
                someOtherSection:
                  - renegadeParam: someValue"));

            Assert.That(ex.Message, Is.EqualTo("Unknown section `someOtherSection`."));
        }

        [Test]
        public void Read_GivenEmptySource_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(string.Empty));

            Assert.That(ex.Message, Is.EqualTo("Empty YAML source. Cannot read configuration."));
        }


        private static Configuration Read(string source)
        {
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = new YamlConfigurationSourceDriver().Read(sourceStream);
            return configuration;
        }
    }
}