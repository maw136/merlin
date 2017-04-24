using NUnit.Framework;

namespace MarWac.Merlin.UnitTests.ExcelConfigurationSourceDriver
{
    [TestFixture]
    public class ReadingTests
    {
        private const string XmlWrapUpFormat = @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
       xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"">
  <Worksheet ss:Name=""Sheet1"">
    <Table>{0}
    </Table>
  </Worksheet>
</Workbook>";

        [Test]
        public void Read_GivenA1IsNotTitledName_Throws()
        {
            var source = ExcelDocWith(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Not Name</Data></Cell>
                </Row>");

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => Read(source));

            Assert.That(ex.Message, Is.EqualTo("A1 cell should be `Name`"));
        }

        [Test]
        public void Read_GivenB1IsNotTitledDescription_Throws()
        {
            var source = ExcelDocWith(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Name</Data></Cell>
                  <Cell><Data ss:Type=""String""></Data></Cell>
                </Row>");

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => Read(source));

            Assert.That(ex.Message, Is.EqualTo("B1 cell should be `Description`"));
        }

        [Test]
        public void Read_GivenC1IsNotTitledDefault_Throws()
        {
            var source = ExcelDocWith(@"
                <Row>
                  <Cell><Data ss:Type=""String"">Name</Data></Cell>
                  <Cell><Data ss:Type=""String"">Description</Data></Cell>
                </Row>");

            var ex = Assert.Throws<InvalidExcelConfigurationFormatException>(() => Read(source));

            Assert.That(ex.Message, Is.EqualTo("C1 cell should be `Default`"));
        }

        private Configuration Read(string source)
        {
            return new Configuration(new ConfigurationParameter[] {});
        }

        private static string ExcelDocWith(string excelData) => string.Format(XmlWrapUpFormat, excelData);
    }
}