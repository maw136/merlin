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

        [Test]
        public void Read_GivenSingleParamWithMultipleEnvironments_ReadsEnvironmentConfigurationCorrectly()
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
                        <Cell><Data ss:Type=""String"">Localhost</Data></Cell>
                        <Cell><Data ss:Type=""String"">Test</Data></Cell>
                        <Cell ss:Index=""7""><Data ss:Type=""String"">Disregarded since after blank</Data></Cell>
                      </Row>
                      <Row>
                        <Cell><Data ss:Type=""String"">maxThreads</Data></Cell>
                        <Cell ss:Index=""3""><Data ss:Type=""String"">10</Data></Cell>
                        <Cell><Data ss:Type=""String"">20</Data></Cell>
                        <Cell><Data ss:Type=""String"">30</Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var config = ReadExcel(source);

            Assert.That(config.Environments.Count, Is.EqualTo(2));

            var param = config.Parameters.First();
            Assert.That(param.Name, Is.EqualTo("maxThreads"));
            Assert.That(param.Description, Is.Null);
            Assert.That(param.DefaultValue, Is.EqualTo("10"));
            Assert.That(param.Values.Keys.Count(), Is.EqualTo(2));
            Assert.That(param.Values[new ConfigurableEnvironment("Localhost")], Is.EqualTo("20"));
            Assert.That(param.Values[new ConfigurableEnvironment("Test")], Is.EqualTo("30"));
        }

        [Test]
        public void Read_GivenMultipleParamsWithMultipleEnvironments_ReadsAllCorrectly()
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
                        <Cell><Data ss:Type=""String"">Localhost</Data></Cell>
                        <Cell><Data ss:Type=""String"">Test</Data></Cell>
                      </Row>
                      <Row>
                        <Cell><Data ss:Type=""String"">maxThreads</Data></Cell>
                        <Cell><Data ss:Type=""String"">Thread limit</Data></Cell>
                        <Cell><Data ss:Type=""String"">10</Data></Cell>
                        <Cell><Data ss:Type=""String"">20</Data></Cell>
                      </Row>
                      <Row>
                        <Cell><Data ss:Type=""String"">installUri</Data></Cell>
                        <Cell ss:Index=""4""><Data ss:Type=""String"">http://local/share</Data></Cell>
                      </Row>
                      <Row>
                        <Cell><Data ss:Type=""String"">allowedEntries</Data></Cell>
                        <Cell ss:Index=""3""><Data ss:Type=""String"">6</Data></Cell>
                        <Cell ss:Index=""12""><Data ss:Type=""String"">12</Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var config = ReadExcel(source);

            Assert.That(config.Environments.Count, Is.EqualTo(2));
            Assert.That(config.Parameters.Count, Is.EqualTo(3));

            var param1 = config.Parameters.ElementAt(0);
            Assert.That(param1.Name, Is.EqualTo("maxThreads"));
            Assert.That(param1.Description, Is.EqualTo("Thread limit"));
            Assert.That(param1.DefaultValue, Is.EqualTo("10"));
            Assert.That(param1.Values.Keys.Count(), Is.EqualTo(1));
            Assert.That(param1.Values[new ConfigurableEnvironment("Localhost")], Is.EqualTo("20"));

            var param2 = config.Parameters.ElementAt(1);
            Assert.That(param2.Name, Is.EqualTo("installUri"));
            Assert.That(param2.Description, Is.Null);
            Assert.That(param2.DefaultValue, Is.Null);
            Assert.That(param2.Values.Keys.Count(), Is.EqualTo(1));
            Assert.That(param2.Values[new ConfigurableEnvironment("Localhost")], Is.EqualTo("http://local/share"));

            var param3 = config.Parameters.ElementAt(2);
            Assert.That(param3.Name, Is.EqualTo("allowedEntries"));
            Assert.That(param3.Description, Is.Null);
            Assert.That(param3.DefaultValue, Is.EqualTo("6"));
            Assert.That(param3.Values.Keys.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Read_EmptyParams_ReadsCorrectly()
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
                        <Cell><Data ss:Type=""String"">Localhost</Data></Cell>
                      </Row>
                    </Table>
                  </Worksheet>
                </Workbook>";

            var config = ReadExcel(source);

            Assert.That(config.Environments.Count, Is.EqualTo(1));
            Assert.That(config.Parameters.Count, Is.EqualTo(0));
        }
    }
}