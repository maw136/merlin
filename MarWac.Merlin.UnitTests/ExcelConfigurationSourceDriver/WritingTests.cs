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

            var actualOut = Write(configuration);

            const string expected = @"
                <?xml version=""1.0""?>
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
                    <Cell><Data ss:Type=""String"">Max number of threads</Data></Cell>
                    <Cell><Data ss:Type=""String"">5</Data></Cell>
                   </Row>
                  </Table>
                 </Worksheet>
                </Workbook>";

            Assert.That(actualOut, Is.EqualTo(expected));
        }

        private string Write(Configuration configuration)
        {
            return null;
        }
    }
}