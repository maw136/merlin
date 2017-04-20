using System.IO;
using System.Linq;
using System.Text;

namespace MarWac.Merlin
{
    public class ExcelConfigurationSourceDriver
    {
        private const string XmlHeader = "<?xml version=\"1.0\"?>\r\n" +
                                         "<?mso-application progid=\"Excel.Sheet\"?>\r\n";

        private const string BeginningTableInWorksheetInWorkbookElements = 
            "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
            "       xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">\r\n" +
            "  <Worksheet ss:Name=\"Sheet1\">\r\n" +
            "    <Table>\r\n";

        private const string ClosingTableInWorksheetInWorkbookElements = 
            "    </Table>\r\n" +
            "  </Worksheet>\r\n" +
            "</Workbook>";

        public void Write(Stream output, Configuration configuration)
        {
            using (var writer = new StreamWriter(output, Encoding.UTF8, 512, true))
            {
                writer.Write(XmlHeader);
                writer.Write(BeginningTableInWorksheetInWorkbookElements);

                writer.WriteLine("      <Row>");
                writer.WriteLine("        <Cell><Data ss:Type=\"String\">Name</Data></Cell>");
                writer.WriteLine("        <Cell><Data ss:Type=\"String\">Description</Data></Cell>");
                writer.WriteLine("        <Cell><Data ss:Type=\"String\">Default</Data></Cell>");
                writer.WriteLine("      </Row>");

                foreach (var parameter in configuration.Parameters)
                {
                    writer.WriteLine("      <Row>");
                    writer.WriteLine($"        <Cell><Data ss:Type=\"String\">{parameter.Name}</Data></Cell>");
                    writer.WriteLine($"        <Cell><Data ss:Type=\"String\">{parameter.Description}</Data></Cell>");
                    writer.WriteLine($"        <Cell><Data ss:Type=\"String\">{parameter.DefaultValue}</Data></Cell>");
                    writer.WriteLine("      </Row>");
                }

                writer.Write(ClosingTableInWorksheetInWorkbookElements);
                writer.Flush();
            }
        }
    }
}