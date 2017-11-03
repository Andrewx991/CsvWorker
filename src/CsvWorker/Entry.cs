using System;
using System.Collections.Generic;
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
                var validationRules = new Dictionary<int, Func<string, bool>>()
                {
                    { 0, (s) => !String.IsNullOrWhiteSpace(s) && s.Trim().Length == 8 && s.Trim().All(char.IsDigit) },
                    { 1, (s) => !String.IsNullOrWhiteSpace(s) && s.Trim().Length <= 15 },
                    { 2, (s) => s == null || s.Length <= 15 },
                    { 3, (s) => !String.IsNullOrWhiteSpace(s) && s.Trim().Length <= 15 },
                    { 4, (s) => !String.IsNullOrWhiteSpace(s) && new Regex(@"^\d{3}-\d{3}-\d{4}$").IsMatch(s.Trim()) },
                };

                int index = 0;
                foreach (var value in values)
                {
                    var valid = validationRules[index](value);

                    if (!valid)
                    {
                        parsedEntry = null;
                        return false;
                    }
                    index++;
                }

                if (index != 5)
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