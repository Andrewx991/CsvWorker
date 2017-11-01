using CommandLine;

namespace CsvWorker
{
    internal class Options
    {
        [Option('i', "inputDirectory", Required = true, HelpText = "The directory that files will be read from.")]
        public string InputDirectory { get; set; }

        [Option('o', "outputDirectory", Required = true, HelpText = "The directory that files will be output to.")]
        public string OutputDirectory { get; set; }

        [Option('e', "errorDirectory", Required = true, HelpText = "The directory that errors will be output to.")]
        public string ErrorDirectory { get; set; }
    }
}
