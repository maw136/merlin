using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests
{
    [TestFixture]
    public class ConfigurationParameterTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Please analyser.")]
        public void Ctor_GivenBothDefaultValueAndValueMappingEmpty_ThrowsArgumentException()
        {
            var configurationParameter = new ConfigurationParameter("threadLimit", "");

            Assert.That(configurationParameter.Name, Is.EqualTo("threadLimit"));
            Assert.That(configurationParameter.Description, Is.Null);
            Assert.That(configurationParameter.DefaultValue, Is.Empty);
        }
    }
}