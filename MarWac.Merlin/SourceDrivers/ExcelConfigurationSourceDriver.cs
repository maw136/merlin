using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MarWac.Merlin.SourceDrivers
{
    /// <summary>
    /// Retrieves/stores the configSettings from/to Excel XML 2003 format stream.
    /// </summary>
    public class ExcelConfigurationSourceDriver : ConfigurationSourceDriver
    {
        internal static readonly XNamespace Ns = "urn:schemas-microsoft-com:office:spreadsheet";

        /// <summary>
        /// Retrieves the configSettings from Excel XML 2003 format stream.
        /// </summary>
        /// <param name="source">An Excel XML 2003 source stream</param>
        /// <returns>ConfigSettings instance filled with data from the the source</returns>
        /// <exception cref="SourceReadException">Thrown if the Excel XML cannot be read as XML
        /// (XML-malformed).</exception>
        /// <exception cref="InvalidExcelConfigurationFormatException">Thrown if content of the Excel source does not
        /// align with the expected format.</exception>
        public override ConfigSettings Read(Stream source) => Reader.Read(source);

        /// <summary>
        /// Stores the configSettings to Excel XML 2003 format stream.
        /// </summary>
        /// <param name="output">An output stream to store the configSettings</param>
        /// <param name="configSettings">ConfigSettings instance to be stored to the stream</param>
        public override void Write(Stream output, ConfigSettings configSettings) =>
            new Writer(configSettings).Write(output);

        private static class Reader
        {
            private const int ColumnIndexOfFirstEnvironment = 4; // first column has index = 1
            private const int EnvironmentColumnsShift = ColumnIndexOfFirstEnvironment - 1;

            public static ConfigSettings Read(Stream source)
            {
                var allRows = GetAllTableRows(XElement.Load(source));

                if (!allRows.Any())
                {
                    return new ConfigSettings(Enumerable.Empty<ConfigurationParameter>());
                }

                ConfigurableEnvironment[] environments = ParseHeader(allRows);
                IEnumerable<ConfigurationParameter> parameters = ParseParameters(allRows, environments);

                return new ConfigSettings(parameters, environments);
            }

            private static XElement[] GetAllTableRows(XElement root)
            {
                return root.Descendants(Ns + "Table")
                           .FirstOrDefault()?
                           .Elements(Ns + "Row")
                           .ToArray() ?? new XElement[] {};
            }

            private static ConfigurableEnvironment[] ParseHeader(IReadOnlyList<XElement> allRows)
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

                var environments = headerCells
                    .Skip(3)
                    .TakeWhile(IsNotBlank)
                    .Select(cell => new ConfigurableEnvironment(GetCellValue(cell)))
                    .ToArray();

                return environments;
            }

            private static IEnumerable<ConfigurationParameter> ParseParameters(IEnumerable<XElement> allRows,
                ConfigurableEnvironment[] environments)
            {
                return allRows
                    .Skip(1) // the header row
                    .TakeWhile(IsNotBlank)
                    .Select(row => CreateParameter(row.Elements(Ns + "Cell"), environments));
            }

            private static ConfigurationParameter CreateParameter(IEnumerable<XElement> rowCells,
                ConfigurableEnvironment[] environments)
            {
                var environmentValuesMapping = new Dictionary<ConfigurableEnvironment, string>();
                var currentColumnIndex = 1;
                var lastEnvironmentColumnIndex = environments.Length + EnvironmentColumnsShift;
                string paramName = null;
                string paramDescription = null;
                string paramDefaultValue = null;

                foreach (var cell in rowCells)
                {
                    currentColumnIndex = GetAdjustedColumnIndex(cell) ?? currentColumnIndex;
                    if (currentColumnIndex > lastEnvironmentColumnIndex)
                    {
                        break;
                    }

                    BuildParamPart(environments, cell, currentColumnIndex,
                        ref paramName, ref paramDescription, ref paramDefaultValue, environmentValuesMapping);

                    currentColumnIndex++;
                    if (currentColumnIndex > lastEnvironmentColumnIndex)
                    {
                        break;
                    }
                }

                return new ConfigurationParameter(paramName, paramDefaultValue, environmentValuesMapping)
                {
                    Description = paramDescription
                };
            }

            private static void BuildParamPart(ConfigurableEnvironment[] environments,
                XElement cell, int currentColumnIndex,
                ref string paramName, ref string paramDescription, ref string paramDefaultValue,
                Dictionary<ConfigurableEnvironment, string> environmentValuesMapping)
            {
                string cellValue = GetCellValue(cell);

                if (string.IsNullOrEmpty(cellValue))
                {
                    return;
                }

                if (currentColumnIndex <= EnvironmentColumnsShift)
                {
                    switch (currentColumnIndex)
                    {
                        case 1:
                            paramName = cellValue;
                            break;
                        case 2:
                            paramDescription = cellValue;
                            break;
                        case 3:
                            paramDefaultValue = cellValue;
                            break;
                    }
                }
                else
                {
                    var environment = environments[currentColumnIndex - EnvironmentColumnsShift - 1];

                    environmentValuesMapping.Add(environment, cellValue);
                }
            }

            private static int? GetAdjustedColumnIndex(XElement cell)
            {
                return cell
                    .Attributes(Ns + "Index")
                    .Select(a => (int?) int.Parse(a.Value, NumberStyles.None, CultureInfo.InvariantCulture))
                    .FirstOrDefault();
            }

            private static bool IsNotBlank(XElement element) => !element.Attributes(Ns + "Index").Any();

            private static string GetCellValue(XElement cellElement)
            {
                return cellElement.Elements(Ns + "Data").FirstOrDefault()?.Value;
            }
        }

        private class Writer
        {
            private const string MainSheetName = "ConfigurationDictionary";
            private readonly ConfigSettings _configSettings;
            private readonly ConfigurableEnvironment[] _environments;
            private static readonly string[] HeaderColumnNames = {"Name", "Description", "Default"};

            public Writer(ConfigSettings configSettings)
            {
                _configSettings = configSettings;
                _environments = _configSettings.Environments.ToArray();
            }

            public void Write(Stream output)
            {
                var content = new XStreamingElement(Ns + "Workbook",
                    new XAttribute(XNamespace.Xmlns + "ss", Ns),
                    new XStreamingElement(Ns + "Worksheet", new XAttribute(Ns + "Name", MainSheetName),
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
                    from parameter in _configSettings.Parameters
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
                    else
                    {
                        valuesInEnvironments[index] = parameter.DefaultValue;
                    }
                }
                return valuesInEnvironments;
            }

            private static XElement CreateRow(IEnumerable<string> values)
            {
                return new XElement(Ns + "Row", values.Select(v => new XElement(Ns + "Cell",
                    new XElement(Ns + "Data", new XAttribute(Ns + "Type", "String"), v))));
            }
        }
    }
}