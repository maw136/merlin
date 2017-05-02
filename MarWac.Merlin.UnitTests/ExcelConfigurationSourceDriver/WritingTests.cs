using System.Collections.Generic;
using MarWac.Merlin.UnitTests.Utils;
using NUnit.Framework;
using static MarWac.Merlin.UnitTests.Utils.DriverWrapper;
using static MarWac.Merlin.UnitTests.Utils.XmlEqualityAssertions;

namespace MarWac.Merlin.UnitTests.ExcelConfigurationSourceDriver
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
                    {
                        Description = "Max number of threads"
                    }
                });

            var actual = WriteExcel(configuration);

            AssertCell(actual, new Cell(2, 1), "maxThreads");
            AssertCell(actual, new Cell(2, 2), "Max number of threads");
            AssertCell(actual, new Cell(2, 3), "5");
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

            var actual = WriteExcel(configuration);

            AssertCell(actual, new Cell(1, 4), "Local");
            AssertCell(actual, new Cell(1, 5), "Test");
            AssertCell(actual, new Cell(2, 1), "maxThreads");
            AssertCell(actual, new Cell(2, 3), "5");
            AssertCell(actual, new Cell(2, 4), "15");
            AssertCell(actual, new Cell(2, 5), "25");
        }

        [Test]
        public void Write_GivenTwoParametersWithMultipleEnvironmentValues_WritesCorrectly()
        {
            var configuration = new Configuration(
                new[]
                {
                    new ConfigurationParameter("maxThreads", null, new Dictionary<ConfigurableEnvironment, string>
                    {
                        {new ConfigurableEnvironment("Local"), "15"},
                        {new ConfigurableEnvironment("Test"), "25"}
                    })
                    {
                        Description = "Max number of threads"
                    },
                    new ConfigurationParameter("timeoutSecs", "40", new Dictionary<ConfigurableEnvironment, string>
                    {
                        {new ConfigurableEnvironment("Test"), "60"}
                    })
                },
                new[]
                {
                    new ConfigurableEnvironment("Local"),
                    new ConfigurableEnvironment("Test")
                });

            var actual = WriteExcel(configuration);

            AssertCell(actual, new Cell(1, 4), "Local");
            AssertCell(actual, new Cell(1, 5), "Test");

            AssertCell(actual, new Cell(2, 1), "maxThreads");
            AssertCell(actual, new Cell(2, 2), "Max number of threads");
            AssertCell(actual, new Cell(2, 3), string.Empty);
            AssertCell(actual, new Cell(2, 4), "15");
            AssertCell(actual, new Cell(2, 5), "25");

            AssertCell(actual, new Cell(3, 1), "timeoutSecs");
            AssertCell(actual, new Cell(3, 2), string.Empty);
            AssertCell(actual, new Cell(3, 3), "40");
            AssertCell(actual, new Cell(3, 4), string.Empty);
            AssertCell(actual, new Cell(3, 5), "60");
        }
    }
}