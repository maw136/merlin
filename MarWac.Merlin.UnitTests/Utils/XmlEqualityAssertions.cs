using System.Collections.Generic;
using NUnit.Framework;
using Org.XmlUnit.Constraints;

namespace MarWac.Merlin.UnitTests.Utils
{
    /// <summary>
    /// A utility providing checking XML equality of actual utest result XML and expected output XML.
    /// </summary>
    internal static class XmlEqualityAssertions
    {
        public static void AssertCell(string actual, Cell cell, string value)
        {
            var constraint = EvaluateXPathConstraint
                                .HasXPath("/x:Workbook/x:Worksheet/x:Table" +
                                         $"/x:Row[{cell.Row}]/x:Cell[{cell.Column}]/x:Data/text()", Is.EqualTo(value))
                                .WithNamespaceContext(new Dictionary<string, string>
                                {
                                    { "x", SourceDrivers.ExcelConfigurationSourceDriver.Ns.NamespaceName }
                                });
            Assert.That(actual, constraint);
        }
    }
}
