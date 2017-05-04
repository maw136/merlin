using System.Linq;
using MarWac.Merlin.UnitTests.Utils;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver.ReadingTests
{
    [TestFixture]
    public class ValidEnvironmentsTests
    {
        [Test]
        public void Read_GivenTwoEnvironments_ReadsEnvironmentsCorrectly()
        {
            var configuration = DriverWrapper.ReadYaml(@"---
                environments:
                    - firstEnv
                    - secondEnv

                parameters:
                    - callTimeoutSeconds:
                        value: 10");

            Assert.That(configuration.Environments.ElementAt(0).Name, Is.EqualTo("firstEnv"));
            Assert.That(configuration.Environments.ElementAt(1).Name, Is.EqualTo("secondEnv"));
        }
    }
}