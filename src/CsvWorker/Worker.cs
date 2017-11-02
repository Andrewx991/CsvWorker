using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace CsvWorker
{
    internal class Worker
    {
        private readonly string inputDirectory;
        private readonly string outputDirectory;
        private readonly string errorDirectory;
        private const int SLEEP_INTERVAL_MILLISECONDS = 5000;
        private HashSet<string> processedFiles = new HashSet<string>();

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
                var files = Directory.GetFiles(inputDirectory);

                var unprocessedFiles = files.Except(processedFiles);

                foreach (var filepath in unprocessedFiles)
                {
                    var filename = Path.GetFileName(filepath);
                    processedFiles.Add(filename);
                    int currentLineNumber = 1;

                    using (var reader = new StreamReader(filepath))
                    using (var streamWriter = new StreamWriter(Path.Combine(outputDirectory, filename.Replace("csv", "json"))))
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartArray();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();

                            if (String.IsNullOrWhiteSpace(line))
                                continue;

                            var values = line.Split(',');

                            if (currentLineNumber == 1)
                            {
                                if (!ValidHeaders(values))
                                {
                                    LogError(filename, line);
                                }
                            }
                            else
                            {
                                Entry entry = null;
                                var parsed = Entry.TryParse(values, out entry);

                                if(!parsed)
                                {
                                    LogError(filename, line);
                                    break;
                                }
                                else
                                {
                                    var settings = new JsonSerializerSettings();
                                    settings.NullValueHandling = NullValueHandling.Ignore;
                                    var jsonEntry = JsonConvert.SerializeObject(entry, Formatting.Indented, settings);
                                    jsonWriter.WriteRawValue(jsonEntry);
                                }
                            }

                            currentLineNumber++;
                        }

                        jsonWriter.WriteEndArray();
                    }

                    File.Delete(filepath);
                }

                Thread.Sleep(SLEEP_INTERVAL_MILLISECONDS);
            }
        }

        class Entry
        {
            public string Id { get; set; }
            public Name Name { get; set; }
            public string Phone { get; set; }

            public static bool TryParse(string[] values, out Entry parsedEntry)
            {
                if (values.Count() != 5)
                {
                    parsedEntry = null;
                    return false;
                }

                if (String.IsNullOrWhiteSpace(values[0]) ||
                    String.IsNullOrWhiteSpace(values[1]) ||
                    String.IsNullOrWhiteSpace(values[3]) ||
                    String.IsNullOrWhiteSpace(values[4]))
                {
                    parsedEntry = null;
                    return false;
                }

                if (!new Regex(@"^\d{3}-\d{3}-\d{4}$").IsMatch(values[4].Trim()))
                {
                    parsedEntry = null;
                    return false;
                }

                Entry entry = new Entry();
                entry.Id = values[0].Trim();
                entry.Name = new Name();
                entry.Name.First = values[1].Trim();

                if (!String.IsNullOrWhiteSpace(values[2]))
                    entry.Name.Middle = values[2].Trim();

                entry.Name.Last = values[3].Trim();
                entry.Phone = values[4].Trim();

                parsedEntry = entry;
                return true;
            }
        }

        class Name
        {
            public string First { get; set; }
            public string Middle { get; set; }
            public string Last { get; set; }
        }

        private void LogError(string filepath, string line)
        {
            File.AppendAllLines(Path.Combine(errorDirectory, filepath), new[] { line });
        }

        private bool ValidHeaders(IEnumerable<string> headers)
        {
            var expected = new[] { "INTERNAL_ID", "FIRST_NAME", "MIDDLE_NAME", "LAST_NAME", "PHONE_NUM" };
            return expected.SequenceEqual(headers.Select(header => header.ToUpper()));
        }
    }
}