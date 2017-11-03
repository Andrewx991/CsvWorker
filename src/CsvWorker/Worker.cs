using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CsvWorker
{
    internal class Worker
    {
        private readonly string inputDirectory;
        private readonly string outputDirectory;
        private readonly string errorDirectory;
        private const int SLEEP_INTERVAL_MILLISECONDS = 5000;
        private HashSet<string> processedFilepaths = new HashSet<string>();

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
                var filepaths = Directory.GetFiles(inputDirectory);
                var unprocessedFiles = filepaths.Except(processedFilepaths);

                foreach (var filepath in unprocessedFiles)
                {
                    var filename = Path.GetFileName(filepath);
                    int currentLineNumber = 1;
                    bool processedSuccessfully = true;

                    processedFilepaths.Add(filepath);

                    using (var streamReader = new StreamReader(filepath))
                    using (var streamWriter = new StreamWriter(Path.Combine(outputDirectory, filename.Replace("csv", "json", StringComparison.OrdinalIgnoreCase))))
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartArray();

                        while (!streamReader.EndOfStream)
                        {
                            var currentLine = streamReader.ReadLine();

                            if (!String.IsNullOrWhiteSpace(currentLine))
                            {
                                var values = currentLine.Split(',');

                                if (currentLineNumber == 1)
                                {
                                    if (!ValidHeaders(values))
                                    {
                                        LogError(filename, "Invalid Header Row.", currentLineNumber);
                                        processedSuccessfully = false;
                                    }
                                }
                                else
                                {
                                    Entry entry = null;
                                    var parsed = Entry.TryParse(values, out entry);

                                    if (!parsed)
                                    {
                                        LogError(filename, "Row Parsing Error", currentLineNumber);
                                        processedSuccessfully = false;
                                    }
                                    else
                                    {
                                        var settings = new JsonSerializerSettings();
                                        settings.NullValueHandling = NullValueHandling.Ignore;
                                        var jsonEntry = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
                                        jsonWriter.WriteRawValue(jsonEntry);
                                    }
                                }
                            }

                            currentLineNumber++;
                        }

                        jsonWriter.WriteEndArray();
                    }

                    if (processedSuccessfully)
                    {
                        File.Delete(filepath);
                    }
                }

                Thread.Sleep(SLEEP_INTERVAL_MILLISECONDS);
            }
        }

        private void LogError(string filepath, string message, int lineNumber)
        {
            var lineMessage = "LINE_NUM : " + lineNumber;
            var errorMessage = "ERROR_MSG : " + message;
            File.AppendAllLines(Path.Combine(errorDirectory, filepath), new[] { lineMessage, errorMessage });
        }

        private bool ValidHeaders(IEnumerable<string> headers)
        {
            var expected = new[] { "INTERNAL_ID", "FIRST_NAME", "MIDDLE_NAME", "LAST_NAME", "PHONE_NUM" };
            return expected.SequenceEqual(headers.Select(header => header.ToUpper().Trim()));
        }
    }
}