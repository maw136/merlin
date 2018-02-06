using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace MarWac.Merlin.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(TranformSourceFileIntoTargetFile);
            if (result.Tag == ParserResultType.NotParsed)
            {
                var help = HelpText.AutoBuild(result,
                    current => HelpText.DefaultParsingErrorsHandler(result, current), null);
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                help.Heading = new HeadingInfo(assemblyName.Name, assemblyName.Version.ToString(3));
                help.AddPostOptionsLine(
                    "Currently only .xml (Excel 2003 XML) and .yml (YAML) formats are supported.");
                System.Console.WriteLine(help);
            }
        }

        private static void TranformSourceFileIntoTargetFile(Options options)
        {
            using (var source = new FileStream(options.SourceFile, FileMode.Open, FileAccess.Read))
            using (var target = new FileStream(options.TargetFile, FileMode.Create, FileAccess.Write))
            {
                var sourceDriver = SourceDriverFactory.CreateByFileName(options.SourceFile);
                var targetDriver = SourceDriverFactory.CreateByFileName(options.TargetFile);

                var configuration = sourceDriver.Read(source);
                targetDriver.Write(target, configuration);
            }
        }
    }
}
