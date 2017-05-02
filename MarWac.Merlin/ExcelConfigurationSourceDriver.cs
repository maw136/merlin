using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MarWac.Merlin
{
    /// <summary>
    /// Retrieves/stores the configuration from/to Excel XML 2003 format stream.
    /// </summary>
    public class ExcelConfigurationSourceDriver : ConfigurationSourceDriver
    {
        internal static readonly XNamespace Ns = "urn:schemas-microsoft-com:office:spreadsheet";

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
            new Writer(configuration).Write(output);

        private class Reader
        {
            public Configuration Read(Stream source)
            {
                var allRows = GetAllTableRows(XElement.Load(source));

                if (!allRows.Any())
                {
                    return new Configuration(Enumerable.Empty<ConfigurationParameter>());
                }
                
                ParseHeader(allRows);

                IEnumerable<ConfigurationParameter> parameters = ParseParameters(allRows);

                return new Configuration(parameters);
            }

            private static XElement[] GetAllTableRows(XElement root)
            {
                return root.Descendants(Ns + "Table")
                           .FirstOrDefault()?
                           .Elements(Ns + "Row")
                           .ToArray() ?? new XElement[] {};
            }

            private static void ParseHeader(IReadOnlyList<XElement> allRows)
            {
                var headerCells = allRows[0].Elements(Ns + "Cell").ToArray();

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
            }

            private static IEnumerable<ConfigurationParameter> ParseParameters(IEnumerable<XElement> allRows)
            {
                var paramRowsTillFirstBlank = allRows
                    .Skip(1)
                    .TakeWhile(row => row.Attributes()
                        .All(attr => attr.Name != Ns + "Index"));

                return ReadParamsRowByRow(paramRowsTillFirstBlank);
            }

            private static IEnumerable<ConfigurationParameter> ReadParamsRowByRow(IEnumerable<XElement> paramRows)
            {
                // TODO: handling blank cells
                return 
                    from row in paramRows
                    let cells = row.Elements(Ns + "Cell").ToArray()
                    let name = GetCellValue(cells[0])
                    let defaultValue = GetCellValue(cells[2])
                    let description = GetCellValue(cells[1])
                    select new ConfigurationParameter(name, defaultValue)
                    {
                        Description = description
                    };
            }

            private static string GetCellValue(XElement cellElement)
            {
                return cellElement.Elements(Ns + "Data").FirstOrDefault()?.Value;
            }
        }

        private class Writer
        {
            private readonly Configuration _configuration;
            private readonly ConfigurableEnvironment[] _environments;
            private static readonly string[] HeaderColumnNames = {"Name", "Description", "Default"};

            public Writer(Configuration configuration)
            {
                _configuration = configuration;
                _environments = _configuration.Environments.ToArray();
            }

            public void Write(Stream output)
            {
                var content = new XStreamingElement(Ns + "Workbook",
                    new XAttribute(XNamespace.Xmlns + "ss", Ns),
                    new XStreamingElement(Ns + "Worksheet", new XAttribute(Ns + "Name", "Sheet1"),
                        new XStreamingElement(Ns + "Table",
                            CreateHeaderRow(),
                            CreateParameterRows())));

                content.Save(output);
            }

            private XElement CreateHeaderRow() => CreateRow(HeaderColumnNames
                .Concat(_environments.Select(env => env.Name)));

            private IEnumerable<XElement> CreateParameterRows()
            {
                return 
                    from parameter in _configuration.Parameters
                    let nonEnvironmentValues = new[] {parameter.Name, parameter.Description, parameter.DefaultValue}
                    let valuesInEnvironments = CalculateValuesPerAllEnvironments(parameter)
                    select CreateRow(nonEnvironmentValues.Concat(valuesInEnvironments));
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

            private XElement CreateRow(IEnumerable<string> values)
            {
                return new XElement(Ns + "Row", values.Select(v => new XElement(Ns + "Cell",
                    new XElement(Ns + "Data", new XAttribute(Ns + "Type", "String"), v))));
            }
        }
    }
}