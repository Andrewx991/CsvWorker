using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CsvWorker
{
    internal partial class Worker
    {
        public class Entry
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
    }
}