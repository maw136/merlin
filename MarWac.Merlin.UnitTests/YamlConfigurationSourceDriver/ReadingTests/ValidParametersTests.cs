using System.Linq;
using MarWac.Merlin.UnitTests.Utils;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver.ReadingTests
{
    [TestFixture]
    public class ValidParametersTests
    {
        [Test]
        public void Read_GivenSingleParamUnderParametersNode_ReadsParameterNameCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds: 15");

            Assert.That(configuration.Parameters.ElementAt(0).Name, Is.EqualTo("callTimeoutSeconds"));
        }

        [Test]
        public void Read_GivenSingleParamUnderParametersNode_ReadsParameterValueCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds: 15");

            Assert.That(configuration.Parameters.ElementAt(0).DefaultValue, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsFirstParamNameCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters.ElementAt(0).Name, Is.EqualTo("callTimeoutSeconds"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsFirstParamValueCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters.ElementAt(0).DefaultValue, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsSecondParamNameCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters.ElementAt(1).Name, Is.EqualTo("importLocation"));
        }

        [Test]
        public void Read_GivenTwoParamsUnderParametersNode_ReadsSecondParamValueCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds: 15
                    - importLocation: //share/imports/");

            Assert.That(configuration.Parameters.ElementAt(1).DefaultValue, Is.EqualTo("//share/imports/"));
        }

        [Test]
        public void Read_GivenParameterWithDescription_ReadsParameterNameCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds:
                        description: How long the system waits for the response to come
                        value: 15");

            Assert.That(configuration.Parameters.ElementAt(0).Name, Is.EqualTo("callTimeoutSeconds"));
        }

        [Test]
        public void Read_GivenParameterWithDescription_ReadsParameterValueCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds:
                        description: How long the system waits for the response to come
                        value: 15");

            Assert.That(configuration.Parameters.ElementAt(0).DefaultValue, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenParameterWithDescription_ReadsParameterDescriptionCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                parameters:
                    - callTimeoutSeconds:
                        description: How long the system waits for the response to come
                        value: 15");

            Assert.That(configuration.Parameters.ElementAt(0).Description, Is.EqualTo(
                "How long the system waits for the response to come"));
        }

        [Test]
        public void Read_GivenParameterWithDistinctValuesPerEnvironments_ReadsParameterAssigningValuesPerEnvironments()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                environments:
                    - dev
                    - test

                parameters:
                    - callTimeoutSeconds: 
                        value: 
                            - dev: 20
                            - test: 30");

            Assert.That(configuration.Parameters.ElementAt(0).Values[new ConfigurableEnvironment("dev")],
                Is.EqualTo("20"));
            Assert.That(configuration.Parameters.ElementAt(0).Values[new ConfigurableEnvironment("test")],
                Is.EqualTo("30"));
        }

        [Test]
        public void Read_GivenParameterWithSpecificValuePerEnvironment_ReadsDefaultValueNull()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                environments:
                    - local

                parameters:
                    - callTimeoutSeconds: 
                        value: 
                            - local: 15");

            Assert.That(configuration.Parameters.ElementAt(0).DefaultValue, Is.Null);
        }

        [Test]
        public void Read_GivenParameterWithSameValuePerEnvironments_ReadsParameterDefaultValueOnly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                environments:
                    - dev
                    - test

                parameters:
                    - callTimeoutSeconds: 15");

            Assert.That(configuration.Parameters.ElementAt(0).Values.ContainsKey(new ConfigurableEnvironment("dev")),
                Is.False);
            Assert.That(configuration.Parameters.ElementAt(0).Values.ContainsKey(new ConfigurableEnvironment("test")),
                Is.False);
            Assert.That(configuration.Parameters.ElementAt(0).DefaultValue, Is.EqualTo("15"));
        }

        [Test]
        public void Read_GivenParameterWithDefaultValueAlongWithEnvironments_ReadsParameterDefaultValueAlso()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                environments:
                    - dev
                    - test

                parameters:
                    - callTimeoutSeconds:
                        value:
                          - dev: 50
                          - default: 10");

            Assert.That(configuration.Parameters.ElementAt(0).DefaultValue, Is.EqualTo("10"));
        }
    }
}