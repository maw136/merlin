using System;
using System.Collections.Generic;
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
            var ex = Assert.Throws<ArgumentException>(() => {
                new ConfigurationParameter("threadLimit", "", new Dictionary<ConfigurableEnvironment, string>());
            });

            Assert.That(ex.Message, Is.EqualTo("Either default value or value mapping per environment of parameter " +
                                               "`threadLimit` must be non empty."));
        }
    }
}