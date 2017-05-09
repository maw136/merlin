using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace MarWac.Merlin.Console
{
    internal class Options
    {
        [Option('f', "from", Required = true, HelpText = "Configuration source file to be transformed.")]
        public string SourceFile { get; set; }

        [Option('t', "to", Required = true, HelpText = "Output containing transformed configuration.")]
        public string TargetFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            help.Heading = new HeadingInfo(assemblyName.Name, assemblyName.Version.ToString(3));
            help.AddPostOptionsLine("Currently only .xml (Excel 2003 XML) and .yml (YAML) formats are supported.");
            return help;
        }
    }
}