using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TextfileFilterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = @"C:\Users\ywu11\Downloads\RunAll-638429836088510776.testlog";
            string filterOutputFilePath = @"C:\Users\ywu11\Downloads\filtered_log_entries.txt";
            string filterItem = "TaxDueItems";

            FilterLogEntriesAndSortByDifferences(logFilePath, filterOutputFilePath, filterItem);

            Console.WriteLine("Filtered log entries have been written to: " + filterOutputFilePath);
        }

        static void FilterLogEntriesAndSortByRefCode(string logFilePath, string filterOutputFilePath, string filterItem)
        {
            // Check if the log file exists
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("Log file does not exist.");
                return;
            }

            string[] logEntries = File.ReadAllLines(logFilePath);

            string finalOutputFilePath = GetAvailableFileName(filterOutputFilePath);

            // Filter log entries containing the desired phrase
            //A HashSet<string> automatically removes duplicates,
            HashSet<string> filteredEntries = new HashSet<string>();
            foreach (string entry in logEntries)
            {
                if (entry.ToLower().Contains(filterItem.ToLower()))
                {
                    filteredEntries.Add(entry);
                }
            }

            // Sort the unique entries by reference code
            List<string> sortedFilteredEntries = new List<string>(filteredEntries);
            sortedFilteredEntries.Sort((x, y) => GetReferenceCode(x).CompareTo(GetReferenceCode(y)));

            // Write the filtered log entries to a new text file
            File.WriteAllLines(finalOutputFilePath, sortedFilteredEntries);
        }

        static void FilterLogEntriesAndSortByDifferences(string logFilePath, string filterOutputFilePath, string filterItem)
        {
            // Check if the log file exists
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("Log file does not exist.");
                return;
            }

            string[] logEntries = File.ReadAllLines(logFilePath);

            string finalOutputFilePath = GetAvailableFileName(filterOutputFilePath);

            // Filter log entries containing the desired phrase
            HashSet<string> filteredEntries = new HashSet<string>();
            foreach (string entry in logEntries)
            {
                if (entry.ToLower().Contains(filterItem.ToLower()))
                {
                    filteredEntries.Add(entry);
                }
            }

            // Sort the unique entries by the difference
            List<string> sortedFilteredEntries = new List<string>(filteredEntries);
            sortedFilteredEntries.Sort((x, y) => GetDifference(x).CompareTo(GetDifference(y)));

            // Write the filtered log entries to a new text file
            File.WriteAllLines(finalOutputFilePath, sortedFilteredEntries);
        }

        static string GetAvailableFileName(string originalFilePath)
        {
            string directory = Path.GetDirectoryName(originalFilePath);
            string fileNameOnly = Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = Path.GetExtension(originalFilePath);
            string finalPath = originalFilePath;

            int counter = 1;
            while (File.Exists(finalPath))
            {
                string tempFileName = string.Format("{0}({1}){2}", fileNameOnly, counter, extension);
                finalPath = Path.Combine(directory, tempFileName);
                counter++;
            }

            return finalPath;
        }

        static string GetReferenceCode(string entry)
        {
            // Assuming the reference code appears after "- " and before ","
            int startIndex = entry.IndexOf("- ") + 2;
            int endIndex = entry.IndexOf(",", startIndex);
            if (startIndex >= 0 && endIndex >= 0)
            {
                return entry.Substring(startIndex, endIndex - startIndex);
            }
            return string.Empty;
        }

        static int GetDifference(string entry)
        {
            // Extract the difference from the entry
            Match match = Regex.Match(entry, @"difference of (-?\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return 0;
        }
    }
}

