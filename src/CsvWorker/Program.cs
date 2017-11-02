using CommandLine;
using System;
using System.Collections.Generic;

namespace CsvWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Started...");

            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(LaunchWorker)
                .WithNotParsed(TerminateUnsuccessfully);
        }

        private static void TerminateUnsuccessfully(IEnumerable<Error> errors)
        {
            Environment.Exit(-1);
        }

        private static void LaunchWorker(Options options)
        {
            var worker = new Worker(options.InputDirectory, options.OutputDirectory, options.ErrorDirectory);
            worker.Start();
        }
    }
}
