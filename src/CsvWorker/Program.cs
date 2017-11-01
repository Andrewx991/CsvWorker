using CommandLine;
using System;
using System.Collections.Generic;

namespace CsvWorker
{
    class Program
    {
        static void Main(string[] args)
        {
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
            // TODO
        }
    }
}
