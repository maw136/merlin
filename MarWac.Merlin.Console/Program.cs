using System.IO;
using CommandLine;

namespace MarWac.Merlin.Console
{
    static class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();

            if (!Parser.Default.ParseArguments(args, options))
            {
                return;
            }

            TranformSourceFileIntoTargetFile(options);
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
