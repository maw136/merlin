using NUnit.Framework;
using static MarWac.Merlin.UnitTests.Utils.DriverWrapper;

namespace MarWac.Merlin.UnitTests.YamlConfigurationSourceDriver
{
    [TestFixture]
    public class WritingTests
    {
        private static readonly string Nl = System.Environment.NewLine;

        [Test]
        public void Write_GivenOneParameterWithDefaultValueOnly_WritesCorrectly()
        {
            var configuration = new Configuration(
                new[]
                {
                    new ConfigurationParameter("maxThreads", "5")
                });

            var actual = WriteYaml(configuration);

            var expected = "parameters:" + Nl +
                           "- maxThreads: 5" + Nl;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}