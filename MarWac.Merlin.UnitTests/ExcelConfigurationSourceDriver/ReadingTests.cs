using System.Linq;
using NUnit.Framework;
using static MarWac.Merlin.UnitTests.Utils.DriverWrapper;

namespace MarWac.Merlin.UnitTests.ExcelConfigurationSourceDriver
{
    [TestFixture]
    public class ReadingTests
    {
        [Test]
        public void Read_GivenA1IsNotTitledName_Throws()
        {
            var source = 
                @"<?xml version=""1.0""?>
                <?mso-application progid=""Excel.Sheet""?>
                <Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                       xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"">  
                  <Worksheet ss:Name=""Sheet1"">
                    <Table>
                      <Row>
                        <Cell><Data ss:Type=""String"">Not Name</Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => ReadExcel(source));

            Assert.That(ex.Message, Is.EqualTo("A1 cell should be `Name`"));
        }

        [Test]
        public void Read_GivenB1IsNotTitledDescription_Throws()
        {
            var source = 
                @"<?xml version=""1.0""?>
                <?mso-application progid=""Excel.Sheet""?>
                <Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                       xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"">  
                  <Worksheet ss:Name=""Sheet1"">
                    <Table>
                      <Row>
                        <Cell><Data ss:Type=""String"">Name</Data></Cell>
                        <Cell><Data ss:Type=""String""></Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => ReadExcel(source));

            Assert.That(ex.Message, Is.EqualTo("B1 cell should be `Description`"));
        }

        [Test]
        public void Read_GivenC1IsNotTitledDefault_Throws()
        {
            var source = 
                @"<?xml version=""1.0""?>
                <?mso-application progid=""Excel.Sheet""?>
                <Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                       xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"">  
                  <Worksheet ss:Name=""Sheet1"">
                    <Table>
                      <Row>
                        <Cell><Data ss:Type=""String"">Name</Data></Cell>
                        <Cell><Data ss:Type=""String"">Description</Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => ReadExcel(source));

            Assert.That(ex.Message, Is.EqualTo("C1 cell should be `Default`"));
        }

        [Test]
        public void Read_GivenMultipleParams_ReadsAllParamsUpToFirstBlankRow()
        {
            var source = 
                $@"<?xml version=""1.0""?>
                <?mso-application progid=""Excel.Sheet""?>
                <Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                       xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"">  
                  <Worksheet ss:Name=""Sheet1"">
                    <Table>
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
                        <Cell><Data ss:Type=""String"">{"http://server.com/api"}</Data></Cell>
                      </Row>
                      <Row ss:Index=""4"">
                        <Cell><Data ss:Type=""String"">disregardedParam</Data></Cell>
                        <Cell><Data ss:Type=""String"">This param will not be taken into account</Data></Cell>
                        <Cell><Data ss:Type=""String""></Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var config = ReadExcel(source);

            Assert.That(config.Parameters.Count, Is.EqualTo(2));

            var param1 = config.Parameters.ElementAt(0);
            Assert.That(param1.Name, Is.EqualTo("maxThreads"));
            Assert.That(param1.Description, Is.EqualTo("Thread limit"));
            Assert.That(param1.DefaultValue, Is.EqualTo("5"));

            var param2 = config.Parameters.ElementAt(1);
            Assert.That(param2.Name, Is.EqualTo("apiBaseUri"));
            Assert.That(param2.Description, Is.EqualTo("Base URI for server REST API"));
            Assert.That(param2.DefaultValue, Is.EqualTo("http://server.com/api"));
        }
    }
}