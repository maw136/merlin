using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace MarWac.Merlin.Console
{
    internal class Options
    {
        [Option('f', "from", Required = true, HelpText = "ConfigSettings source file to be transformed.")]
        public string SourceFile { get; set; }

        [Option('t', "to", Required = true, HelpText = "Output containing transformed configuration.")]
        public string TargetFile { get; set; }
    }
}