using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MarWac.Merlin
{
    /// <summary>
    /// Retrieves/stores the configuration from/to Excel XML 2003 format stream.
    /// </summary>
    public class ExcelConfigurationSourceDriver : ConfigurationSourceDriver
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

        /// <summary>
        /// Retrieves the configuration from Excel XML 2003 format stream.
        /// </summary>
        /// <param name="source">An Excel XML 2003 source stream</param>
        /// <returns>Configuration instance filled with data from the the source</returns>
        /// <exception cref="SourceReadException">Thrown if the Excel XML cannot be read as XML 
        /// (XML-malformed).</exception>
        /// <exception cref="InvalidExcelConfigurationFormatException">Thrown if content of the Excel source does not
        /// align with the expected format.</exception>
        public override Configuration Read(Stream source) => new Reader().Read(source);

        /// <summary>
        /// Stores the configuration to Excel XML 2003 format stream.
        /// </summary>
        /// <param name="output">An output stream to store the configuration</param>
        /// <param name="configuration">Configuration instance to be stored to the stream</param>
        public override void Write(Stream output, Configuration configuration) => 
            new Writer(output, configuration).Write();

        internal static string CreateExcelXmlWithRows(string rowsXml) 
            => string.Format($"{XmlHeader}{BeginningTableInWorksheetInWorkbookElements}" +
                             $"{{0}}\r\n{ClosingTableInWorksheetInWorkbookElements}", rowsXml.TrimStart('\r', '\n'));

        private class Reader
        {
            public Configuration Read(Stream source)
            {
                var xElement = XElement.Load(source);

                XNamespace ns = "urn:schemas-microsoft-com:office:spreadsheet";
                var rows = xElement.Descendants(ns + "Table").FirstOrDefault()?.Elements(ns + "Row");

                var headerCells = rows?.ElementAt(0).Elements(ns + "Cell").ToArray() ?? new XElement[] { };

                if (headerCells.Length == 0 || GetCellValue(headerCells[0]) != "Name")
                {
                    throw new InvalidExcelConfigurationFormatException("A1 cell should be `Name`");
                }

                if (headerCells.Length == 1 || GetCellValue(headerCells[1]) != "Description")
                {
                    throw new InvalidExcelConfigurationFormatException("B1 cell should be `Description`");
                }

                if (headerCells.Length == 2 || GetCellValue(headerCells[2]) != "Default")
                {
                    throw new InvalidExcelConfigurationFormatException("C1 cell should be `Default`");
                }

                var paramRowsTillFirstBlank = rows.Skip(1)
                                                  .TakeWhile(row => row.Attributes()
                                                                       .All(attr => attr.Name != ns + "Index"));
                IEnumerable<ConfigurationParameter> parameters = ReadParamsRowByRow(paramRowsTillFirstBlank);

                return new Configuration(parameters);
            }

            private IEnumerable<ConfigurationParameter> ReadParamsRowByRow(IEnumerable<XElement> paramRows)
            {
                XNamespace ns = "urn:schemas-microsoft-com:office:spreadsheet";

                return from row in paramRows
                       let cells = row.Elements(ns + "Cell").ToArray()
                       select new ConfigurationParameter(GetCellValue(cells[0]), GetCellValue(cells[2]))
                       {
                           Description = GetCellValue(cells[1])
                       };
            }

            private string GetCellValue(XElement cellElement)
            {
                XNamespace ns = "urn:schemas-microsoft-com:office:spreadsheet";
                return cellElement.Elements(ns + "Data").FirstOrDefault()?.Value;
            }
        }

        private class Writer : IDisposable
        {
            private readonly Stream _output;
            private readonly Configuration _configuration;
            private readonly ConfigurableEnvironment[] _environments;
            private StreamWriter _streamWriter;

            public Writer(Stream output, Configuration configuration)
            {
                _output = output;
                _configuration = configuration;
                _environments = _configuration.Environments.ToArray();
            }

            public void Write()
            {
                using (_streamWriter = new StreamWriter(_output, Encoding.UTF8, bufferSize: 512, leaveOpen: true))
                {
                    _streamWriter.Write(XmlHeader);
                    _streamWriter.Write(BeginningTableInWorksheetInWorkbookElements);
                    WriteHeaderRow();
                    WriteParameters();
                    _streamWriter.Write(ClosingTableInWorksheetInWorkbookElements);

                    _streamWriter.Flush();
                }
            }

            private void WriteHeaderRow()
            {
                WriteRow("Name", "Description", "Default", _environments.Select(env => env.Name));
            }

            private void WriteParameters()
            {
                foreach (var parameter in _configuration.Parameters)
                {
                    var valuesInEnvironments = CalculateValuesPerAllEnvironments(parameter);

                    WriteRow(parameter.Name, parameter.Description, parameter.DefaultValue, valuesInEnvironments);
                }
            }

            private string[] CalculateValuesPerAllEnvironments(ConfigurationParameter parameter)
            {
                var valuesInEnvironments = new string[_environments.Length];
                for (var index = 0; index < _environments.Length; index++)
                {
                    var environment = _environments[index];
                    string parameterEnvironmentValue;
                    if (parameter.Values.TryGetValue(environment, out parameterEnvironmentValue))
                    {
                        valuesInEnvironments[index] = parameterEnvironmentValue;
                    }
                }
                return valuesInEnvironments;
            }

            private void WriteRow(string name, string description, string @default, 
                IEnumerable<string> environmentValues)
            {
                _streamWriter.WriteLine("      <Row>");
                _streamWriter.WriteLine(CellValueFormat, name);
                _streamWriter.WriteLine(CellValueFormat, description);
                _streamWriter.WriteLine(CellValueFormat, @default);

                foreach (var environmentValue in environmentValues)
                {
                    _streamWriter.WriteLine(CellValueFormat, environmentValue);
                }

                _streamWriter.WriteLine("      </Row>");
            }

            public void Dispose()
            {
                _streamWriter.Dispose();
            }
        }
    }
}