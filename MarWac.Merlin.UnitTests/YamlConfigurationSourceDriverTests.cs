using System.IO;
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

            Assert.That(configuration.Parameters[0].DefaultValue, Is.EqualTo("15"));
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

            Assert.That(configuration.Parameters[0].DefaultValue, Is.EqualTo("15"));
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

            Assert.That(configuration.Parameters[1].DefaultValue, Is.EqualTo("//share/imports/"));
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

        [Test]
        public void Read_GivenParameterWithDescription_ReadsParameterNameCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        description: How long the system waits for the response to come
                        value: 15");

            Assert.That(configuration.Parameters[0].Name, Is.EqualTo("callTimeoutSeconds"));
        }

        [Test]
        public void Read_GivenParameterWithDescription_ReadsParameterValueCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        description: How long the system waits for the response to come
                        value: 15");

            Assert.That(configuration.Parameters[0].DefaultValue, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenParameterWithDescription_ReadsParameterDescriptionCorrectly()
        {
            var configuration = Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        description: How long the system waits for the response to come
                        value: 15");

            Assert.That(configuration.Parameters[0].Description, Is.EqualTo("How long the system waits " +
                                                                            "for the response to come"));
        }

        [Test]
        public void Read_GivenParameterWithDistinctValuesPerEnvironments_ReadsParameterAssigningValuesPerEnvironments()
        {
            var configuration = Read(@"---
                environments:
                    - dev
                    - test

                parameters:
                    - callTimeoutSeconds: 
                        value: 
                            - dev: 20
                            - test: 30");

            Assert.That(configuration.Parameters[0].Values[new ConfigurableEnvironment("dev")], Is.EqualTo("20"));
            Assert.That(configuration.Parameters[0].Values[new ConfigurableEnvironment("test")], Is.EqualTo("30"));
        }

        [Test]
        public void Read_GivenParameterWithSpecificValuePerEnvironment_ReadsDefaultValueNull()
        {
            var configuration = Read(@"---
                environments:
                    - local

                parameters:
                    - callTimeoutSeconds: 
                        value: 
                            - local: 15");

            Assert.That(configuration.Parameters[0].DefaultValue, Is.Null);
        }

        [Test]
        public void Read_GivenParameterWithSameValuePerEnvironments_ReadsParameterDefaultValueOnly()
        {
            var configuration = Read(@"---
                environments:
                    - dev
                    - test

                parameters:
                    - callTimeoutSeconds: 15");

            Assert.That(configuration.Parameters[0].Values.ContainsKey(new ConfigurableEnvironment("dev")), Is.False);
            Assert.That(configuration.Parameters[0].Values.ContainsKey(new ConfigurableEnvironment("test")), Is.False);
            Assert.That(configuration.Parameters[0].DefaultValue, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenParameterWithDefaultValueAlongWithEnvironments_ReadsParameterDefaultValueAlso()
        {
            var configuration = Read(@"---
                environments:
                    - dev
                    - test

                parameters:
                    - callTimeoutSeconds:
                        value:
                          - dev: 50
                          - default: 10");

            Assert.That(configuration.Parameters[0].DefaultValue, Is.EqualTo("10"));
        }

        [Test]
        public void Read_GivenParameterWithMalformedValueNode_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        value:
                          default: 15"));

            Assert.That(ex.Message, Is.EqualTo("Invalid value definition for parameter `callTimeoutSeconds`."));
        }

        [Test]
        public void Read_GivenParameterWithMalformedProperties_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"---
                parameters:
                    - callTimeoutSeconds:
                        - value: 15"));

            Assert.That(ex.Message, Is.EqualTo("Invalid `callTimeoutSeconds` parameter definition."));
        }

        [Test]
        public void Read_GivenParameterValueDefinedForUnknownEnvironment_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"---
                environments:
                    - local

                parameters:
                    - callTimeoutSeconds:
                        value: 
                            - test: 10"));

            Assert.That(ex.Message, Is.EqualTo("Unknown environment `test` for which parameter `callTimeoutSeconds` " +
                                               "is configured."));
        }

        [Test]
        public void Read_GivenTwoEnvironments_ReadsEnvironmentsCorrectly()
        {
            var configuration = Read(@"---
                environments:
                    - firstEnv
                    - secondEnv

                parameters:
                    - callTimeoutSeconds:
                        value: 10");

            Assert.That(configuration.Environments[0].Name, Is.EqualTo("firstEnv"));
            Assert.That(configuration.Environments[1].Name, Is.EqualTo("secondEnv"));
        }

        [Test]
        public void Read_GivenDoubledEnvironments_ThrowsInvalidYamlSourceFormatException()
        {
            var ex = Assert.Throws<InvalidYamlSourceFormatException>(() => Read(@"---
                environments:
                    - local
                    - local

                parameters:
                    - threadLimit: 20"));

            Assert.That(ex.Message, Is.EqualTo("Environment `local` cannot occur multiple times."));
        }

        private static Configuration Read(string source)
        {
            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(source));
            var configuration = new YamlConfigurationSourceDriver().Read(sourceStream);
            return configuration;
        }
    }
}