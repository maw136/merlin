using System.Linq;
using MarWac.Merlin.UnitTests.Utils;
using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.ExcelConfigurationSourceDriver
{
    [TestFixture]
    public class ReadingTests
    {
        [Test]
        public void Read_GivenA1IsNotTitledName_Throws()
        {
            var source = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Not Name</Data></Cell>
                </Row>");

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => DriverWrapper.ReadExcel(source));

            Assert.That(ex.Message, Is.EqualTo("A1 cell should be `Name`"));
        }

        [Test]
        public void Read_GivenB1IsNotTitledDescription_Throws()
        {
            var source = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Name</Data></Cell>
                  <Cell><Data ss:Type=""String""></Data></Cell>
                </Row>");

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => DriverWrapper.ReadExcel(source));

            Assert.That(ex.Message, Is.EqualTo("B1 cell should be `Description`"));
        }

        [Test]
        public void Read_GivenC1IsNotTitledDefault_Throws()
        {
            var source = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Name</Data></Cell>
                  <Cell><Data ss:Type=""String"">Description</Data></Cell>
                </Row>");

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => DriverWrapper.ReadExcel(source));

            Assert.That(ex.Message, Is.EqualTo("C1 cell should be `Default`"));
        }

        [Test]
        public void Read_GivenMultipleParams_ReadsAllParamsUpToFirstBlankRow()
        {
            var source = Merlin.ExcelConfigurationSourceDriver.CreateExcelXmlWithRows(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Name</Data></Cell>
                  <Cell><Data ss:Type=""String"">Description</Data></Cell>
                  <Cell><Data ss:Type=""String"">Default</Data></Cell>
                </Row>
                <Row>
                  <Cell><Data ss:Type=""String"">maxThreads</Data></Cell>
                  <Cell><Data ss:Type=""String"">Thread limit</Data></Cell>
                  <Cell><Data ss:Type=""String"">5</Data></Cell>
                </Row>
                <Row>
                  <Cell><Data ss:Type=""String"">apiBaseUri</Data></Cell>
                  <Cell><Data ss:Type=""String"">Base URI for server REST API</Data></Cell>
                  <Cell><Data ss:Type=""String"">http://server.com/api</Data></Cell>
                </Row>
                <Row ss:Index=""4"">
                  <Cell><Data ss:Type=""String"">disregardedParam</Data></Cell>
                  <Cell><Data ss:Type=""String"">This param will not be taken into account</Data></Cell>
                  <Cell><Data ss:Type=""String""></Data></Cell>
                </Row>");

            var config = DriverWrapper.ReadExcel(source);

            Assert.That(config.Parameters.Count, Is.EqualTo(2));

            Assert.That(config.Parameters.ElementAt(0).Name, Is.EqualTo("maxThreads"));
            Assert.That(config.Parameters.ElementAt(0).Description, Is.EqualTo("Thread limit"));
            Assert.That(config.Parameters.ElementAt(0).DefaultValue, Is.EqualTo("5"));

            Assert.That(config.Parameters.ElementAt(1).Name, Is.EqualTo("apiBaseUri"));
            Assert.That(config.Parameters.ElementAt(1).Description, Is.EqualTo("Base URI for server REST API"));
            Assert.That(config.Parameters.ElementAt(1).DefaultValue, Is.EqualTo("http://server.com/api"));
        }
    }
}