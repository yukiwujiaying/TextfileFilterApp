using System;
using System.Collections.Generic;
using System.IO;

namespace TextfileFilterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = @"C:\Users\ywu11\Downloads\taxDueError.testlog";
            string filterOutputFilePath = @"C:\Users\ywu11\Downloads\filtered_log_entries.txt";
            string filterItem = "TaxDueItems";

            FilterLogEntries(logFilePath, filterOutputFilePath, filterItem);

            Console.WriteLine("Filtered log entries have been written to: " + filterOutputFilePath);
        }

        static void FilterLogEntries(string logFilePath, string filterOutputFilePath, string filterItem)
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

            // Write the filtered log entries to a new text file
            File.WriteAllLines(finalOutputFilePath, filteredEntries);
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
    }
}

