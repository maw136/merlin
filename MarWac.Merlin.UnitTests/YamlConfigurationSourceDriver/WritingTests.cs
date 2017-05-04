using System.Collections.Generic;
using NUnit.Framework;
using static MarWac.Merlin.UnitTests.Utils.DriverWrapper;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    [TestFixture]
    public class WritingTests
    {
        [Test]
        public void Write_GivenOneParameterWithDefaultValueOnly_WritesCorrectly()
        {
            var configuration = new Configuration(
                new[]
                {
                    new ConfigurationParameter("maxThreads", "5")
                });

            var actual = WriteYaml(configuration);

            var expected = 
@"parameters:
- maxThreads: 5
";

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_GivenOneParameterWithMultipleEnvironmentValues_WritesCorrectly()
        {
            var configuration = new Configuration(
                new[]
                {
                    new ConfigurationParameter("maxThreads", "5", new Dictionary<ConfigurableEnvironment, string>
                    {
                        {new ConfigurableEnvironment("Local"), "15"},
                        {new ConfigurableEnvironment("Test"), "25"}
                    })
                    {
                        Description = "Max number of threads"
                    }
                },
                new[]
                {
                    new ConfigurableEnvironment("Local"),
                    new ConfigurableEnvironment("Test")
                });

            var actual = WriteYaml(configuration);

            var expected =
@"environments:
- Local
- Test
parameters:
- maxThreads:
    description: Max number of threads
    value:
    - Local: 15
    - Test: 25
    - default: 5
";

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}