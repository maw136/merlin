using System;
using MarWac.Merlin.SourceDrivers;

namespace MarWac.Merlin.Console
{
    internal class SourceDriverFactory
    {
        public static ConfigurationSourceDriver CreateByFileName(string fileName)
        {
            if (fileName.EndsWith(".yml"))
            {
                return new YamlConfigurationSourceDriver();
            }
            if (fileName.EndsWith(".xml"))
            {
                return new ExcelConfigurationSourceDriver();
            }

            throw new InvalidOperationException("Unknown file extension. Cannot create a source driver.");
        }
    }
}