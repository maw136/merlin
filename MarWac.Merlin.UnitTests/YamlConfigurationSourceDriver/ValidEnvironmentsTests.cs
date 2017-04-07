using System.Linq;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    [TestFixture]
    public class ValidEnvironmentsTests
    {
        [Test]
        public void Read_GivenTwoEnvironments_ReadsEnvironmentsCorrectly()
        {
            var configuration = DriverWrapper.Read(@"---
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