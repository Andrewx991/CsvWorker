using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CsvWorker
{
    internal class Worker
    {
        private readonly string inputDirectory;
        private readonly string outputDirectory;
        private readonly string errorDirectory;
        private const int SLEEP_INTERVAL_MILLISECONDS = 5000; // TODO: Make configurable?

        public Worker(string inputDirectory, string outputDirectory, string errorDirectory)
        {
            this.inputDirectory = inputDirectory;
            this.outputDirectory = outputDirectory;
            this.errorDirectory = errorDirectory;
        }

        public void Start()
        {
            while (true)
            {
                // Get list of files in directory
                // Filter out files already processed via set
                // Foreach new file
                     // Try Lock File & Try Move File To Handle File System Race Condition When Going To Delete (May only be needed if requirement around reprocessing observed files changes)
                     // Add filename to set if set not at max size limit, else error and exit application with error code.
                     // Foreach line in new file
                        // If first line check if headers mismatch requirements, if so error & stop processing file, else open json array notation
                        // Else parse line to Json, exclude empty fields in serialization, write to output file (with comma and newline), unless file size exceed, then close js array notation & stop processing file.
                            // If parse error, write line to error file unless max file size exceeded in which case close json array notation stop processing file.
                    // Release Lock, Try Delete File If 100% Success
                // Wait some duration before rechecking to avoid excessive cpu spin.

                Thread.Sleep(SLEEP_INTERVAL_MILLISECONDS);
            }
        }
    }
}
