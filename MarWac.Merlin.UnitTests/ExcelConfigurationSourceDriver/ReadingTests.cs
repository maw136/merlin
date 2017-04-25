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
    }
}