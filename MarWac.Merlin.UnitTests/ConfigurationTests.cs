using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void Ctor_GivenDoubledEnvironments_Throws()
        {
            var ex = Assert.Throws<InvalidConfigurationException>(() => new Configuration(
                new[]
                {
                    new ConfigurationParameter("threadLimit", "20")
                },
                new[]
                {
                    new ConfigurableEnvironment("local"),
                    new ConfigurableEnvironment("local")
                }));

            Assert.That(ex.Message, Is.EqualTo("Environment `local` cannot occur multiple times."));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void Ctor_GivenDoubledParameters_Throws()
        {
            var ex = Assert.Throws<InvalidConfigurationException>(() => new Configuration(
                new[]
                {
                    new ConfigurationParameter("threadLimit", "20"),
                    new ConfigurationParameter("threadLimit", "10")
                }));

            Assert.That(ex.Message, Is.EqualTo("Parameter `threadLimit` cannot occur multiple times."));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void Ctor_GivenParameterValueDefinedForUnknownEnvironment_Throws()
        {
            var ex = Assert.Throws<InvalidConfigurationException>(() => new Configuration(
                new[]
                {
                    new ConfigurationParameter("callTimeoutSeconds", null,
                        new Dictionary<ConfigurableEnvironment, string>
                        {
                            { new ConfigurableEnvironment("test"), "10" }
                        })
                },
                new[]
                {
                    new ConfigurableEnvironment("local")
                }));

            Assert.That(ex.Message, Is.EqualTo(
                "Unknown environment `test` for which parameter `callTimeoutSeconds` is configured."));
        }
    }
}