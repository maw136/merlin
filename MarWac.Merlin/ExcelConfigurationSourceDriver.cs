using System.Collections.Generic;
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

        private const string CellValueFormat = "        <Cell><Data ss:Type=\"String\">{0}</Data></Cell>";

        public void Write(Stream output, Configuration configuration)
        {
            using (var writer = new StreamWriter(output, Encoding.UTF8, bufferSize: 512, leaveOpen: true))
            {
                writer.Write(XmlHeader);
                writer.Write(BeginningTableInWorksheetInWorkbookElements);
                var environmentsArray = configuration.Environments.ToArray();
                WriteHeaderRow(writer, environmentsArray);
                WriteParameters(writer, configuration, environmentsArray);
                writer.Write(ClosingTableInWorksheetInWorkbookElements);

                writer.Flush();
            }
        }

        private static void WriteHeaderRow(StreamWriter writer, ConfigurableEnvironment[] environmentsArray)
        {
            WriteRow(writer, "Name", "Description", "Default", environmentsArray.Select(env => env.Name));
        }

        private static void WriteParameters(StreamWriter writer, Configuration configuration,
            ConfigurableEnvironment[] environmentsArray)
        {
            foreach (var parameter in configuration.Parameters)
            {
                var valuesInEnvironments = CalculateValuesPerAllEnvironments(parameter, environmentsArray);

                WriteRow(writer, parameter.Name, parameter.Description, parameter.DefaultValue, valuesInEnvironments);
            }
        }

        private static string[] CalculateValuesPerAllEnvironments(ConfigurationParameter parameter, 
            ConfigurableEnvironment[] environmentsArray)
        {
            var valuesInEnvironments = new string[environmentsArray.Length];
            for (var index = 0; index < environmentsArray.Length; index++)
            {
                var environment = environmentsArray[index];
                string parameterEnvironmentValue;
                if (parameter.Values.TryGetValue(environment, out parameterEnvironmentValue))
                {
                    valuesInEnvironments[index] = parameterEnvironmentValue;
                }
            }
            return valuesInEnvironments;
        }

        private static void WriteRow(StreamWriter writer, string name, string description, string @default,
            IEnumerable<string> environmentValues)
        {
            writer.WriteLine("      <Row>");
            writer.WriteLine(CellValueFormat, name);
            writer.WriteLine(CellValueFormat, description);
            writer.WriteLine(CellValueFormat, @default);

            foreach (var environmentValue in environmentValues)
            {
                writer.WriteLine(CellValueFormat, environmentValue);
            }

            writer.WriteLine("      </Row>");
        }
    }
}