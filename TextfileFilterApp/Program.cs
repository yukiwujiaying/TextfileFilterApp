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
            string logFilePath = @"C:\Users\ywu11\Downloads\failtests03-04-2024.testlog";

            string newlogfilepath = @"C:\Users\ywu11\Downloads\RunAll-638447310080012496.testlog";
            string filterOutputFilePath = @"C:\Users\ywu11\Downloads\filtered_log_originalfail.txt";
            string filtertaxduePath = @"C:\Users\ywu11\Downloads\filtered_taxDuefail.txt";
            string filterAdjustedTotalIncomePath = @"C:\Users\ywu11\Downloads\filterAdjustedTotalIncome.txt";
            string filterAdjustedNetIncomePath = @"C:\Users\ywu11\Downloads\filterAdjustedNetIncome.txt";
            string filterPersonalAllowanceDeductionPath = @"C:\Users\ywu11\Downloads\filterPersonalAllowanceDeduction.txt";

            string taxYear201415FilterString = "2014/2015";
            string taxYear201516FilterString = "2015/2016";

            string totalIncomefailTestsPath = @"C:\Users\ywu11\Downloads\filterAdjustedTotalIncome03-04-2024.txt";
            string failTotalIncomeTestCasesIn201415Path = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesIn2014-2015.txt";
            string failTotalIncomeTestCasesIn201516Path = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesIn2015-2016.txt";

            string netIncomeFailedTestsPath = @"C:\Users\ywu11\Downloads\filterAdjustedNetIncome03-04-2024.txt";
            string netIncomeFailedTestsIn201415Path = @"C:\Users\ywu11\Downloads\NetIncomeFailedTestsIn2014-2015.txt";
            string netIncomeFailedTestsIn201516Path = @"C:\Users\ywu11\Downloads\NetIncomeFailedTestsIn2015-2016.txt";

            string netIncomeFailedTestsIn201415RefCodePath = @"C:\Users\ywu11\Downloads\NetIncomeFailedTestCasesIn2014-2015RefCode.txt";
            string netIncomeFailedTestsIn201516RefCodePath = @"C:\Users\ywu11\Downloads\NetIncomeFailedTestCasesIn2015-2016RefCode.txt";

            string totalIncomeFailedTestsIn201415RefCodePath = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesIn2014-2015RefCode.txt";
            string totalIncomeFailedTestsIn201516RefCodePath = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesIn2015-2016RefCode.txt";

            string totalIncomeFailedTestsWith201415RefCodePath = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesWith2014-2015RefCode.txt";
            string totalIncomeFailedTestsWith201516RefCodePath = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesWith2015-2016RefCode.txt";

            string filterTaxDue = "TaxDueItems";
            var filterAdjustedTotalIncome = "AdjustedTotalIncome";
            var filterAdjustedNetIncome = "AdjustedNetIncome";
            var filterPersonalAllowanceDeduction = "PersonalAllowanceDeduction";
            string filePathForfailcasewithOutgoing = @"C:\Users\ywu11\Downloads\failtestwithoutgoing.testlog";

            //FilterLogEntriesAndSortByRefCode(netIncomeFailedTestsPath, netIncomeFailedTestsIn201415Path, taxYear201415FilterString);
            //FilterLogEntriesAndSortByRefCode(netIncomeFailedTestsPath, netIncomeFailedTestsIn201516Path, taxYear201516FilterString);
            //PrintRefCodesToFile(netIncomeFailedTestsIn201516Path);
            //FilterLogEntriesByRefCodes(totalIncomefailTestsPath, failTotalIncomeTestCasesIn201516Path, totalIncomeFailedTestsWith201516RefCodePath);
            
            FilterFilesWithSameEntry(netIncomeFailedTestsIn201415RefCodePath, netIncomeFailedTestsIn201516RefCodePath);

            //CompareFiles(newlogfilepath, logFilePath);
            //FilterLogEntriesAndSortByRefCode(logFilePath, filterAdjustedTotalIncomePath, filterAdjustedTotalIncome);
            //FilterLogEntriesAndSortByDifferences(logFilePath, filtertaxduePath, filterTaxDue);

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

        static List<string> PrintRefCodesToFile(string logFilePath)
        {
            // Check if the log file exists
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("Log file does not exist.");
                return null;
            }

            string[] logEntries = File.ReadAllLines(logFilePath);

            // HashSet to store unique reference codes
            HashSet<string> refCodes = new HashSet<string>();

            // Extract reference codes from log entries
            foreach (string entry in logEntries)
            {
                string refCode = GetReferenceCode(entry);
                if (!string.IsNullOrEmpty(refCode))
                {
                    refCodes.Add(refCode);
                }
            }

            var totalIncomeFailedTestsIn201415RefCodePath = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesIn2014-2015RefCode.txt";
            var totalIncomeFailedTestsIn201516RefCodePath = @"C:\Users\ywu11\Downloads\TotalIncomeFailedTestCasesIn2015-2016RefCode.txt";

            string netIncomeFailedTestsIn201415RefCodePath = @"C:\Users\ywu11\Downloads\NetIncomeFailedTestCasesIn2014-2015RefCode.txt";
            string netIncomeFailedTestsIn201516RefCodePath = @"C:\Users\ywu11\Downloads\NetIncomeFailedTestCasesIn2015-2016RefCode.txt";


            // Write reference codes to the output file
            File.WriteAllLines(netIncomeFailedTestsIn201516RefCodePath, refCodes);
            return refCodes.ToList();
        }

        static void FilterLogEntriesByRefCodes(string logFilePath, string refcodePath, string filterOutputFilePath)
        {
            // Check if the log file exists
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("Log file does not exist.");
                return;
            }

            var referenceCodes = PrintRefCodesToFile(refcodePath);

            string[] logEntries = File.ReadAllLines(logFilePath);

            List<string> filteredEntries = new List<string>();

            // Filter log entries containing the desired reference codes
            foreach (string entry in logEntries)
            {
                string refCode = GetReferenceCode(entry);
                if (referenceCodes.Contains(refCode))
                {
                    filteredEntries.Add(entry);
                }
            }

            List<string> sortedFilteredEntries = new List<string>(filteredEntries);
            sortedFilteredEntries.Sort((x, y) => GetReferenceCode(x).CompareTo(GetReferenceCode(y)));

            // Write the filtered log entries to a new text file
            File.WriteAllLines(filterOutputFilePath, sortedFilteredEntries);
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

        static void CompareFiles(string filePath1, string filePath2)
        {
            string[] file1Lines = File.ReadAllLines(filePath1);
            string[] file2Lines = File.ReadAllLines(filePath2);
            HashSet<string> differentEntries = new HashSet<string>();

            // Create a HashSet to store unique lines from file 1
            HashSet<string> uniqueLines = new HashSet<string>(file1Lines);

            // Compare each line in file 2 to every line in file 1
            for (int i = 0; i < file2Lines.Length; i++)
            {
                if (!uniqueLines.Contains(file2Lines[i]))
                {
                    differentEntries.Add(file2Lines[i]);
                }
            }

            List<string> sortedFilteredEntries = new List<string>(differentEntries);
            sortedFilteredEntries.Sort((x, y) => GetReferenceCode(x).CompareTo(GetReferenceCode(y)));

            string finalOutputFilePath = GetAvailableFileName(@"C:\Users\ywu11\Downloads\entries_differences_log.txt");
            File.WriteAllLines(finalOutputFilePath, sortedFilteredEntries);
            Console.WriteLine("Comparison complete.");
        }

        static void FilterFilesWithSameEntry(string filePath1, string filePath2)
        {
            // Read all lines from both files
            string[] file1Lines = File.ReadAllLines(filePath1);
            string[] file2Lines = File.ReadAllLines(filePath2);

            // HashSet to store unique rows
            HashSet<string> uniqueRows = new HashSet<string>();

            // Add all lines from file 1 to uniqueRows
            foreach (string line in file1Lines)
            {
                uniqueRows.Add(line);
            }

            // Add all lines from file 2 to uniqueRows
            foreach (string line in file2Lines)
            {
                uniqueRows.Add(line);
            }

            // Sort the unique rows
            List<string> sortedUniqueRows = new List<string>(uniqueRows);
            sortedUniqueRows.Sort((x, y) => GetReferenceCode(x).CompareTo(GetReferenceCode(y)));

            // Write the unique rows to a new text file
            string finalOutputFilePath = GetAvailableFileName(@"C:\Users\ywu11\Downloads\combined_unique_rows.txt");
            File.WriteAllLines(finalOutputFilePath, sortedUniqueRows);

            Console.WriteLine("Combination and filtering complete.");
        }

    }
}

