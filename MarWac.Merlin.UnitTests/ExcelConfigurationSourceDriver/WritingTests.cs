﻿using System.Collections.Generic;
using MarWac.Merlin.UnitTests.Utils;
using NUnit.Framework;

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

            var actualOut = DriverWrapper.WriteExcel(configuration);

            string expected = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
      <Row>
        <Cell><Data ss:Type=""String"">Name</Data></Cell>
        <Cell><Data ss:Type=""String"">Description</Data></Cell>
        <Cell><Data ss:Type=""String"">Default</Data></Cell>
      </Row>
      <Row>
        <Cell><Data ss:Type=""String"">maxThreads</Data></Cell>
        <Cell><Data ss:Type=""String"">Max number of threads</Data></Cell>
        <Cell><Data ss:Type=""String"">5</Data></Cell>
      </Row>");

            Assert.That(actualOut, Is.EqualTo(expected));
        }

        [Test]
        public void Write_GivenOneParameterWithMultipleEnvironmentValues_WritesCorrectly()
        {
            var configuration = new Configuration(
                new[]
                {
                    new ConfigurationParameter("maxThreads", "5", new Dictionary<ConfigurableEnvironment, string>
                    {
                        { new ConfigurableEnvironment("Local"), "15" }, 
                        { new ConfigurableEnvironment("Test"), "25" } 
                    })
                    {
                        Description = "Max number of threads"
                    }
                },
                new []
                {
                    new ConfigurableEnvironment("Local"), 
                    new ConfigurableEnvironment("Test") 
                });

            var actualOut = DriverWrapper.WriteExcel(configuration);

            string expected = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
      <Row>
        <Cell><Data ss:Type=""String"">Name</Data></Cell>
        <Cell><Data ss:Type=""String"">Description</Data></Cell>
        <Cell><Data ss:Type=""String"">Default</Data></Cell>
        <Cell><Data ss:Type=""String"">Local</Data></Cell>
        <Cell><Data ss:Type=""String"">Test</Data></Cell>
      </Row>
      <Row>
        <Cell><Data ss:Type=""String"">maxThreads</Data></Cell>
        <Cell><Data ss:Type=""String"">Max number of threads</Data></Cell>
        <Cell><Data ss:Type=""String"">5</Data></Cell>
        <Cell><Data ss:Type=""String"">15</Data></Cell>
        <Cell><Data ss:Type=""String"">25</Data></Cell>
      </Row>");

            Assert.That(actualOut, Is.EqualTo(expected));
        }

        [Test]
        public void Write_GivenTwoParametersWithMultipleEnvironmentValues_WritesCorrectly()
        {
            var configuration = new Configuration(
                new[]
                {
                    new ConfigurationParameter("maxThreads", null, new Dictionary<ConfigurableEnvironment, string>
                    {
                        { new ConfigurableEnvironment("Local"), "15" },
                        { new ConfigurableEnvironment("Test"), "25" }
                    })
                    {
                        Description = "Max number of threads"
                    },
                    new ConfigurationParameter("timeoutSecs", "40", new Dictionary<ConfigurableEnvironment, string>
                    {
                        { new ConfigurableEnvironment("Test"), "60" }
                    })
                },
                new[]
                {
                    new ConfigurableEnvironment("Local"),
                    new ConfigurableEnvironment("Test")
                });

            var actualOut = DriverWrapper.WriteExcel(configuration);

            string expected = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
      <Row>
        <Cell><Data ss:Type=""String"">Name</Data></Cell>
        <Cell><Data ss:Type=""String"">Description</Data></Cell>
        <Cell><Data ss:Type=""String"">Default</Data></Cell>
        <Cell><Data ss:Type=""String"">Local</Data></Cell>
        <Cell><Data ss:Type=""String"">Test</Data></Cell>
      </Row>
      <Row>
        <Cell><Data ss:Type=""String"">maxThreads</Data></Cell>
        <Cell><Data ss:Type=""String"">Max number of threads</Data></Cell>
        <Cell><Data ss:Type=""String""></Data></Cell>
        <Cell><Data ss:Type=""String"">15</Data></Cell>
        <Cell><Data ss:Type=""String"">25</Data></Cell>
      </Row>
      <Row>
        <Cell><Data ss:Type=""String"">timeoutSecs</Data></Cell>
        <Cell><Data ss:Type=""String""></Data></Cell>
        <Cell><Data ss:Type=""String"">40</Data></Cell>
        <Cell><Data ss:Type=""String""></Data></Cell>
        <Cell><Data ss:Type=""String"">60</Data></Cell>
      </Row>");

            Assert.That(actualOut, Is.EqualTo(expected));
        }
    }
}